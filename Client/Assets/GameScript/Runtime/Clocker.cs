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
        
        _nowMs = UserInfoModule.Instance.Now;
        _nowDate = new DateTime(1970, 1, 1, 8, 0, 0).AddMilliseconds(_nowMs);
    }

    public void AddOneSecond()
    {
        AddSecond(1);    
    }
    
    public void AddSecond(int i)
    {
        _nowDate = _nowDate.AddSeconds(i);
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
        DateTime nextDay;
        if (_nowDate.Hour <= 23)
        { 
            nextDay = _nowDate.AddDays(1);
            nextDay = new DateTime(nextDay.Year,nextDay.Month,nextDay.Day,21,0,0);
        }
        else
        {
            nextDay = new DateTime(_nowDate.Year,_nowDate.Month,_nowDate.Day,21,0,0);   
        }

        var distance = (nextDay - _nowDate).TotalSeconds;
        AddSecond((int)distance);
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        _subject.OnCompleted();
    }
}
