using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using cfg.food;
using UniRx;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class SteamFoodWindow : UIWindow,CookWindowUI
{
    public Action ClickStart;
    public Action ClickFinish;
    public override void OnCreate()
    {
        base.OnCreate();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);

        XBtn_Start.OnClick.Subscribe((param) =>
        {
            ClickStart?.Invoke();
        }).AddTo(handles);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    
    public void ShowGameOver(CookResult cookResult)
    {
        
    }

    public void LoadQTEConfigTips(List<qte_info> tbQTEInfos)
    {
        
    }

    public void ShowQTETip(int qteId)
    {
        
    }

    public void HideQTETip(int qteId)
    {
        
    }

    public void SetDifficulty(RecipeDifficulty difficulty)
    {
        
    }
}