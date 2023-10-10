using System;
using System.Collections;
using System.Collections.Generic;
using cfg.common;
using UnityEngine;

public class  GameDateTime
{
    public WeekDay DayOfWeek => _WeekDay;
    private WeekDay _WeekDay;
    public long Year => _year+2045;
    private long _year;

    public Season Season => _season;
    private Season _season;
    public long Day => _day;
    private long _day;
    public long Hour => _hour;
    private long _hour;
    public long Minute => _minutes;
    private long _minutes;
    public long Seconds => _seconds;
    private long _seconds;

    public long Timestamp
    {
        get;
        private set;
    }

    private GameDateTime(long ts)
    {
        Timestamp = ts;
    }
    
    
    public void AddSeconds(int i)
    {
        Timestamp += i;
        _seconds += i;
        while (_seconds >= 60)
        {
            _seconds -= 60;
            _minutes += 1;
        }

        while (_minutes >= 60)
        {
            _hour += 1;
            _minutes -= 60;
        }

        while (_hour >= HourOfEndDay)
        {
            _day += 1;

            var isNewWeek = false;
            var wd = (int)_WeekDay + 1;
            if (wd > (int)WeekDay.Sunday)
            {
                _WeekDay = WeekDay.Monday;
                isNewWeek = true;
            }
            else
            {
                _WeekDay = (WeekDay)wd;
            }
            //重置回到下一天晚上八点
            _hour = 20;
            _minutes = 0;
            _seconds = 0;
            
            EventModule.Instance.ToNextDayTopic.OnNext(this);
            if (isNewWeek)
            {
                EventModule.Instance.ToNextWeekTopic.OnNext(this);    
            }
        }

        while (_day > 30)
        {
            var now_seaon = (int)_season;
            now_seaon += 1;
            if (now_seaon > (int)cfg.common.Season.Winnter)
            {
                _season = cfg.common.Season.Spring;
            }
            else
            {
                _season = (Season)now_seaon;
            }

            _day = 1;
        }
    }
    
    public long NextDay()
    {
        var remainHour = HourOfEndDay - _hour;
        var remainMinutes = 60 - _minutes;
        var addSecond = remainHour * 60 * 60 - remainMinutes * 60;
        //重置到第二天晚上八点
        addSecond += 20 * 60 * 60;
        return addSecond;
    }
    
    public const long DayOfSeason = 30;
    public const long SecondOfDay = 60 * 60 * (24+5);
    public const long SecondOfSeason = SecondOfDay * 30;
    public const long SecondOfYear = SecondOfSeason * 4;
    private const int InitialYear = 2045;
    private const int InitialSeason = 1;
    private const int InitialDay = 1;
    private const int InitialHour = 8;
    private const int InitialMinute = 0;
    private const int InitialSecond = 0;
    public const long HourOfEndDay = 30;
    public static TimeSpan operator -(GameDateTime a,GameDateTime b)
    {
        var a_total_second = a.FromInitialDay();
        var b_total_second = b.FromInitialDay();

        var diff = a_total_second > b_total_second
            ? a_total_second - b_total_second
            : b_total_second - a_total_second;
        
        TimeSpan span = new TimeSpan(diff*1000*1000*10);
        return span;
    }

    private long FromInitialDay()
    {
        long result = 0;
        var big_year_from_inital = _year - InitialYear;
        var big_season_from_initial = _season - InitialSeason;
        var big_day_from_initial = _day - InitialDay;
        
        if (big_year_from_inital > 0)
        {
            result += SecondOfYear * big_year_from_inital;
        }

        if (big_season_from_initial > 0)
        {
            result += SecondOfSeason * (int)big_season_from_initial;
        }

        if (big_day_from_initial > 0)
        {
            result += SecondOfDay * big_day_from_initial;
        }

        result += _hour * 3600;
        result += _minutes * 60;
        result += _seconds;
        return result;
    }
    
    public string WeekDayStr()
    {
        switch (DayOfWeek)
        {
            case WeekDay.Monday:
                return "星期一";
            case WeekDay.Tuesday:
                return "星期二";
            case WeekDay.Wednesday:
                return "星期三";
            case WeekDay.Thursday:
                return "星期四";
            case WeekDay.Friday:
                return "星期五";
            case WeekDay.Saturday:
                return "星期六";
            case WeekDay.Sunday:
                return "星期日";
        }
        return "";
    }


    public static GameDateTime From(long ts)
    {
        long tmp = 0;
        
        GameDateTime result = new GameDateTime(ts);
        
        result._year = ts /SecondOfYear;
        var yearSeconds = result._year * SecondOfYear;
        tmp = ts - yearSeconds;
        
        result._day = tmp/SecondOfDay;
        var daySeconds = result._day * SecondOfDay;
        tmp -= daySeconds;
        
        result._hour = tmp/3600;
        tmp -= result._hour * 3600;
        
        result._minutes = tmp/60;
        tmp -= result._minutes * 60;
        
        result._seconds = tmp;
        
        result._season = (Season)(result._day/DayOfSeason) + 1;
        result._WeekDay = (WeekDay)(result._day % 7)+1;
        result._day %= DayOfSeason;
        return result;
    }
    public static bool operator >(GameDateTime a,GameDateTime b)
    {
        if (a._year > b._year) return true;
        if (a._year < b._year) return false;
        if (a._season > b._season) return true;
        if (a._season < b._season) return false;
        if (a._day > b._day) return true;
        if (a._day < b._day) return false;
        if (a._hour > b._hour) return true;
        if (a._hour < b._hour) return false;
        if (a._minutes > b._minutes) return true;
        if (a._minutes < b._minutes) return false;
        if (a._seconds > b._seconds) return true;
        if (a._seconds < b._seconds) return false;
        return false;
    }

    public static bool operator <(GameDateTime a, GameDateTime b)
    {
        return !(a > b);
    }
}
