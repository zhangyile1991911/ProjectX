using System;
using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class PhoneAppWidget : UIComponent
{
    public PhoneAppWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        Btn_App.OnClickAsObservable().Subscribe(_ =>
        {
            onClick?.Invoke();
        }).AddTo(uiTran);
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

    private Action onClick;
    public void SetAPPInfo(string appName,Action click)
    {
        Txt_App.text = appName;
        onClick = click;
    }
}