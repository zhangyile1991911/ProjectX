using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class EventModule : IModule
{
    //人物发送气泡
    public IObserver<Character> CharBubbleTopic => _charBubble;
    public IObservable<Character> CharBubbleSub => _charBubble;
    private Subject<Character> _charBubble;
    
    //发送订单
    public IObserver<OrderMealInfo> OrderMealTopic => _orderMeal;
    public IObservable<OrderMealInfo> OrderMealSub => _orderMeal;
    
    private Subject<OrderMealInfo> _orderMeal;
    public void OnCreate(object createParam)
    {
        _charBubble = new Subject<Character>();
        _orderMeal = new Subject<OrderMealInfo>();
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        _charBubble.OnCompleted();
    }
}
