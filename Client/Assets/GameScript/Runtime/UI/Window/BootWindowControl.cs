using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using UniRx;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class BootWindow : UIWindow
{
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
        Btn_Restaurant.OnClickAsObservable()
            .Subscribe(ClickRestaurant).AddTo(handles);
        Btn_FryFood.OnClickAsObservable()
            .Subscribe(ClickFry).AddTo(handles);
        Btn_Barbecue.OnClickAsObservable()
            .Subscribe(ClickBarbecue).AddTo(handles);    
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    
    void ClickRestaurant(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/Restaurant.unity");
        var uiManager = UniModule.GetModule<UIManager>();
        uiManager.DestroyUI(UIEnum.BootWindow);
    }

    void ClickFry(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/FryingPan.unity");
        UIManager.Instance.DestroyUI(UIEnum.BootWindow);
    }

    void ClickBarbecue(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/Barbecue.unity");
        UIManager.Instance.DestroyUI(UIEnum.BootWindow);
    }
}