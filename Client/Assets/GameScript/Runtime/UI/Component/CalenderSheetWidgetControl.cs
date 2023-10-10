using System.Collections;
using System.Collections.Generic;
using cfg.common;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class CalenderSheetWidget : UIComponent
{
    public CalenderSheetWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        
    }
    
    public override void OnDestroy()
    {
        
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
        
    }

    public void SetDate(Season season, int day, string weekday)
    {
        Txt_month.text = SeasonStr(season);
        Txt_day.text = day.ToString();
        Txt_weekday.text = weekday;
        
    }

    private string SeasonStr(Season season)
    {
        switch (season)
        {
            case Season.Spring:
                return "Spr.";
            case Season.Summer:
                return "Sum.";
            case Season.Autumn:
                return "Aut.";
            case Season.Winnter:
                return "Win.";
        }

        return "";
    }
    
}