using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class ChatBubble : UIComponent
{
    private DOTweenAnimation _doTweenAnimation;
    private Character _owner;
    public ChatBubble(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        var uiManager = UniModule.GetModule<UIManager>();
        _doTweenAnimation = uiTran.GetComponent<DOTweenAnimation>();
    }
    
    public override void OnDestroy()
    {
        _doTweenAnimation.DOKill();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        //随机生成一个目的
        float x = Random.Range(-870, 870);
        float y = Random.Range(0, 440);
        // uiRectTran.anchoredPosition = new Vector2(x, y);
        uiRectTran.DOAnchorPos(new Vector2(x, y), 5.0f).OnComplete(()=>
        {
            _doTweenAnimation.DOPlay();
        });
        
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }
}