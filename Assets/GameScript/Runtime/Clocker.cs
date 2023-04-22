using System;
using UniRx;

public class Clocker : IModule
{
    public IObservable<long> Topic;
    public long Now => _nowMs;
    
    private Subject<long> _subject;
    private long _nowMs;
    public void OnCreate(object createParam)
    {
        _subject = new Subject<long>();
        Topic = _subject;
        //读取数据库或者json
        _nowMs = 1682155171369;
    }

    public void AddOneSecond()
    {
        AddSecond(1);    
    }
    
    public void AddSecond(int i)
    {
        _nowMs += 1000 * i;
        _subject.OnNext(_nowMs);
    }

    public void AddOneMinute()
    {
        AddMinute(1);
    }

    public void AddMinute(int i)
    {
        _nowMs += 1000 * 60 * i;
        _subject.OnNext(_nowMs);
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        _subject.OnCompleted();
    }
}
