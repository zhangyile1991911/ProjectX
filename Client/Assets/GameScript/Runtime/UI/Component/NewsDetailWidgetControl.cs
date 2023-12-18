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
    private float Inverval = 20;
    private List<NewsCommentWidget> commentWidgets;
    private RectTransform ContentRT;
    private RectTransform CommentRt;
    private RectTransform returnRt;
    private DOTweenAnimation _animations;
    public NewsDetailWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        
    }
    
    public override void OnCreate()
    {
        ContentRT = Tran_Content.GetComponent<RectTransform>();
        CommentRt = Tran_Coment.GetComponent<RectTransform>();
        returnRt = XBtn_Return.GetComponent<RectTransform>();
        commentWidgets = new List<NewsCommentWidget>(3);
        commentWidgets.Add(new NewsCommentWidget(Ins_CommentA.gameObject,this.ParentWindow));
        commentWidgets.Add(new NewsCommentWidget(Ins_CommentB.gameObject,this.ParentWindow));
        commentWidgets.Add(new NewsCommentWidget(Ins_CommentC.gameObject,this.ParentWindow));
        _animations = uiTran.GetComponent<DOTweenAnimation>();
        ScrollRect.OnValueChangedAsObservable().Select(_=>!GlobalFunctions.IsDebugMode).Subscribe(param =>
        {
            Clocker.Instance.AddSecond(2);
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
        ScrollRect.verticalNormalizedPosition = 1f;
    }

    public async void PlayOutAnimation()
    {
        _animations.DOPlayBackwards();
        await UniTask.Delay(800);
        OnHide();
    }

    public void SetNewDetail(cfg.phone.AppNewsInfo newsDetailInfo) 
    {
        // float totalHeight = 0;
        var h = refreshNewsContent(newsDetailInfo);
        // h = 0 - h;
        
        var seqRT = Tran_Sep.GetComponent<RectTransform>();
        seqRT.anchoredPosition = new Vector2(0, ContentRT.anchoredPosition.y - h - Inverval);
        
        var commentRT = Tran_Coment.GetComponent<RectTransform>();
        commentRT.anchoredPosition = new Vector2(0,ContentRT.anchoredPosition.y - h - Inverval * 2f);

        h = refreshNewsComment(newsDetailInfo.Comments);
        returnRt.anchoredPosition = new Vector2(returnRt.anchoredPosition.x, commentRT.anchoredPosition.y - h - Inverval*2);

        h = Math.Abs(returnRt.anchoredPosition.y) +64+Inverval*2;
        ScrollRect.content.sizeDelta = new Vector2(ScrollRect.content.sizeDelta.x,h);
        
        // ScrollRect
    }

    private float refreshNewsContent(cfg.phone.AppNewsInfo newsDetailInfo)
    {
        Txt_Title.text = newsDetailInfo.Title;
        Txt_From.text = newsDetailInfo.From;
        Txt_Content.text = newsDetailInfo.Content;
        
        float NewsContentHeight = 0;
        var pos = Txt_Title.rectTransform.anchoredPosition;
        var preferSize = Txt_Title.GetPreferredValues();
        NewsContentHeight += Math.Abs(preferSize.y);
       
        Txt_From.rectTransform.anchoredPosition = new Vector2(0,pos.y - preferSize.y - Inverval);
        preferSize = Txt_From.GetPreferredValues();
        NewsContentHeight += Math.Abs(preferSize.y);
        
        pos = Txt_From.rectTransform.anchoredPosition;
        Txt_Content.rectTransform.anchoredPosition = new Vector2(0, pos.y - preferSize.y - Inverval);
        preferSize = Txt_Content.GetPreferredValues();
        NewsContentHeight += Math.Abs(preferSize.y);

        NewsContentHeight += Inverval * 4;
       
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

        NewsCommentHeight += Inverval;
        for (int i = 0; i < newsDetailInfo.Count; i++)
        {
            commentWidgets[i].OnShow(null);
            commentWidgets[i].SetCommentInfo(newsDetailInfo[i]);
            NewsCommentHeight += Inverval + 117f;
        }

        CommentRt.sizeDelta = new Vector2(CommentRt.sizeDelta.x, NewsCommentHeight);
        return NewsCommentHeight;
    }
    
}