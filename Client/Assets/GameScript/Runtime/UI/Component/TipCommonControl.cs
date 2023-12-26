using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class TipCommon : UIComponent
{
    public TipCommon(GameObject go,UIWindow parent):base(go,parent)
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
        var tmp = openParam as TipCommonData;
        Txt_tip.text = tmp.tipstr;
        uiRectTran.DOLocalMoveY(200, 1.1f).OnComplete(() =>
        {
            //todo 改成用对象池
            UIManager.Instance.DestroyUIComponent(this);
        });
        Txt_tip.DOFade(0, 1f);
        // var doTweenAnimation = uiGo.GetComponent<DOTweenAnimation>();
        // // doTweenAnimation.onComplete.AddListener(()=>{UIManager.Instance.DestroyUIComponent(this);});
        // doTweenAnimation.DOPlay();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }
}