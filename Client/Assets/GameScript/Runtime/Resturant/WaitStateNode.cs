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
    
    private CharacterMgr _characterMgr;
    private RestaurantEnter _restaurant;
    private RestaurantWindow _restaurantWindow;
    private UIManager _uiManager;
    
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handles = new CompositeDisposable();
    }
    
    public void OnEnter(object param = null)
    {
        _characterMgr = UniModule.GetModule<CharacterMgr>();
        
        var _clocker = UniModule.GetModule<Clocker>();
        _clocker.Topic.Subscribe(TimeGoesOn).AddTo(_handles);

        // _fiveSecondTimer = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(fiveSecondLoop);
        
        _uiManager = UniModule.GetModule<UIManager>();
        _uiManager.OpenUI(UIEnum.RestaurantWindow, (uiBase)=>{_restaurantWindow = uiBase as RestaurantWindow;}, null);

        var eventModule = UniModule.GetModule<EventModule>();
        eventModule.CharBubbleSub.Subscribe(GenerateChatBubble).AddTo(_handles);
    }

    public void OnExit()
    {
        _handles?.Clear();
        _uiManager.CloseUI(UIEnum.RestaurantWindow);
    }
    
    public void OnUpdate()
    {
        // foreach (var one in _restaurant.CharacterEnumerable)
        // {
        //     one.CurBehaviour.Update();
        // }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _machine.ChangeState<PrepareStateNode>();            
        }
    }

    private async void TimeGoesOn(DateTime dateTime)
    { //时间流逝
        // var characters = checkWhoAppear(dateTime);
        // if (characters == null) return;
        //
        // foreach (var one in characters)
        // {
        //     if (_restaurant.ExistCharacter(one.CharacterId))
        //     {
        //         continue;
        //     }
        //     if (!_restaurant.HaveEmptySeat())
        //     {
        //         break;
        //     }
        //     var man = await _characterMgr.CreateCharacter(one.CharacterId,one.CharacterImage);
        //     _restaurant.CharacterTakeRandomSeat(man);
        //
        //     var seatPoint = _restaurant.TakeSeatPoint(man.SeatIndex);
        //     var spawnPoint = _restaurant.RandSpawnPoint();
        //     
        //     man.CurBehaviour = new CharacterEnterScene(spawnPoint,seatPoint);
        // }
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
            
            var man = await _characterMgr.CreateCharacter(CharacterId);
            _restaurant.CharacterTakeRandomSeat(man);
        
            var seatPoint = _restaurant.TakeSeatPoint(man.SeatIndex);
            var spawnPoint = _restaurant.RandSpawnPoint();
            
            man.CurBehaviour = new CharacterEnterScene(spawnPoint,seatPoint);
        }
    }

    // private List<CharacterAppear> checkWhoAppear(DateTime dateTime)
    // {
    //     return GlobalFunctions.appears.Where(one => one.DayOfWeek == (int)dateTime.DayOfWeek)
    //         .Where(one => dateTime.Hour >= one.startHour)
    //         .Where(one => dateTime.Minute >= one.startMinutes).ToList();
    // }

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
        
        // var uiWindow = _uiManager.Get(UIEnum.RestaurantWindow) as RestaurantWindow;
        _restaurantWindow.RemoveChatBubble(bubble);
        _machine.ChangeState<DialogueStateNode>(bubble.Owner);
    }
}
