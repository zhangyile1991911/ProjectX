using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class MealOrderComponent : UIComponent
{
    private OrderMealInfo _orderMealInfo;
    public MealOrderComponent(GameObject go,UIWindow parent):base(go,parent)
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

    public void SetMealOrderInfo(OrderMealInfo info)
    {
        _orderMealInfo = info;
        Txt_Name.text = _orderMealInfo.Customer.CharacterName;
    }
}