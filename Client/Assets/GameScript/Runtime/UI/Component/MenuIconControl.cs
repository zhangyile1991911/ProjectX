﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class MenuIcon : UIComponent
{
    public MenuIcon(GameObject go,UIWindow parent):base(go,parent)
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

    public void SetMenuInfo(cfg.ItemBaseInfo info)
    {
        uiGo.SetActive(true);
        ParentWindow.LoadSpriteAsync(info.UiResPath,Img_icon);
    }

    public void ClearMenuInfo()
    {
        Img_icon.sprite = null;
    }
}