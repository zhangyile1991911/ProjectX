using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class TipAddFriendValue : UIComponent
{
    public TipAddFriendValue(GameObject go,UIWindow parent):base(go,parent)
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
        var tmp = openParam as DialogueData;
        var startPos = UIManager.Instance.WorldPositionToUI(tmp.Character.EmojiNode.position);
        uiRectTran.anchoredPosition = startPos;
        var doTweenAnimation = uiGo.GetComponent<DOTweenAnimation>();
        doTweenAnimation.DOPlay();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }
}