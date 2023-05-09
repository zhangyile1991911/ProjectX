using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Transactions;
using UniRx;
using Random = UnityEngine.Random;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class ChatBubble : UIComponent
{
    public Character Owner => _owner;
    public int ChatId => _chatId;
    private DOTweenAnimation _doTweenAnimation;
    private Character _owner;
    private int _chatId;
    private Action<ChatBubble> _click;
    private Button _btn;
    private Tweener _tweener;
    public ChatBubble(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        var uiManager = UniModule.GetModule<UIManager>();
        _doTweenAnimation = uiTran.GetComponent<DOTweenAnimation>();
        _btn = uiTran.GetComponent<Button>();
        _btn.OnClickAsObservable().Subscribe(onBubbleClick).AddTo(uiTran);
    }

    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
    }

    public override void OnHide()
    {
        base.OnHide();
    }
    
    public override void OnDestroy()
    {
        _doTweenAnimation.DOKill();
    }

    public override void OnUpdate()
    {
        
    }

    private void onBubbleClick(Unit param)
    {
        if (!_tweener.IsComplete())
        {
            _tweener.Pause();
        }
        _click?.Invoke(this);
    }
    
    public void SetBubbleInfo(int chatId,Character origin,Action<ChatBubble> click)
    {
        //随机生成一个目的
        float x = Random.Range(-870, 870);
        float y = Random.Range(0, 440);
        _owner = origin;
        _chatId = chatId;
        _click = click;
        Txt_content.text = "hello";
        
        _tweener = uiRectTran.DOAnchorPos(new Vector2(x, y), 5.0f).OnComplete(()=>
        {
            _doTweenAnimation.DOPlay();
        });
    }
}