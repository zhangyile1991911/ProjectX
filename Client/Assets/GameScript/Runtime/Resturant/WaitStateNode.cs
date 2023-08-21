using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using cfg.character;
using cfg.common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Time = cfg.common.Time;

public class WaitStateNode : IStateNode
{
    private StateMachine _machine;
    private CompositeDisposable _handles;
    
    private RestaurantEnter _restaurant;
    private RestaurantWindow _restaurantWindow;

    private float _create_people_interval = 5f;
    private float _cur_create_interval = 0f;
    private Clocker _clocker;

    private Crowd _crowdSchedule;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handles = new CompositeDisposable();
    }
    
    public void OnEnter(object param = null)
    {
        _clocker = UniModule.GetModule<Clocker>();
        _clocker.Topic.Subscribe(TimeGoesOn).AddTo(_handles);
        
        var openData = new FlowControlWindowData();
        openData.StateMachine = _machine;
        UIManager.Instance.OpenUI(UIEnum.RestaurantWindow, (uiBase)=>{_restaurantWindow = uiBase as RestaurantWindow;}, openData);
        // UIManager.Instance.OpenUI(UIEnum.OrderQueueWindow,null,null,UILayer.Top);
        
        EventModule.Instance.CharBubbleSub.Subscribe(GenerateChatBubble).AddTo(_handles);
        // EventModule.Instance.CloseRestaurantSub.Subscribe(CloseRestaurant).AddTo(_handles);
        EventModule.Instance.CharacterLeaveSub.Subscribe(character =>
        {
            UserInfoModule.Instance.RemoveWaitingCharacter(character.CharacterId);
        }).AddTo(_handles);
        
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.RestaurantMain);
        _crowdSchedule = DataProviderModule.Instance.WeekDayHourCrowd(
            (WeekDay)_clocker.NowDateTime.DayOfWeek,
            _clocker.NowDateTime.Hour);
        
        _restaurant.ResumeAllPeople();
    }

    public void OnExit()
    {
        _handles?.Clear();
        UIManager.Instance.CloseUI(UIEnum.RestaurantWindow);
        UIManager.Instance.CloseUI(UIEnum.PhoneWindow);
        
        _restaurant.FrozenAllPeople();
    }
    
    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.D))
        {
            _machine.ChangeState<PrepareStateNode>();            
        }
        handlePeople();
    }
    
    private void TimeGoesOn(DateTime dateTime)
    { //时间流逝
        if (TimeToClose(dateTime))
        {
            return;
        }
        _crowdSchedule = DataProviderModule.Instance.WeekDayHourCrowd(
            (WeekDay)_clocker.NowDateTime.DayOfWeek,
            _clocker.NowDateTime.Hour);
        handleCharacter(dateTime);
    }

    private async void handleCharacter(DateTime dateTime)
    {
        //检查是否有空位--->检查出场条件--->创建角色
        var seatNum = _restaurant.EmptySeatNum();
        if (seatNum <= 0) return;

        while (seatNum > 0)
        {
            var CharacterId = filterNPCAppear(dateTime,ref seatNum);
            // Debug.Log($"filterNPCAppear = {CharacterId}");
            if (CharacterId == 0) break;
            await loadCharacter(CharacterId);
        }
        
        // if (ids is not { Count: > 0 }) return;

        // foreach (var CharacterId in ids)
        // {
            // var man = await CharacterMgr.Instance.CreateCharacter(CharacterId);
            // // _restaurant.CharacterTakeRandomSeat(man);
            // var emptySeatIndex = _restaurant.FindEmptySeatIndex();
            // man.SeatOccupy = emptySeatIndex;
            // var seatPoint = _restaurant.CharacterTakeSeatPoint(man.SeatOccupy,emptySeatIndex);
            // var spawnPoint = _restaurant.RandSpawnPoint();
            //
            // man.CurBehaviour = new CharacterEnterScene(spawnPoint,seatPoint);
            //
            // UserInfoModule.Instance.AddCharacterArrived(CharacterId);
            // UserInfoModule.Instance.AddWaitingCharacter(CharacterId,man.SeatOccupy);
        // }
    }

    private async UniTask loadCharacter(int CharacterId)
    {
        Debug.Log($"loadCharacter = {CharacterId}");
        var tbScheduler = DataProviderModule.Instance.GetCharacterScheduler(CharacterId);
        //一般NPC  
        var character = await CharacterMgr.Instance.CreateCharacter(CharacterId);
        Debug.Log($"await CharacterMgr.Instance.CreateCharacter");
        var seatPoint = _restaurant.CharacterTakeSeat(character);
        
        // var spawnPoint = _restaurant.RandSpawnPoint();
        character.CurBehaviour = new CharacterEnterScene(seatPoint);
      
        UserInfoModule.Instance.AddCharacterArrivedAndWaiting(CharacterId);
        //特别NPC
        if (tbScheduler.PartnerId <= 0) return;
        if (character.PartnerID < 0) return;
        var partner = await CharacterMgr.Instance.CreateCharacter(tbScheduler.PartnerId);
        var partnerSeatPoint = _restaurant.CharacterTakeSeat(partner);
        partner.CurBehaviour = new FollowCharacter(character,partnerSeatPoint);
        
        UserInfoModule.Instance.AddCharacterArrivedAndWaiting(tbScheduler.PartnerId);
    }

    private bool TimeToClose(DateTime dateTime)
    {
        //暂定三点关门
        if (dateTime.Hour == 3)
        {
            _machine.ChangeState<StatementStateNode>(null);
            return true;
        }
        return false;
    }
    
    private int filterNPCAppear(DateTime nowTime,ref int seatNum)
    {
        var characterIds = DataProviderModule.Instance.AtWeekDay((int)nowTime.DayOfWeek);
        // var result = new List<int>(4);
        foreach (var cid in characterIds)
        {
            if (_restaurant.ExistWaitingCharacter(cid)) continue;

            var arrived = UserInfoModule.Instance.RestaurantCharacterArrived(cid);
            if(arrived)continue;

            var onTime = checkCharacterAppearTime(cid, nowTime);
            if(!onTime)continue;

            var canSeat = checkSeatNum(cid,ref seatNum);
            if (!canSeat) continue;

            return cid;
            // result.Add(cid);
            // var tbScheduler = DataProviderModule.Instance.GetCharacterScheduler(cid);
            // foreach (var info in tbScheduler.CharacterAppearInfos)
            // {
            //     if((DayOfWeek)info.Weekday != nowTime.DayOfWeek)continue;
            //
            //     if (info.EnterTime.Hour == nowTime.Hour)
            //     {
            //         if (nowTime.Minute < info.EnterTime.Minutes) continue;
            //         seatNum--;
            //         result.Add(cid);
            //         break;
            //     }
            //
            //     var startHour = info.EnterTime.Hour < 6 ? info.EnterTime.Hour + 24 : info.EnterTime.Hour;
            //     var leaveHour = info.LeaveTime.Hour < 6 ? info.LeaveTime.Hour + 24 : info.LeaveTime.Hour;
            //     if(nowTime.Hour > startHour  && nowTime.Hour < leaveHour)
            //     {
            //         seatNum--;
            //         result.Add(cid);
            //         break;
            //     }
            //
            //     if (nowTime.Hour != info.LeaveTime.Hour) continue;
            //     if (nowTime.Minute >= info.LeaveTime.Minutes) continue;
            //     seatNum--;
            //     result.Add(cid);
            //     break;
            // }
        }

        return 0;
    }

    private bool checkSeatNum(int cid,ref int curSeatNum)
    {
        if (cid == 10002)
        {//假设这是喵老师
            if (curSeatNum >= 2)
            {
                curSeatNum -= 2;
                return true;
            }
        }

        if (curSeatNum >= 1)
        {
            curSeatNum -= 1;
            return true;
        }

        return false;
    }
    private bool checkCharacterAppearTime(int cid,DateTime nowTime)
    {
        var tbScheduler = DataProviderModule.Instance.GetCharacterScheduler(cid);
        foreach (var info in tbScheduler.CharacterAppearInfos)
        {
            if((DayOfWeek)info.Weekday != nowTime.DayOfWeek)continue;

            if (info.EnterTime.Hour == nowTime.Hour)
            {
                if (nowTime.Minute < info.EnterTime.Minutes) continue;
                return true;
            }

            var startHour = info.EnterTime.Hour < 6 ? info.EnterTime.Hour + 24 : info.EnterTime.Hour;
            var leaveHour = info.LeaveTime.Hour < 6 ? info.LeaveTime.Hour + 24 : info.LeaveTime.Hour;
            if(nowTime.Hour > startHour  && nowTime.Hour < leaveHour)
            {
                return true;
            }

            if (nowTime.Hour != info.LeaveTime.Hour) continue;
            if (nowTime.Minute >= info.LeaveTime.Minutes) continue;
            return true;
        }

        return false;
    }


    private void GenerateChatBubble(CharacterSaidInfo info)
    {
        var TBbubble = DataProviderModule.Instance.GetCharacterBubble(info.ChatId);
        var characterObj = CharacterMgr.Instance.GetCharacterById(info.CharacterId);
        if (TBbubble != null && characterObj != null)
        {
            Debug.Log($"GenerateChatBubble chatId = {info.ChatId} characterId = {info.CharacterId}");
            _restaurantWindow.GenerateChatBubble(info.ChatId,characterObj,OnClickBubble);
        }
    }

    private void OnClickBubble(ChatBubble bubble)
    {
        var tbbubble = DataProviderModule.Instance.GetCharacterBubble(bubble.ChatId);
        var read = UserInfoModule.Instance.HaveReadDialogueId(bubble.ChatId);
        if (read) return;
        switch (tbbubble.BubbleType)
        {
            case bubbleType.MainLine:
            case bubbleType.Talk:
            case bubbleType.Comment:
                var stateData = new DialogueStateNodeData();
                stateData.ChatId = bubble.ChatId;
                stateData.ChatRestaurantCharacter = bubble.Owner;
                _machine.ChangeState<DialogueStateNode>(stateData);
                break;
            case bubbleType.Order:
                OrderMealInfo info = new()
                {
                    MenuId = tbbubble.MenuId,
                    CharacterId = bubble.Owner.CharacterId,
                    
                };
                var rc = bubble.Owner as RestaurantCharacter;
                rc.CurOrderInfo = info;
                // EventModule.Instance.OrderMealTopic.OnNext(info);
                break;
        }
        _restaurantWindow.RemoveChatBubble(bubble);
    }
    
    private void handlePeople()
    {
        if (_cur_create_interval <= 0)
        {
            for (int i = 0; i < _crowdSchedule.PeopleNum; i++)
            {
                _restaurant.GeneratePeople();    
            }
            _cur_create_interval = _crowdSchedule.Interval;
        }
        _cur_create_interval -= UnityEngine.Time.deltaTime;
    }
    
}
