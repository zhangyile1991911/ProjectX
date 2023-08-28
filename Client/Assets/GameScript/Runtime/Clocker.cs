using System;
using cfg.common;
using Codice.Client.ChangeTrackerService;

using UniRx;



public class Clocker : SingletonModule<Clocker>
{
    public IObservable<GameDateTime> Topic => _subject;
    public long Now => _nowMs;
    public GameDateTime NowDateTime => _nowDate;
    private Subject<GameDateTime> _subject;
    private long _nowMs;
    private GameDateTime _nowDate;
    public override void OnCreate(object createParam)
    {
        _subject = new Subject<GameDateTime>();
        _nowDate = GameDateTime.From(UserInfoModule.Instance.Now);
        base.OnCreate(this);
    }

    public void AddOneSecond()
    {
        AddSecond(1);    
    }
    
    public void AddSecond(int i)
    {
        _nowDate.AddSeconds(i);
        UserInfoModule.Instance.AddSecond(i);
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

    public void MoveToNextDay()
    {
        var addSecond = _nowDate.NextDay();
        _nowDate.AddSeconds((int)addSecond);
        UserInfoModule.Instance.AddSecond((int)addSecond);
        _subject.OnNext(_nowDate);
        
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnDestroy()
    {
        _subject.OnCompleted();
    }
}
