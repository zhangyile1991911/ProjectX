using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
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
        var clocker = UniModule.GetModule<Clocker>();
        Txt_timer.text = ZString.Format("{0}:{1:D2}",clocker.NowDateTime.Hour,clocker.NowDateTime.Minute);
        clocker.Topic.Subscribe(nowms =>
        {
            Txt_timer.text = ZString.Format("{0}:{1:D2}",nowms.Hour,nowms.Minute);
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