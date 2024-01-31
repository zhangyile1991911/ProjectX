using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class NewsDetailWidget : UIComponent
{
    private float Inverval = 16f;
    private List<NewsCommentWidget> commentWidgets;
    private RectTransform ContentRT;
    private RectTransform CommentRt;
    private DOTweenAnimation _animations;
    public NewsDetailWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        
    }
    
    public override void OnCreate()
    {
        ContentRT = Txt_Content.GetComponent<RectTransform>();
        CommentRt = Tran_Coment.GetComponent<RectTransform>();
        
        commentWidgets = new List<NewsCommentWidget>(3);
        commentWidgets.Add(new NewsCommentWidget(Ins_CommentA.gameObject,ParentWindow));
        commentWidgets.Add(new NewsCommentWidget(Ins_CommentB.gameObject,ParentWindow));
        commentWidgets.Add(new NewsCommentWidget(Ins_CommentC.gameObject,ParentWindow));
        _animations = uiTran.GetComponent<DOTweenAnimation>();
        ContentScrollRect.OnValueChangedAsObservable().Select(_=>!GlobalFunctions.IsDebugMode).Subscribe(param =>
        {
            Clocker.Instance.AddSecond(1);
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

    public void PlayInAnimation()
    {
        _animations.DOPlayForward();
        ContentScrollRect.verticalNormalizedPosition = 1f;
    }

    public async void PlayOutAnimation()
    {
        _animations.DOPlayBackwards();
        await UniTask.Delay(800);
        OnHide();
    }

    public void SetNewDetail(cfg.phone.AppNewsInfo newsDetailInfo)
    {
        var h = refreshNewsTitle(newsDetailInfo);
        var contentHeight = refreshNewsContent(newsDetailInfo);
        h += contentHeight;
        CommentRt.anchoredPosition = new Vector2(0,ContentRT.anchoredPosition.y - contentHeight - Inverval);
        
        h += refreshNewsComment(newsDetailInfo.Comments);

        ContentScrollRect.content.sizeDelta = new Vector2(ContentScrollRect.content.sizeDelta.x,h);
        
        // ScrollRect
    }

    private float refreshNewsTitle(cfg.phone.AppNewsInfo newsDetailInfo)
    {
        Txt_Title.text = newsDetailInfo.Title;
        Txt_author.text = newsDetailInfo.From;
        return 200f;
    }

    private float refreshNewsContent(cfg.phone.AppNewsInfo newsDetailInfo)
    {
        Txt_Content.text = newsDetailInfo.Content;
        
        // float NewsContentHeight = 0;
        // var pos = Txt_Title.rectTransform.anchoredPosition;
        // var preferSize = Txt_Title.GetPreferredValues();
        // NewsContentHeight += Math.Abs(preferSize.y);
        //
        // Txt_From.rectTransform.anchoredPosition = new Vector2(0,pos.y - preferSize.y - Inverval);
        // preferSize = Txt_From.GetPreferredValues();
        // NewsContentHeight += Math.Abs(preferSize.y);
        //
        // pos = Txt_From.rectTransform.anchoredPosition;
        // Txt_Content.rectTransform.anchoredPosition = new Vector2(0, pos.y - preferSize.y - Inverval);
        Vector2 preferSize = Txt_Content.GetPreferredValues();
        float NewsContentHeight = 0;
        NewsContentHeight += Math.Abs(preferSize.y);
        NewsContentHeight += Inverval;
       
        ContentRT.sizeDelta = new Vector2(ContentRT.sizeDelta.x,NewsContentHeight);
        Debug.Log($"rt.rect = {ContentRT.rect}  totalHeight = {NewsContentHeight}");
        return NewsContentHeight;
    }

    private float refreshNewsComment(List<cfg.phone.news_comment> newsDetailInfo)
    {
        float NewsCommentHeight = 0f;
        foreach (var one in commentWidgets)
        {
            one.OnHide();
        }

        float newY = 0f;
        // NewsCommentHeight += Inverval;
        for (int i = 0; i < newsDetailInfo.Count; i++)
        {
            commentWidgets[i].OnShow(null);
            float curHeight = commentWidgets[i].SetCommentInfo(newsDetailInfo[i]);
            float oldX = commentWidgets[i].uiRectTran.anchoredPosition.x;
            commentWidgets[i].uiRectTran.anchoredPosition = new Vector2(oldX, newY);
            newY = newY - curHeight - Inverval;
            NewsCommentHeight += curHeight + Inverval;
        }

        NewsCommentHeight -= Inverval;
        CommentRt.sizeDelta = new Vector2(CommentRt.sizeDelta.x, Mathf.Abs(NewsCommentHeight));
        return NewsCommentHeight;
    }
    
}