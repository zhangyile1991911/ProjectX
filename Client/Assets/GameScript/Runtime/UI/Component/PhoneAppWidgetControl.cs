using System;
using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

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
    
    // public void SetAPPInfo(string appName,Action click)
    // {
    //     Txt_App.text = appName;
    //     onClick = click;
    // }

    public void SetAPPInfo(int appId)
    {
        var tb = DataProviderModule.Instance.GetAppBaseInfo(appId);
        Txt_App.text = tb.Name;
        var handle = YooAssets.LoadAssetAsync<Sprite>(tb.AppIconRes);
        handle.Completed += (result) =>
        {
            Img_App.sprite = result.AssetObject as Sprite;
        };
    }
}