using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;


public class OrderMealInfo
{
    public RestaurantCharacter Customer;
    public int MenuId;
    public int operation;// 0 添加 1 删除


    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        var tmp = obj as OrderMealInfo;
        var isSameMenu = tmp.MenuId == this.MenuId;
        var isSamePerson = tmp.Customer.CharacterId == this.Customer.CharacterId;
        return isSameMenu && isSamePerson;
    }
}

public enum Difficulty
{
    easy,
    normal,
    hard,
}
public class PickFoodAndTools
{
    public int MenuId;
    public cfg.food.cookTools CookTools;
    public List<ItemTableData> CookFoods;
    public HashSet<int> QTESets;
}

//炒完的菜
public class CookResult
{
    public int menuId;
    public float CompletePercent;
    public HashSet<cfg.food.flavorTag> Tags;
    public Dictionary<int, bool> QTEResult;//int = QTEId
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