using System;
using System.Collections;
using UnityEngine;
using YooAsset;

public static class GlobalFunctions
{
    // public static Action<bool> YooAssetsComplete;
    public static IEnumerator InitYooAssets(Action<bool> complete)
    {
        //todo 这部分初始化 之后移动一个专门地方做
        YooAssets.Initialize();
        var package = YooAssets.CreateAssetsPackage("DefaultPackage");
        YooAssets.SetDefaultAssetsPackage(package);
        var initParameters = new EditorSimulateModeParameters();
        initParameters.SimulatePatchManifestPath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        yield return package.InitializeAsync(initParameters);
        Debug.Log($"YooAssets初始化完成");
        complete?.Invoke(true);
    }
}
