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
        UniModule.CreateModule<DataProviderModule>();
        UniModule.CreateModule<EventModule>();
        UniModule.CreateModule<UserInfoModule>();
        UniModule.CreateModule<CharacterMgr>();
        UniModule.CreateModule<Clocker>();
        // UniModule.CreateModule<DialogueModule>();
        
        UniModule.GetModule<UIManager>().OpenUI(UIEnum.BootWindow,null,null);
    }
    
}
