using System;
using System.Collections;
using System.Reflection.PortableExecutable;
using UnityEngine;
using YooAsset;


public class ChatMessage
{
    public int ChatId;
    public int CharacterId;
    public int ChatType;//1 正常对话 2 订单需求
    public string Title;
    public string ContentYarn;
}

public static class GlobalFunctions
{
    // public static Action<bool> YooAssetsComplete;
    public static IEnumerator InitYooAssets(Action<bool> complete)
    {
        //todo 这部分初始化 之后移动一个专门地方做
        YooAssets.Initialize();

        var package = YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(package);
        
        var initParameters = new EditorSimulateModeParameters();
        initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        
        // var initParameters = new OfflinePlayModeParameters();
        yield return package.InitializeAsync(initParameters);
        Debug.Log($"YooAssets初始化完成");
        complete?.Invoke(true);
    }
}


