using System;
using System.Collections.Generic;
using System.Linq;
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

        // _fiveSecondTimer = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(fiveSecondLoop);
        
        UIManager.Instance.OpenUI(UIEnum.RestaurantWindow, (uiBase)=>{_restaurantWindow = uiBase as RestaurantWindow;}, null);
        UIManager.Instance.OpenUI(UIEnum.OrderQueueWindow,null,null,UILayer.Top);
        
        EventModule.Instance.CharBubbleSub.Subscribe(GenerateChatBubble).AddTo(_handles);

        CreateBoss();
    }

    public void OnExit()
    {
        _handles?.Clear();
        UIManager.Instance.CloseUI(UIEnum.RestaurantWindow);
    }
    
    public void OnUpdate()
    {
        // foreach (var one in _restaurant.CharacterEnumerable)
        // {
        //     one.CurBehaviour.Update();
        // }
        if (Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.D))
        {
            _machine.ChangeState<PrepareStateNode>();            
        }
    }

    private async void CreateBoss()
    {
        var man = CharacterMgr.Instance.GetCharacterById(10005);
        if (man == null)
        {
            man = await CharacterMgr.Instance.CreateCharacter(10005);    
        }

        man.gameObject.transform.position = new(0,0,-13f);
    }
    private async void TimeGoesOn(DateTime dateTime)
    { //时间流逝
        var ids = pickWhoAppear(dateTime);
        if (ids is not { Count: > 0 }) return;
        
        var module = UniModule.GetModule<DataProviderModule>();

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
        }
    }

    private List<int> pickWhoAppear(DateTime dateTime)
    {
        var module = UniModule.GetModule<DataProviderModule>();
        return module.AtWeekDay((int)dateTime.DayOfWeek);
    }
    
    private void GenerateChatBubble(Character character)
    {
        var chatId = character.HaveChatId();
        if (chatId > 0)
        {
            // var uiWindow = _uiManager.Get(UIEnum.RestaurantWindow) as RestaurantWindow;
            _restaurantWindow.GenerateChatBubble(chatId,character,OnClickBubble);
        }
    }

    public void OnClickBubble(ChatBubble bubble)
    {
        var dm = UniModule.GetModule<DialogueModule>();
        dm.CurentDialogueCharacter = bubble.Owner;

        _restaurantWindow.RemoveChatBubble(bubble);
        
        var read = UserInfoModule.Instance.HaveReadDialogueId(bubble.ChatId);
        if (read) return;
        
        UserInfoModule.Instance.InsertReadDialogueId(bubble.ChatId);

        var stateData = new DialogueStateNodeData();
        stateData.ChatId = bubble.ChatId;
        stateData.ChatCharacter = bubble.Owner;
        _machine.ChangeState<DialogueStateNode>(stateData);
    }
}
