﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class NewsCommentWidget : UIComponent
{
    public NewsCommentWidget(GameObject go,UIWindow parent):base(go,parent)
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

    public float SetCommentInfo(cfg.phone.news_comment comment)
    {
        Txt_Name.text = comment.NickName;
        Txt_Comment.text = comment.Comment;
        float preferHeight = Txt_Comment.GetPreferredValues().y;

        //重新计算组件高度
        float height = Mathf.Abs(RT_Comment.anchoredPosition.y) + preferHeight + 16f;
        float oldWidth = uiRectTran.sizeDelta.x;
        uiRectTran.sizeDelta = new Vector2(oldWidth,height);
        
        return uiRectTran.sizeDelta.y;
    }
    
}