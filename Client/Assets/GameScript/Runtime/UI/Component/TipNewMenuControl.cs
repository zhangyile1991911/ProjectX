using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class TipNewMenu : UIComponent
{
    public TipNewMenu(GameObject go,UIWindow parent):base(go,parent)
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
        var tbMenu = DataProviderModule.Instance.GetMenuInfo(tmp.MenuId);
        string tipsContent = ZString.Format("获得新菜谱{0}",tbMenu.Name);
        Txt_Desc.text = tipsContent;
        
        var startPos = Vector2.zero;
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