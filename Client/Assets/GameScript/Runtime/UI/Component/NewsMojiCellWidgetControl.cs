using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class NewsMojiCellWidget : UIComponent
{
    public NewsMojiCellWidget(GameObject go,UIWindow parent):base(go,parent)
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
    
    public void SetNewsDetailInfo(cfg.phone.AppNewsInfo newsData)
    {
        Txt_Title.text = newsData.Title;
        Txt_Follow.text = newsData.From;
    }
}