using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class EventModule : SingletonModule<EventModule>
{
    //人物发送气泡
    public IObserver<Character> CharBubbleTopic => _charBubble;
    public IObservable<Character> CharBubbleSub => _charBubble;
    private Subject<Character> _charBubble;
    
    //发送订单
    public IObserver<OrderMealInfo> OrderMealTopic => _orderMeal;
    public IObservable<OrderMealInfo> OrderMealSub => _orderMeal;
    
    private Subject<OrderMealInfo> _orderMeal;

    public IObserver<FoodReceipt> StartCookTopic => _startCook;
    public IObservable<FoodReceipt> StartCookSub => _startCook;
    private Subject<FoodReceipt> _startCook;

    public override void OnCreate(object createParam)
    {
        _charBubble = new Subject<Character>();
        _orderMeal = new Subject<OrderMealInfo>();
        _startCook = new Subject<FoodReceipt>();
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
        _orderMeal.OnCompleted();
    }
}
