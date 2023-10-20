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
    public int MenuId;
}

public class TipCommonData : UIOpenParam
{
    public string tipstr;
}



public class OrderMealInfo
{
    // public RestaurantCharacter Customer;
    public int CharacterId;
    public int MenuId;
    public cfg.common.bubbleType OrderType;
    // public int operation;// 0 添加 1 删除
    public HashSet<flavorTag> flavor;


    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        var tmp = obj as OrderMealInfo;
        // var isSameMenu = tmp.MenuId == this.MenuId;
        // var isSamePerson = tmp.CharacterId == this.CharacterId;
        return tmp.CharacterId == this.CharacterId;
    }
}

public class CharacterSaidInfo
{
    public int CharacterId;
    public int ChatId;
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

    private static RecipeTrie OwnRecipe;

    public static void InitRecipe(List<OwnMenu> recipeIds)
    {
        if (OwnRecipe != null) return;
        OwnRecipe = new RecipeTrie();
        foreach (var menu in recipeIds)
        {
            var tb = DataProviderModule.Instance.GetMenuInfo(menu.MenuId);
            if (tb == null)
            {
                Debug.LogError($"menuid = {menu.MenuId} is null");
                continue;
            }
            OwnRecipe.Insert(tb.RelatedMaterial,menu.MenuId);
        }
    }

    public static HashSet<int> SearchRecipe(List<int> foods)
    {
        return OwnRecipe.StartsWith(foods);
    }

    public static void InsertRecipe(int recipeId)
    {
        var tb = DataProviderModule.Instance.GetMenuInfo(recipeId);
        if (tb != null)
        {
            OwnRecipe.Insert(tb.RelatedMaterial,recipeId);    
        }
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

public static class GlobalContent
{
    public static string OrderTicketContent = "";
}