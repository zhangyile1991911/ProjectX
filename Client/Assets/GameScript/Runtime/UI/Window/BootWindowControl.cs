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
        Btn_Phone.OnClickAsObservable()
            .Subscribe(ClickPhone).AddTo(handles);
        Btn_Dialgue.OnClickAsObservable()
            .Subscribe(ClickDialogue).AddTo(handles);
        Btn_steam.OnClickAsObservable().
            Subscribe(ClickSteam).AddTo(handles);
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
        var uiManager = UniModule.GetModule<UIManager>();
        //提前加载好窗口
        uiManager.LoadUI(UIEnum.CalenderWindow, (IUIBase) =>
        {
            var calenderWindow = IUIBase as CalenderWindow;
            calenderWindow.OnHide();
            calenderWindow.Refresh();
        });
        //uiManager.LoadUI(UIEnum.CalenderWindow,null,null);
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/Restaurant.unity");
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
    void ClickSteam(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/SteamedFood.unity");
        UIManager.Instance.DestroyUI(UIEnum.BootWindow);
    }

    void ClickPhone(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/Phone.unity");
        UIManager.Instance.DestroyUI(UIEnum.BootWindow);
    }

    void ClickDialogue(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/Story.unity");
        UIManager.Instance.DestroyUI(UIEnum.BootWindow);
    }
}