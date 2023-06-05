using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class MenuIcon : UIComponent
{
    private Action<int> click;
    private int menuId;
    public MenuIcon(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        Btn_icon.OnClickAsObservable().Subscribe(_ =>
        {
            click?.Invoke(menuId);
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

    public void SetMenuInfo(cfg.ItemBaseInfo info,Action<int> cb)
    {
        uiGo.SetActive(true);
        menuId = info.Id;
        click = cb;
        ParentWindow.LoadSpriteAsync(info.UiResPath,Img_icon);
    }

    public void ClearMenuInfo()
    {
        Img_icon.sprite = null;
    }
}