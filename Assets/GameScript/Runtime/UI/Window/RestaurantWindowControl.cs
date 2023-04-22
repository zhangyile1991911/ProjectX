using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class RestaurantWindow : UIWindow
{
    private ClockWidget _clockWidget;
    private List<FoodOrderComponent> foodOrderList;
    public override void OnCreate()
    {
        base.OnCreate();
        // foodOrderList = new List<FoodOrderComponent>(4);
        // for (int i = 0; i < 4; i++)
        // {
        //     var one = UIManager.Instance.CreateUIComponent<FoodOrderComponent>(null,Tran_OrderGroup,this);
        //     foodOrderList.Add(one);
        // }
        _clockWidget = new ClockWidget(Ins_ClockWidget.gameObject,this);

       
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        
        Btn_Phone.OnClickAsObservable().Subscribe(_ =>
        {
            UniModule.GetModule<Clocker>().AddOneMinute();
        }).AddTo(handles);
        
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