using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI.Checkin;
using UniRx;

public class EventModule : SingletonModule<EventModule>
{
    //人物发送气泡
    public IObserver<CharacterSaidInfo> CharBubbleTopic => _charBubble;
    public IObservable<CharacterSaidInfo> CharBubbleSub => _charBubble;
    private Subject<CharacterSaidInfo> _charBubble;

    // public IObserver<RestaurantCharacter> CharDialogTopic => _charDialogue;
    // public IObservable<RestaurantCharacter> CharDialogSub => _charDialogue;
    // private Subject<RestaurantCharacter> _charDialogue;
    
    //当客人离开时候消息
    public IObserver<RestaurantRoleBase> CharacterLeaveTopic => _characterLeave;
    public IObservable<RestaurantRoleBase> CharacterLeaveSub => _characterLeave;
    private Subject<RestaurantRoleBase> _characterLeave;

    //发送订单
    // public IObserver<OrderMealInfo> OrderMealTopic => _orderMeal;
    // public IObservable<OrderMealInfo> OrderMealSub => _orderMeal;
    
    // private Subject<OrderMealInfo> _orderMeal;

    // public IObserver<PickFoodAndTools> StartCookTopic => _startCook;
    // public IObservable<PickFoodAndTools> StartCookSub => _startCook;
    // private Subject<PickFoodAndTools> _startCook;

    // public IObserver<bool> CookGameStartTopic => _cookGameStart;
    // public IObservable<bool> CookGameStartSub => _cookGameStart;
    // private Subject<bool> _cookGameStart;
    
    // public IObservable<CookResult> CookFinishSub => _cookFinish;
    // public IObserver<CookResult> CookFinishTopic => _cookFinish;
    // private Subject<CookResult> _cookFinish;

    // public IObservable<DialogueNotification> DialogueMsgSub => _dialogueMsg;
    // public IObserver<DialogueNotification> DialogueMsgTopic => _dialogueMsg;
    //
    // private Subject<DialogueNotification> _dialogueMsg;
    // public IObservable<Unit> ExitKitchenSub => _exitKitchen;
    // public IObserver<Unit> ExitKitchenTopic => _exitKitchen;
    // private Subject<Unit> _exitKitchen;


    // public IObservable<Unit> CloseRestaurantSub => _closeRestaurant;
    // public IObserver<Unit> CloseRestaurantTopic => _closeRestaurant;
    // //玩家主动闭店
    // private Subject<Unit> _closeRestaurant;

    public IObserver<GameDateTime> ToNextWeekTopic => _toNextWeek;
    public IObservable<GameDateTime> ToNextWeekSub => _toNextWeek;
    private Subject<GameDateTime> _toNextWeek;


    public IObserver<GameDateTime> ToNextDayTopic => _toNextDay;
    public IObservable<GameDateTime> ToNextDaySub => _toNextDay;
    private Subject<GameDateTime> _toNextDay;


    // private Subject<GameDateTime> _toNextMonth;

    public override void OnCreate(object createParam)
    {
        _charBubble = new Subject<CharacterSaidInfo>();
        // _orderMeal = new Subject<OrderMealInfo>();
        // _startCook = new Subject<PickFoodAndTools>();
        // _cookGameStart = new Subject<bool>();
        // _exitKitchen = new Subject<Unit>();
        // _cookFinish = new Subject<CookResult>();
        // _charDialogue = new Subject<RestaurantCharacter>();
        _characterLeave = new Subject<RestaurantRoleBase>();
        // _closeRestaurant = new();
        _toNextWeek = new Subject<GameDateTime>();
        _toNextDay = new Subject<GameDateTime>();
        base.OnCreate(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _charBubble.OnCompleted();
        // _orderMeal.OnCompleted();
        // _startCook.OnCompleted();
        // _cookGameStart.OnCompleted();
        // _exitKitchen.OnCompleted();
        // _cookFinish.OnCompleted();
        // _charDialogue.OnCompleted();
        _characterLeave.OnCompleted();
        // _closeRestaurant.OnCompleted();
        _toNextWeek.OnCompleted();
    }
}
