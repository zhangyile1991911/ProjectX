using System;
using System.Collections.Generic;
using System.Data;
using cfg.common;
using UniRx;
using UnityEngine;

public class WaitStateNode : IStateNode
{
    private StateMachine _machine;
    private CompositeDisposable _handles;
    
    // private IDisposable _clockTopic;
    // private IDisposable _fiveSecondTimer;
    
    private RestaurantEnter _restaurant;
    private RestaurantWindow _restaurantWindow;

    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handles = new CompositeDisposable();
    }
    
    public void OnEnter(object param = null)
    {
        var _clocker = UniModule.GetModule<Clocker>();
        _clocker.Topic.Subscribe(TimeGoesOn).AddTo(_handles);
        
        var openData = new FlowControlWindowData();
        openData.StateMachine = _machine;
        UIManager.Instance.OpenUI(UIEnum.RestaurantWindow, (uiBase)=>{_restaurantWindow = uiBase as RestaurantWindow;}, openData);
        UIManager.Instance.OpenUI(UIEnum.OrderQueueWindow,null,null,UILayer.Top);
        
        EventModule.Instance.CharBubbleSub.Subscribe(GenerateChatBubble).AddTo(_handles);
        // EventModule.Instance.CloseRestaurantSub.Subscribe(CloseRestaurant).AddTo(_handles);
        EventModule.Instance.CharacterLeaveSub.Subscribe(character =>
        {
            UserInfoModule.Instance.RemoveWaitingCharacter(character.CharacterId);
        }).AddTo(_handles);
        
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.RestaurantMain);
    }

    public void OnExit()
    {
        _handles?.Clear();
        UIManager.Instance.CloseUI(UIEnum.RestaurantWindow);
    }
    
    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.D))
        {
            _machine.ChangeState<PrepareStateNode>();            
        }
    }
    
    private void TimeGoesOn(DateTime dateTime)
    { //时间流逝
        if (TimeToClose(dateTime))
        {
            return;
        }
        
        handleCharacter(dateTime);
    }

    private async void handleCharacter(DateTime dateTime)
    {
        var ids = pickWhoAppear(dateTime);
        if (ids is not { Count: > 0 }) return;

        foreach (var CharacterId in ids)
        {
            
            if (_restaurant.ExistCharacter(CharacterId))
            {
                continue;
            }
            if (!_restaurant.HaveEmptySeat())
            {
                break;
            }
            
            var man = await CharacterMgr.Instance.CreateCharacter(CharacterId);
            _restaurant.CharacterTakeRandomSeat(man);
        
            var seatPoint = _restaurant.TakeSeatPoint(man.SeatIndex);
            var spawnPoint = _restaurant.RandSpawnPoint();
            
            man.CurBehaviour = new CharacterEnterScene(spawnPoint,seatPoint);

            UserInfoModule.Instance.AddCharacterArrived(CharacterId);
            UserInfoModule.Instance.AddWaitingCharacter(CharacterId);
        }
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
    
    private List<int> pickWhoAppear(DateTime nowTime)
    {
        var characterIds = DataProviderModule.Instance.AtWeekDay((int)nowTime.DayOfWeek);
        var result = new List<int>(4);
        foreach (var cid in characterIds)
        {
            var arrived = UserInfoModule.Instance.RestaurantCharacterArrived(cid);
            if(arrived)continue;
            
            var tbScheduler = DataProviderModule.Instance.GetCharacterScheduler(cid);
            foreach (var info in tbScheduler.CharacterAppearInfos)
            {
                if((DayOfWeek)info.Weekday != nowTime.DayOfWeek)continue;

                if (info.EnterTime.Hour == nowTime.Hour)
                {
                    if (nowTime.Minute < info.EnterTime.Minutes) continue;
                    result.Add(cid);
                    break;
                }

                var startHour = info.EnterTime.Hour < 6 ? info.EnterTime.Hour + 24 : info.EnterTime.Hour;
                var leaveHour = info.LeaveTime.Hour < 6 ? info.LeaveTime.Hour + 24 : info.LeaveTime.Hour;
                if(nowTime.Hour > startHour  && nowTime.Hour < leaveHour)
                {
                    result.Add(cid);
                    break;
                }

                if (nowTime.Hour != info.LeaveTime.Hour) continue;
                if (nowTime.Minute >= info.LeaveTime.Minutes) continue;
                result.Add(cid);
                break;
            }
        }

        return result;
    }
    
    private void GenerateChatBubble(int chatId)
    {
        var TBbubble = DataProviderModule.Instance.GetCharacterBubble(chatId);
        var restaurantCharacter = CharacterMgr.Instance.GetCharacterById(TBbubble.NpcId);
        if (chatId > 0)
        {
            _restaurantWindow.GenerateChatBubble(chatId,restaurantCharacter,OnClickBubble);
        }
    }

    // private void CloseRestaurant(Unit param)
    // {
    //     Debug.Log("WaitStateNode CloseRestaurant");
    //     _machine.ChangeState<StatementStateNode>(null);
    // }

    private void OnClickBubble(ChatBubble bubble)
    {
        // var dm = UniModule.GetModule<DialogueModule>();
        // dm.CurentDialogueRestaurantCharacter = bubble.Owner;

        _restaurantWindow.RemoveChatBubble(bubble);
        
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
                EventModule.Instance.OrderMealTopic.OnNext(info);
                break;
        }
    }

    // private void EnterDialogue(RestaurantCharacter character)
    // {
    //     var chatId = character.GenerateChatId();
    //     
    //     var stateData = new DialogueStateNodeData();
    //     stateData.ChatId = chatId;
    //     stateData.ChatRestaurantCharacter = character;
    //     _machine.ChangeState<DialogueStateNode>(stateData);
    // }
}
