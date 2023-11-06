using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class HandleOrderWindow : UIWindow
{

    public override void OnCreate()
    {
        base.OnCreate();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    private Dictionary<int, NPCOrderTable>.ValueCollection orderCollection;
    private Dictionary<int, NPCOrderTable>.ValueCollection.Enumerator orderEnumerator;
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        orderCollection = UserInfoModule.Instance.NPCOrderList;
        orderEnumerator = orderCollection.GetEnumerator();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    /*
    x点x分
    顾客: 杀人兔先生
    ---------------------------------------------

    菜品名称\t          \t单价\t         \t数量\t      小计
    美味章鱼烧\t12\t\t1\t      1360
    美味章鱼烧\t12\t\t1\t      1360

    ---------------------------------------------

    备注:

    加麻加辣!加麻加辣!
 */
    private const string OrderInfoFormat = "    {0}点{1}分\n    顾客: {2}\n    ---------------------------------------------\n\n    菜品名称\\t          \\t单价\\t         \\t数量\\t      小计\n{3}---------------------------------------------\n\n    备注:\n\n    {4}";
    private const string OrderDetailFormat = "    {0}\\t{1}\\t\\t1\\t      {2}\n";
    private const string OrderFlavorFormat = "    加{0}";
    private void showCurOrderInfo()
    {
        var tmp= orderEnumerator.Current;
        if (tmp == null)
        {
            return;
        }
        // using(var sb = ZString.CreateStringBuilder())
        // {
        //     sb.Append("foo");
        //     sb.AppendLine(42);
        //     sb.AppendFormat("{0} {1:.###}", "bar", 123.456789);
        // }
        var charaTb = DataProviderModule.Instance.GetCharacterBaseInfo(tmp.CharacterId);
        var menuTb = DataProviderModule.Instance.GetMenuInfo(tmp.MenuId);
        Txt_OrderDetail.text = ZString.Format(OrderInfoFormat,
            tmp.Hour,
            tmp.Minutes,
            charaTb.Name,
            ZString.Format(OrderDetailFormat,menuTb.Name));
        
    }
}