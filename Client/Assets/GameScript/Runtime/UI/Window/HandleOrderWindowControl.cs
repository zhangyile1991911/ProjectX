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
    
    private List<OrderMealInfo> orderList;
    private int curNPCMenuId = -1;
    private int curShowRecipt = -1;
    private int curIndex = 0;
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        orderList = UserInfoModule.Instance.NpcOrderList;
        
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
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            curIndex = curIndex - 1 < 0 ? 0 : curIndex - 1;
            showCurOrderInfo();
        }
        else if (Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow))
        {
            curIndex = curIndex + 1 >= orderList.Count ? orderList.Count - 1 : curIndex + 1;
            showCurOrderInfo();
        }
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
        Observable.EveryUpdate().Subscribe((param)=>
        {
            OnUpdate();
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
    private const string SpecifiedOrderInfoFormat = "    {0}点{1}分\n    顾客: {2}\n    ---------------------------------------------\n\n    菜品名称\t \t单价\t  \t数量\t  小计\n{3}    ---------------------------------------------\n\n";
    private const string OmakaseOrderInfoFormat = "    {0}点{1}分\\n    顾客: {2}\\n    ---------------------------------------------\\n\\n    {3}\\n\\n    ---------------------------------------------\\n\\n";
    private const string OrderDetailFormat = "    {0}\t\t{1}\t\t1   {2}\n";
    private const string OrderFlavorFormat = "    {0}";
    private void showCurOrderInfo()
    {
        
        var tmp= orderList[curIndex];
        if (tmp == null)
        {
            return;
        }

        if (curNPCMenuId == tmp.CharacterId) return;

        switch (tmp.OrderType)
        {
            case bubbleType.SpecifiedOrder:
                showSpecifiedOrder(tmp);
                showRecipt();
                break;
            case bubbleType.Omakase:
                showOmakaseOrder(tmp);
                hideRecipt();
                break;
            case bubbleType.HybridOrder:
                showHybridOrder(tmp);
                showRecipt();
                break;
        }

        curNPCMenuId = tmp.CharacterId;
        
    }

    private void showSpecifiedOrder(OrderMealInfo info)
    {
        var charaTb = DataProviderModule.Instance.GetCharacterBaseInfo(info.CharacterId);
        var menuTb = DataProviderModule.Instance.GetMenuInfo(info.MenuId);
        var itemTb = DataProviderModule.Instance.GetItemBaseInfo(info.MenuId);
        
        Tran_Detail.gameObject.SetActive(true);
        
        Txt_time.text = ZString.Format(" {0}:{1}:{2}",info.OrderTime.Hour,info.OrderTime.Minute,info.OrderTime.Seconds);
        Txt_customername.text = ZString.Format("顾客: {0}",charaTb.Name);
        Txt_orderName.text = menuTb.Name; 
        Txt_orderSale.text = itemTb.Sell.ToString();
        Txt_orderNum.text = "1";
        Txt_orderTotal.text = itemTb.Sell.ToString();
        Txt_extra.gameObject.SetActive(false);
    }

    private void showOmakaseOrder(OrderMealInfo info)
    {
        var charaTb = DataProviderModule.Instance.GetCharacterBaseInfo(info.CharacterId);
        // var menuTb = DataProviderModule.Instance.GetMenuInfo(info.MenuId);
        var chatTb= DataProviderModule.Instance.GetCharacterBubble(info.DialogueId);
        
        Txt_time.text = ZString.Format(" {0}:{1}:{2}",info.OrderTime.Hour,info.OrderTime.Minute,info.OrderTime.Seconds);
        Txt_customername.text = ZString.Format("顾客: {0}",charaTb.Name);
        
        Txt_extra.gameObject.SetActive(true);
        Tran_Detail.gameObject.SetActive(false);
        Txt_extra.text = ZString.Format("{0}\n{1}",chatTb.Title,chatTb.FlavorTags);
        Txt_extra.rectTransform.anchoredPosition = _extraOmakasaPosition;
    }

    private readonly Vector2 _extraNormalPosition = new Vector2(-496, 0);
    private readonly Vector2 _extraOmakasaPosition = new Vector2(-496, 200);
    private void showHybridOrder(OrderMealInfo info)
    {
        // var sb = ZString.CreateStringBuilder();
        // foreach (var one in info.flavor)
        // {
        //     sb.Append(one);
        // }
        // sb.Append(" ");
        //
        var charaTb = DataProviderModule.Instance.GetCharacterBaseInfo(info.CharacterId);
        var menuTb = DataProviderModule.Instance.GetMenuInfo(info.MenuId);
        var itemTb = DataProviderModule.Instance.GetItemBaseInfo(info.MenuId);
        var chatTb= DataProviderModule.Instance.GetCharacterBubble(info.DialogueId);
        // var cellStr = ZString.Format(OrderDetailFormat, menuTb.Name, itemTb.Sell,itemTb.Sell);
        // Txt_OrderDetail.text = ZString.Format(HybridOrderInfoFormat,
        //     info.OrderTime.Hour,
        //     info.OrderTime.Minute,
        //     charaTb.Name,
        //     cellStr,
        //     sb.ToString());
        
        Txt_time.text = ZString.Format(" {0}:{1}:{2}",info.OrderTime.Hour,info.OrderTime.Minute,info.OrderTime.Seconds);
        Txt_customername.text = ZString.Format("顾客: {0}",charaTb.Name);
        Txt_orderName.text = menuTb.Name; 
        Txt_orderSale.text = itemTb.Sell.ToString();
        Txt_orderNum.text = "1";
        Txt_orderTotal.text = itemTb.Sell.ToString();
        Txt_extra.gameObject.SetActive(true);
        Txt_extra.text = ZString.Format("{0}",chatTb.Title);
        Txt_extra.rectTransform.anchoredPosition = _extraNormalPosition;
        
    }

    private const string reciptFormatStr =
        " ---------------\n 材料 :\n {0}\n\n ---------------\n 做法 :\n {1}\n\n---------------\n标签 :\n {2}\n\n反标签 :\n {3}";

    private void hideRecipt()
    {
        Tran_LeftArea.gameObject.SetActive(false);
    }
    
    private void showRecipt()
    {
        var tmp= orderList[curIndex];
        if (tmp == null)
        {
            return;
        }
        Tran_LeftArea.gameObject.SetActive(true);
        if (curShowRecipt == tmp.MenuId) return;
        

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