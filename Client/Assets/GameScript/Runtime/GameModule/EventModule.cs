using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class EventModule : SingletonModule<EventModule>
{
    //人物发送气泡
    public IObserver<int> CharBubbleTopic => _charBubble;
    public IObservable<int> CharBubbleSub => _charBubble;
    private Subject<int> _charBubble;

    public IObserver<RestaurantCharacter> CharDialogTopic => _charDialogue;
    public IObservable<RestaurantCharacter> CharDialogSub => _charDialogue;
    private Subject<RestaurantCharacter> _charDialogue;
    //发送订单
    public IObserver<OrderMealInfo> OrderMealTopic => _orderMeal;
    public IObservable<OrderMealInfo> OrderMealSub => _orderMeal;
    
    private Subject<OrderMealInfo> _orderMeal;

    public IObserver<PickFoodAndTools> StartCookTopic => _startCook;
    public IObservable<PickFoodAndTools> StartCookSub => _startCook;
    private Subject<PickFoodAndTools> _startCook;

    public IObserver<bool> CookGameStartTopic => _cookGameStart;
    public IObservable<bool> CookGameStartSub => _cookGameStart;
    private Subject<bool> _cookGameStart;
    
    public IObservable<CookResult> CookFinishSub => _cookFinish;
    public IObserver<CookResult> CookFinishTopic => _cookFinish;
    private Subject<CookResult> _cookFinish;

    public IObservable<Unit> ExitKitchenSub => _exitKitchen;
    public IObserver<Unit> ExitKitchenTopic => _exitKitchen;
    private Subject<Unit> _exitKitchen;

    public override void OnCreate(object createParam)
    {
        _charBubble = new Subject<int>();
        _orderMeal = new Subject<OrderMealInfo>();
        _startCook = new Subject<PickFoodAndTools>();
        _cookGameStart = new Subject<bool>();
        _exitKitchen = new Subject<Unit>();
        _cookFinish = new Subject<CookResult>();
        _charDialogue = new Subject<RestaurantCharacter>();
        base.OnCreate(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        // _charBubble.OnCompleted();
        _orderMeal.OnCompleted();
        _startCook.OnCompleted();
        _cookGameStart.OnCompleted();
        _exitKitchen.OnCompleted();
        _cookFinish.OnCompleted();
        _charDialogue.OnCompleted();
    }
}
