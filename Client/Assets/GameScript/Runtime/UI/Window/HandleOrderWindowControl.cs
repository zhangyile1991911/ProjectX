using System.Collections;
using System.Collections.Generic;
using cfg.common;
using cfg.food;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
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
    
    private List<OrderMealInfo>.Enumerator orderEnumerator;
    private int curShowMenuId = -1;
    private int curShowRecipt = -1;
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        orderEnumerator = UserInfoModule.Instance.NpcOrderEnumerator;
        orderEnumerator.MoveNext();
        bindButton();
        showCurOrderInfo();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    void bindButton()
    {
        XBtn_close.OnClick.Subscribe(param =>
        {
            UIManager.Instance.CloseUI(UIEnum.HandleOrderWindow);
        }).AddTo(handles);
        XBtn_recipe.OnClick.Subscribe(param =>
        {
            showRecipt();
        }).AddTo(handles);
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
    private const string HybridOrderInfoFormat = "    {0}点{1}分\n    顾客: {2}\n    ---------------------------------------------\n\n    菜品名称\t          \t单价\t         \t数量\t      小计\n{3}    ---------------------------------------------\n\n    备注:\n\n    {4}";
    private const string SpecifiedOrderInfoFormat = "    {0}点{1}分\n    顾客: {2}\n    ---------------------------------------------\n\n    菜品名称\t          \t单价\t         \t数量\t      小计\n{3}    ---------------------------------------------\n\n";
    private const string OmakaseOrderInfoFormat = "    {0}点{1}分\\n    顾客: {2}\\n    ---------------------------------------------\\n\\n    {3}\\n\\n    ---------------------------------------------\\n\\n";
    private const string OrderDetailFormat = "    {0}\t{1}\t\t1\t      {2}\n";
    private const string OrderFlavorFormat = "    {0}";
    private void showCurOrderInfo()
    {
        
        var tmp= orderEnumerator.Current;
        if (tmp == null)
        {
            return;
        }

        if (curShowMenuId == tmp.MenuId) return;

        switch (tmp.OrderType)
        {
            case bubbleType.SpecifiedOrder:
                showSpecifiedOrder(tmp);
                break;
            case bubbleType.Omakase:
                showOmakaseOrder(tmp);
                break;
            case bubbleType.HybridOrder:
                showHybridOrder(tmp);
                break;
        }
        
        if (curShowMenuId < 0)
        {
            curShowMenuId = orderEnumerator.Current.MenuId;
        }
    }

    private void showSpecifiedOrder(OrderMealInfo info)
    {
        var charaTb = DataProviderModule.Instance.GetCharacterBaseInfo(info.CharacterId);
        var menuTb = DataProviderModule.Instance.GetMenuInfo(info.MenuId);
        var itemTb = DataProviderModule.Instance.GetItemBaseInfo(info.MenuId);
        var cellStr = ZString.Format(OrderDetailFormat, menuTb.Name, itemTb.Sell,itemTb.Sell);
        Txt_OrderDetail.text = ZString.Format(SpecifiedOrderInfoFormat,
            info.OrderTime.Hour,
            info.OrderTime.Minute,
            charaTb.Name,
            cellStr);
    }

    private void showOmakaseOrder(OrderMealInfo info)
    {
        var charaTb = DataProviderModule.Instance.GetCharacterBaseInfo(info.CharacterId);
        var menuTb = DataProviderModule.Instance.GetMenuInfo(info.MenuId);
        var chatTb= DataProviderModule.Instance.GetCharacterBubble(info.DialogueId);
        Txt_OrderDetail.text = ZString.Format(OmakaseOrderInfoFormat,
            info.OrderTime.Hour,
            info.OrderTime.Minute,
            charaTb.Name,
            chatTb.Title);
    }

    private void showHybridOrder(OrderMealInfo info)
    {
        var sb = ZString.CreateStringBuilder();
        foreach (var one in info.flavor)
        {
            sb.Append(one);
        }
        sb.Append(" ");
        
        var charaTb = DataProviderModule.Instance.GetCharacterBaseInfo(info.CharacterId);
        var menuTb = DataProviderModule.Instance.GetMenuInfo(info.MenuId);
        var itemTb = DataProviderModule.Instance.GetItemBaseInfo(info.MenuId);
        var cellStr = ZString.Format(OrderDetailFormat, menuTb.Name, itemTb.Sell,itemTb.Sell);
        Txt_OrderDetail.text = ZString.Format(HybridOrderInfoFormat,
            info.OrderTime.Hour,
            info.OrderTime.Minute,
            charaTb.Name,
            cellStr,
            sb.ToString());
    }

    private const string reciptFormatStr =
        " ---------------\n 材料 :\n {0}\n\n ---------------\n 做法 :\n {1}\n\n---------------\n标签 :\n {2}\n\n反标签 :\n {3}";
    private void showRecipt()
    {
        var tmp= orderEnumerator.Current;
        if (tmp == null)
        {
            return;
        }

        if (curShowRecipt == tmp.MenuId) return;
        
        switch (tmp.OrderType)
        {
            case bubbleType.SpecifiedOrder:
                Tran_LeftArea.gameObject.SetActive(!Tran_LeftArea.gameObject.activeSelf);
                break;
            case bubbleType.Omakase:
                Tran_LeftArea.gameObject.SetActive(false);
                break;
            case bubbleType.HybridOrder:
                Tran_LeftArea.gameObject.SetActive(!Tran_LeftArea.gameObject.activeSelf);
                break;
        }

        var menuTb = DataProviderModule.Instance.GetMenuInfo(tmp.MenuId);
        Txt_Title.text = menuTb.Name;
        
        var sb = ZString.CreateStringBuilder();
        foreach (var one in menuTb.RelatedMaterial)
        {
            var itemTB = DataProviderModule.Instance.GetItemBaseInfo(one);
            sb.Append(itemTB.Name);
            sb.Append(",");
        }
        sb.Remove(sb.Length-1,1);
        var materialStr = sb.ToString();

        sb.Clear();
        foreach (var one in menuTb.Tag)
        {
            sb.Append(DataProviderModule.Instance.FlavourStr((int)one));
            sb.Append(",");
        }
        var tagStr = sb.ToString();
        
        sb.Clear();
        foreach (var one in menuTb.OppositeTag)
        {
            sb.Append(DataProviderModule.Instance.FlavourStr((int)one));
            sb.Append(",");
        }
        var oppositeTagStr = sb.ToString();
        
        Txt_ReceipContent.text = ZString.Format(reciptFormatStr,
            materialStr,
            DataProviderModule.Instance.CookToolStr((int)menuTb.MakeMethod),
            tagStr,oppositeTagStr);

        curShowRecipt = tmp.MenuId;
    }
    
}