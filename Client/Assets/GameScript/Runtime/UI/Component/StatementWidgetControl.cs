﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class StatementWidget : UIComponent
{
    public StatementWidget(GameObject go,UIWindow parent):base(go,parent)
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

    public void SetStatementInfo(int id,int num)
    {
        var tb = DataProviderModule.Instance.GetItemBaseInfo(id);
        var total = tb.Sell * num;
        
        Txt_Name.text = tb.Name;
        Txt_Num.text = total.ToString();
        
        ParentWindow.LoadSpriteAsync(tb.UiResPath,Img_Icon);
    }
}