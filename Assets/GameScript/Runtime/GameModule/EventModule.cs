using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class EventModule : IModule
{
    public IObserver<Character> CharBubbleTopic => _charBubble;

    public IObservable<Character> CharBubbleSub => _charBubble;
    
    //人物发送气泡
    private Subject<Character> _charBubble;
    public void OnCreate(object createParam)
    {
        _charBubble = new Subject<Character>();
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        _charBubble.OnCompleted();
    }
}
