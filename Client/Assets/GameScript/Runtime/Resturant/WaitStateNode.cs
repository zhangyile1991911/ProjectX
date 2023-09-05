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

    class LineupInfo
    {
        public int[] characterIds;
    }
    private LinkedList<LineupInfo> _waitingCharacter;
    
    private long _previousTs;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handles = new CompositeDisposable();
        _waitingCharacter = new LinkedList<LineupInfo>();
    }
    
    public void OnEnter(object param = null)
    {
        _clocker = UniModule.GetModule<Clocker>();
        _clocker.Topic.Subscribe(TimeGoesOn).AddTo(_handles);
        _previousTs = _clocker.NowDateTime.Timestamp;
        
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
            _clocker.NowDateTime.DayOfWeek,
            (int)_clocker.NowDateTime.Hour);
        loadShopkeeper();
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
    
    private async void loadShopkeeper()
    {
        //创建老板
        var characterObj = await CharacterMgr.Instance.CreateCharacter(10004);
        characterObj.gameObject.SetActive(false);
    }
    
    private void TimeGoesOn(GameDateTime dateTime)
    { //时间流逝
        if (TimeToClose(dateTime))
        {
            return;
        }

        refreshCrowdSchedule();
           
        handleCharacter(dateTime);
    }

    private void refreshCrowdSchedule()
    {
        _crowdSchedule = DataProviderModule.Instance.WeekDayHourCrowd(
            _clocker.NowDateTime.DayOfWeek,
            (int)_clocker.NowDateTime.Hour);
    }

    private LineupInfo fetchWaitingCharacterId()
    {
        /*现在是链表
         如果当前链表头的角色不满足条件,无法出场,就放到链表尾部
        */
        int emptySeatNum = _restaurant.EmptySeatNum();
        int count = _waitingCharacter.Count;
        for (int i = count; i > 0; i--)
        {
            //debug 之后删除------------------------
            foreach (var id in _waitingCharacter)
            {
                Debug.Log($"before create handleCharacter {id} in waiting");
            }
            //-------------------------------------
            
            var info = _waitingCharacter.First.Value;
            _waitingCharacter.RemoveFirst();
            
            Debug.Log($"take head character {info.characterIds}");

            var enoughSpace = checkSeatNum(info,emptySeatNum);
            if (!enoughSpace)
            {//当前位置不足,放到队列末尾
                _waitingCharacter.AddLast(info);
                continue;
            }

            var existed = false;
            for (int x = 0;x < info.characterIds.Length; x++)
            {
                existed = _restaurant.ExistWaitingCharacter(info.characterIds[x]);
                if (existed) break;
            }
             
            if (existed)
            {//已经存在就直接删除
                _waitingCharacter.RemoveFirst();
                continue;
            }
            
            return info; 
        }

        return null;
    }
    
    private bool isLoading = false;
    private async void handleCharacter(GameDateTime dateTime)
    {
        if (isLoading) return;
        //检查是否有空位--->检查出场条件--->创建角色
        var seatNum = _restaurant.EmptySeatNum();
        if (seatNum <= 0) return;
        
        var distance = (dateTime.Timestamp - _previousTs)/60L ;
        //每经过15分钟（可配置）生成一次随机数，判定是否有客人入座
        var interval = DataProviderModule.Instance.CustomerEnterInterval();
        if (_waitingCharacter.Count > 0 && distance >= interval)
        {
            _previousTs = dateTime.Timestamp;
            
            var lineup = fetchWaitingCharacterId();
            isLoading = true;
            for (int i = 0; i < lineup.characterIds.Length; i++)
            {
                await loadCharacter(lineup.characterIds[i]);    
            }
            isLoading = false;
            Debug.Log($"after create handleCharacter {lineup.characterIds}");
        }
        
        var npcId = filterNPCAppear(dateTime);
        if (npcId != null)
        {
            _waitingCharacter.AddFirst(npcId);    
        }
    }

    private async UniTask loadCharacter(int CharacterId)
    {
        Debug.Log($"loadCharacter = {CharacterId}");
        // var tbScheduler = DataProviderModule.Instance.GetCharacterScheduler(CharacterId);
        //一般NPC  
        var character = await CharacterMgr.Instance.CreateCharacter(CharacterId);
        Debug.Log($"await CharacterMgr.Instance.CreateCharacter");
        var seatPoint = _restaurant.CharacterTakeSeat(character);
        
        character.CurBehaviour = new CharacterEnterScene(seatPoint);
        UserInfoModule.Instance.AddCharacterArrivedAndWaiting(CharacterId);
        
        // //特别NPC
        // if (tbScheduler.PartnerId <= 0) return;
        // if (character.PartnerID < 0) return;
        // var partner = await CharacterMgr.Instance.CreateCharacter(tbScheduler.PartnerId);
        // var partnerSeatPoint = _restaurant.CharacterTakeSeat(partner);
        // partner.CurBehaviour = new CharacterEnterScene(partnerSeatPoint);
        //
        // UserInfoModule.Instance.AddCharacterArrivedAndWaiting(tbScheduler.PartnerId);
    }

    private bool TimeToClose(GameDateTime dateTime)
    {
        //暂定三点关门
        if (dateTime.Hour == 24+2)
        {
            _machine.ChangeState<StatementStateNode>(null);
            return true;
        }
        return false;
    }

    private bool IsInLineup(int characterId)
    {
        foreach (var group in _waitingCharacter)
        {
            foreach (var one in group.characterIds)
            {
                if (characterId == one) return true;
            }
        }
        return false;
    }
    private LineupInfo filterNPCAppear(GameDateTime nowTime)
    {
        // var characterIds = DataProviderModule.Instance.AtWeekDay((int)nowTime.DayOfWeek);
        var characterIds = CharacterScheduler.Instance.AppearCharacterIdAtWeekDay(nowTime.DayOfWeek);
        // var result = new List<int>(4);
        foreach (var cid in characterIds)
        {
            if (_restaurant.ExistWaitingCharacter(cid)) continue;
            if(IsInLineup(cid))continue;
            var arrived = UserInfoModule.Instance.RestaurantCharacterArrived(cid);
            if(arrived)continue;

            var onTime = checkCharacterAppearTime(cid, nowTime);
            if(!onTime)continue;

            LineupInfo info = new LineupInfo();
            info.characterIds = new[] { cid };
            return info;
        }

        return null;
    }

    private bool checkSeatNum(LineupInfo info,int curSeatNum)
    {
        // var tbScheduler = DataProviderModule.Instance.GetCharacterScheduler(cid);
        var seatNeed = info.characterIds.Length;
        if (seatNeed <= 1)
        {
            return curSeatNum > seatNeed;
        }

        return _restaurant.HaveDoubleSeat();
    }
    private bool checkCharacterAppearTime(int cid,GameDateTime nowTime)
    {
        // var tbScheduler = DataProviderModule.Instance.GetCharacterScheduler(cid);
        
        var tbScheduler =  CharacterScheduler.Instance.CharacterScheduleId(cid);
        foreach (var info in tbScheduler.CharacterAppearInfos)
        {
            if(info.Weekday != nowTime.DayOfWeek)continue;

            bool attend = nowTime.Hour >= info.EnterTimeBegin.Hour;
            if(!attend)continue;
            
            attend = nowTime.Minute >= info.EnterTimeBegin.Minutes;
            if(!attend)continue;

            bool weather = false;
            var weatherNow = WeatherMgr.Instance.NowWeather;
            foreach (var one in info.WeatherLimit)
            {
                if (one == weatherNow)
                {
                    weather = true;
                    break;
                }
            }
            
            if(!weather)continue;
                
            if (nowTime.Hour < info.EnterTimeEnd.Hour)
                return true;
            
            if (nowTime.Hour == info.EnterTimeEnd.Hour)
            {
                return info.EnterTimeEnd.Minutes >= nowTime.Minute;
            }
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
            case bubbleType.HybridComment:
                case bubbleType.SpecifiedComment:
                    case bubbleType.OmakaseComment:
                var stateData = new DialogueStateNodeData();
                stateData.ChatId = bubble.ChatId;
                stateData.ChatRestaurantCharacter = bubble.Owner;
                _machine.ChangeState<DialogueStateNode>(stateData);
                break;
            case bubbleType.SpecifiedOrder:
            case bubbleType.HybridOrder:
            case bubbleType.Omakase:
                OrderMealInfo info = new()
                {
                    MenuId = tbbubble.MenuId,
                    CharacterId = bubble.Owner.CharacterId,
                    OrderType = tbbubble.BubbleType,
                };
                var rc = bubble.Owner as RestaurantCharacter;
                rc.CurOrderInfo = info;
                rc.CurBehaviour = new CharacterWaitOrder();//点击气泡
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
