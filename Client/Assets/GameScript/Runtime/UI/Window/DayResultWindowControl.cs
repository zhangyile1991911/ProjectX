using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class DayResultWindow : UIWindow
{

    public override void OnCreate()
    {
        base.OnCreate();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        var headerStr = DataProviderModule.Instance.DaySummaryHeader();
        var commentStr = DataProviderModule.Instance.CustomerComment();
        
        var now  = Clocker.Instance.NowDateTime;
        var sb = ZString.CreateStringBuilder();
        var totalCustomer = UserInfoModule.Instance.RestaurantWaitingCharacter.Count;
        var soldList = UserInfoModule.Instance.SoldMealIdList();
        var totalInCome = 0;
        var expensive  = 0;
        string expensiveName = "";
        foreach (var one in soldList)
        {
            var tb = DataProviderModule.Instance.GetItemBaseInfo(one);
            totalInCome += tb.Sell;
            if (tb.Sell > expensive)
            {
                expensiveName = tb.Name;
            }
        }
        //todo 加入天气
        sb.AppendFormat(headerStr,now.Year,now.Season,now.Day,"晴天",
            totalCustomer,//客人数量
            totalInCome,//总收入
            expensiveName);//单品最高

        foreach (var one in UserInfoModule.Instance.RestaurantWaitingCharacter)
        {
            var characterTb = DataProviderModule.Instance.GetCharacterBaseInfo(one);
            sb.AppendFormat(commentStr,
                characterTb.Name,//顾客名字
                "笑死我了,这上面jb东西");//点评内容
        }
        
        Txt_DailyNote.text = sb.ToString();
        sb.Dispose();
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