using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class MealOrderComponent : UIComponent
{
    public OrderMealInfo OrderMealInfo => _orderMealInfo;
    private OrderMealInfo _orderMealInfo;
    public MealOrderComponent(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }

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
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }

    public void SetMealOrderInfo(OrderMealInfo info,Vector3 startPos,Vector3 endPos)
    {
        _orderMealInfo = info;
        var tbCharacter = DataProviderModule.Instance.GetCharacterBaseInfo(_orderMealInfo.CharacterId);
        Txt_Name.text = tbCharacter.Name;
        uiRectTran.localPosition = startPos;
        uiRectTran.DOAnchorPos(endPos,0.5f).SetAutoKill(true);
    }

    public void RearrangeOrderInfo(Vector3 endPos)
    {
        uiRectTran.DOAnchorPos(endPos,0.5f).SetAutoKill(true);
    }
}