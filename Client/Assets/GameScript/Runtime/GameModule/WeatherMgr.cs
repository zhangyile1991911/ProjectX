using System;
using cfg.common;
using UniRx;
using Random = UnityEngine.Random;

public class WeatherMgr : SingletonModule<WeatherMgr>
{
    public WeatherInfo NowWeather { get; private set; }

    public WeatherInfo NextWeather { get; private set; }


    public override void OnCreate(object createParam)
    {
        base.OnCreate(this);
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        nextDaySub?.Dispose();
        
    }

    private IDisposable nextDaySub;
    public void InitWeather()
    {
        var season = Clocker.Instance.NowDateTime.Season;
        var day = Clocker.Instance.NowDateTime.Day;
        NowWeather = new WeatherInfo();
        NowWeather.Weather = DataProviderModule.Instance.DayWeather(season, (int)day);
        var nowGroup = DataProviderModule.Instance.DayWeatherGroup(season, (int)day);
        int a = Random.Range(nowGroup.TempStart, nowGroup.TempEnd);
        int b = Random.Range(nowGroup.TempStart, nowGroup.TempEnd);
        NowWeather.temperature_start = Math.Min(a,b);
        NowWeather.temperature_end = Math.Max(a,b);
        
        nextDayWeather();
        nextDaySub = EventModule.Instance.ToNextDaySub.Subscribe(day =>
        {
            nextDayWeather(day.Season,(int)day.Day);
        });
    }

    private void nextDayWeather()
    {
        var season = Clocker.Instance.NowDateTime.Season;
        var day = Clocker.Instance.NowDateTime.Day;
        var nextDay = day + 1 >= GameDateTime.DayOfSeason ? 1 : day + 1;;
        Season nextSeason = season;
        if (day + 1 >= GameDateTime.DayOfSeason)
        {
            switch (season)
            {
                case Season.Spring:
                    nextSeason = Season.Summer;
                    break;
                case Season.Summer:
                    nextSeason = Season.Autumn;
                    break;
                case Season.Autumn:
                    nextSeason = Season.Winnter;
                    break;
                case Season.Winnter:
                    nextSeason = Season.Spring;
                    break;
            }
        }

        nextDayWeather(nextSeason,(int)nextDay);

    }

    private void nextDayWeather(Season nextSeason,int nextDay)
    {
        NextWeather ??= new WeatherInfo();
        NextWeather.Weather = DataProviderModule.Instance.DayWeather(nextSeason, (int)nextDay);
        var nextGroup = DataProviderModule.Instance.DayWeatherGroup(nextSeason, (int)nextDay);
        int a = Random.Range(nextGroup.TempStart, nextGroup.TempEnd);
        int b = Random.Range(nextGroup.TempStart, nextGroup.TempEnd);
        NextWeather.temperature_start = Math.Min(a,b);
        NextWeather.temperature_end = Math.Max(a,b);
    }
}
