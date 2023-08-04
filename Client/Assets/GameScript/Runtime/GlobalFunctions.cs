using System;
using System.Collections;
using System.Collections.Generic;
using cfg.food;
using UnityEngine;
using YooAsset;


public class CookWindowParamData:UIOpenParam
{
    // public StateMachine StateMachine;
    public RecipeDifficulty Difficulty;
}

public class FlowControlWindowData : UIOpenParam
{
    public StateMachine StateMachine;
}

public class DialogueData:UIOpenParam
{
    public RestaurantRoleBase Character;
    public int FriendValue;
}

public class OrderMealInfo
{
    // public RestaurantCharacter Customer;
    public int CharacterId;
    public int MenuId;
    public int operation;// 0 添加 1 删除


    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        var tmp = obj as OrderMealInfo;
        // var isSameMenu = tmp.MenuId == this.MenuId;
        // var isSamePerson = tmp.CharacterId == this.CharacterId;
        return tmp.CharacterId == this.CharacterId;
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
    public List<qte_info> QTEConfigs;

}

//炒完的菜


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

public class QTEInfoRecord
{
    public int Id;
    public QTETips tip;
}

public interface CookWindowUI
{
    public void ShowGameOver(CookResult cookResult);
    public void LoadQTEConfigTips(List<qte_info> tbQTEInfos);
    public void ShowQTETip(int qteId);
    public void HideQTETip(int qteId);

    public void SetDifficulty(RecipeDifficulty difficulty);
}