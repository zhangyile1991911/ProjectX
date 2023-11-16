using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class NewsDetailWidget : UIComponent
{
    public NewsDetailWidget(GameObject go,UIWindow parent):base(go,parent)
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

    public void SetNewDetail(cfg.phone.AppNewsInfo newsDetailInfo)
    {
        Txt_Title.text = newsDetailInfo.Title;
        Txt_From.text = newsDetailInfo.From;
        Txt_Content.text = newsDetailInfo.Content;
        
        Txt_Title.ForceMeshUpdate();
        Debug.Log($"Txt_Title = {Txt_Title.rectTransform.sizeDelta}");
        Txt_From.ForceMeshUpdate();
        Debug.Log($"Txt_From = {Txt_From.rectTransform.sizeDelta}");
        Txt_Content.ForceMeshUpdate();
        Debug.Log($"Txt_Title = {Txt_Content.rectTransform.sizeDelta}");
        
    }
}