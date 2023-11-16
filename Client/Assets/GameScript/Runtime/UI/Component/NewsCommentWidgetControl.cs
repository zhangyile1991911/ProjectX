using System.Collections;
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

    public void SetCommentInfo(cfg.phone.news_comment comment)
    {
        Txt_Name.text = comment.NickName;
        Txt_Comment.text = comment.Comment;
        Txt_like.gameObject.SetActive(comment.Like > 0);
        Txt_like.text = comment.Like.ToString();
        Txt_dislike.gameObject.SetActive(comment.Like > 0);
        Txt_dislike.text = comment.Unlike.ToString();
    }
    
}