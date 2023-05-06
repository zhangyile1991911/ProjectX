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
    private List<ChatBubble> _bubbleList;
    private StateMachine _machine;
    public override void OnCreate()
    {
        base.OnCreate();
        // foodOrderList = new List<FoodOrderComponent>(4);
        // for (int i = 0; i < 4; i++)
        // {
        //     var one = UIManager.Instance.CreateUIComponent<FoodOrderComponent>(null,Tran_OrderGroup,this);
        //     foodOrderList.Add(one);
        // }
        _bubbleList = new List<ChatBubble>(20);
        _clockWidget = new ClockWidget(Ins_ClockWidget.gameObject,this);

    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        Btn_Phone.OnClickAsObservable().Subscribe(ClickPhone).AddTo(handles);
        Btn_Bubble.OnClickAsObservable().Subscribe(ClickTest).AddTo(handles);

    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void ClickPhone(Unit param)
    {
        UniModule.GetModule<Clocker>().AddOneMinute();
        var uiManager = UniModule.GetModule<UIManager>();
        var phone = uiManager.Get(UIEnum.PhoneWindow);
        if (phone == null)
        {
            uiManager.OpenUI(UIEnum.PhoneWindow,null,null,UILayer.Center);   
        }
        else if (phone.IsActive)
        {
            uiManager.CloseUI(UIEnum.PhoneWindow);    
        }
        else
        {
            uiManager.OpenUI(UIEnum.PhoneWindow,null,null,UILayer.Center);    
        }
    }

    private async void ClickTest(Unit param)
    {
        var uiManager = UniModule.GetModule<UIManager>();
        var bubble = await uiManager.CreateUIComponent<ChatBubble>(null,uiTran,this);
        _bubbleList.Add(bubble);
    }
}