using System;
using UniRx;

public class Clocker : IModule
{
    public IObservable<DateTime> Topic;
    public long Now => _nowMs;
    public DateTime NowDateTime => _nowDate;
    private Subject<DateTime> _subject;
    private long _nowMs;
    private DateTime _nowDate;
    public void OnCreate(object createParam)
    {
        _subject = new Subject<DateTime>();
        Topic = _subject;
        //读取数据库或者json
        _nowMs = 1682155171369;
        _nowDate = new DateTime(1970, 1, 1, 8, 0, 0).AddMilliseconds(_nowMs);

    }

    public void AddOneSecond()
    {
        AddSecond(1);    
    }
    
    public void AddSecond(int i)
    {
        _nowDate = _nowDate.AddSeconds(i);
        _subject.OnNext(_nowDate);
    }

    public void AddOneMinute()
    {
        AddSecond(60);
    }

    public void AddMinute(int i)
    {
        // _nowMs += 1000 * 60 * i;
        AddSecond(i*60);
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        _subject.OnCompleted();
    }
}
