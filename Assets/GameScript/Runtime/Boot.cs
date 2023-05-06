using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using YooAsset;

public class Boot : MonoBehaviour
{
    // Start is called before the first frame update
    // private bool initYooAsset = false;
    void Start()
    {
        StartCoroutine(GlobalFunctions.InitYooAssets(waitYooAssets));
    }

    void waitYooAssets(bool success)
    {
        // initYooAsset = success;
        UniModule.Initialize();
        
        UniModule.CreateModule<UIManager>();
        UniModule.CreateModule<Clocker>();
        UniModule.CreateModule<CharacterMgr>();
        
        UniModule.GetModule<UIManager>().OpenUI(UIEnum.BootWindow,null,null);
    }

    void ClickRestaurant(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/Restaurant.unity");
    }

    void ClickFry(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/FryingPan.unity");
    }

    void ClickBarbecue(Unit param)
    {
        YooAssets.LoadSceneAsync("Assets/GameRes/Scenes/Barbecue.unity");
    }
}
