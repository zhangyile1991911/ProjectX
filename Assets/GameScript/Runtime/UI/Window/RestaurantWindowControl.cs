using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class RestaurantWindow : UIWindow
{
    private List<FoodOrderComponent> foodOrderList;
    public override void OnCreate()
    {
        foodOrderList = new List<FoodOrderComponent>(4);
        for (int i = 0; i < 4; i++)
        {
            var one = UIManager.Instance.CreateUIComponent<FoodOrderComponent>(null,Tran_OrderGroup,this);
            foodOrderList.Add(one);
        }
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
}