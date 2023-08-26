using System;
using System.Collections;
using System.Collections.Generic;
using cfg.common;
using Cysharp.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class ClockWidget : UIComponent
{
    public ClockWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        base.OnCreate();
        var clocker = UniModule.GetModule<Clocker>();
        Txt_timer.text = ZString.Format("{0:D2}:{1:D2}",clocker.NowDateTime.Hour,clocker.NowDateTime.Minute);
        Txt_weekday.text = WeekDayStr(clocker.NowDateTime.DayOfWeek);
        clocker.Topic.Subscribe(nowms =>
        {
            Txt_weekday.text = WeekDayStr(clocker.NowDateTime.DayOfWeek);
            Txt_timer.text = ZString.Format("{0:D2}:{1:D2}",nowms.Hour,nowms.Minute);
        }).AddTo(uiTran);
    }

    private string WeekDayStr(WeekDay day)
    {
        switch (day)
        {
            case WeekDay.Monday:
                return "周一";
            case WeekDay.Tuesday:
                return "周二";
            case WeekDay.Wednesday:
                return "周三";
            case WeekDay.Thursday:
                return "周四";
            case WeekDay.Friday:
                return "周五";
            case WeekDay.Saturday:
                return "周六";
            case WeekDay.Sunday:
                return "周日";
        }

        return "";
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}