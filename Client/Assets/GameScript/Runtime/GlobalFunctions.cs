using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using UnityEngine;
using YooAsset;



public class OrderMealInfo
{
    public Character Customer;
    public int MealId;
}

public class FoodReceipt
{
    public int MenuId;
    public cfg.food.cookTools CookTools;
    public List<ItemDataDef> CookFoods;
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


