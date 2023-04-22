using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class ClockWidget : UIComponent
{
    public ClockWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        base.OnCreate();
        UniModule.GetModule<Clocker>().Topic.Subscribe(nowms =>
        {
            Txt_timer.text = nowms.ToString();
        }).AddTo(uiTran);
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
        base.OnUpdate();
    }
}