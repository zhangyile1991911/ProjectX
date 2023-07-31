using System.Collections;
using System.Collections.Generic;
using cfg.food;
using Cysharp.Threading.Tasks;
using GameScript.CookPlay;
using UniRx;
using UnityEngine;
using YooAsset;

public class ProduceStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private PickFoodAndTools _data;
    private RecipeDifficulty _currentRecipeDifficulty;
    private CookModule _curCookModule;
    private CookWindowUI _curCookWindowUI;
    private CompositeDisposable _handle;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handle = new CompositeDisposable();
    }

    public async void OnEnter(object param)
    {
        _data = param as PickFoodAndTools;
        // var menuTb = DataProviderModule.Instance.GetMenuInfo(_data.MenuId);
        GameObject go = null;
        
        loadDifficultyConfig(_data.MenuId);
        
        var openWindowParam = new CookWindowParamData();
        // openWindowParam.StateMachine = _machine;
        openWindowParam.Difficulty = _currentRecipeDifficulty;
        
        switch (_data.CookTools)
        {
            case cookTools.Cook:
                UIManager.Instance.OpenUI(UIEnum.FryingFoodWindow,OnLoadUIComplete,openWindowParam,UILayer.Bottom);
                go = await _restaurant.ShowCookGamePrefab(_data.CookTools);
                _curCookModule = go.GetComponent<FryModule>();
                break;
            case cookTools.Barbecue:
                UIManager.Instance.OpenUI(UIEnum.BarbecueWindow, OnLoadUIComplete,openWindowParam);
                go = await _restaurant.ShowCookGamePrefab(_data.CookTools);
                _curCookModule = go.GetComponent<BarbecueModule>();
                break;
            default:
                Debug.LogError("其他玩法暂时没有实现");
                break;
        }
        
        _curCookModule.FinishCook += result =>
        {
            foreach (var one in _data.CookFoods)
            {
                UserInfoModule.Instance.SubItemNum(one.Id,(uint)one.Num);    
            }
            _curCookWindowUI.ShowGameOver(result);
            UserInfoModule.Instance.AddCookedMeal(result);
        };
        
        _curCookModule.Init(_data,_currentRecipeDifficulty);
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Kitchen);
    }
    
    public void OnExit()
    {
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.RestaurantMain);
        _handle.Clear();
        
        switch (_data.CookTools)
        {
            case cookTools.Cook:
                UIManager.Instance.CloseUI(UIEnum.FryingFoodWindow);
                _restaurant.HideCookGamePrefab(_data.CookTools);
                break;
            case cookTools.Barbecue:
                UIManager.Instance.CloseUI(UIEnum.BarbecueWindow);
                _restaurant.HideCookGamePrefab(_data.CookTools);
                break;
            default:
                Debug.LogError("其他玩法暂时没有实现");
                break;
        }
        _data = null;
        
    }
    
    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&!_curCookModule.IsCooking)
        {
            _machine.ChangeState<PrepareStateNode>();
        }
    }

    private void OnLoadUIComplete(IUIBase wnd)
    {
        switch (_data.CookTools)
        {
            case cookTools.Fry:
                var fryingWnd = wnd as FryingFoodWindow;
                fryingWnd.ClickStart = ClickStartCook;
                fryingWnd.ClickFinish = ClickFinishCook;
                _curCookWindowUI = fryingWnd;
                break;
            case cookTools.Barbecue:
                var barbecueWnd = wnd as BarbecueWindow;
                barbecueWnd.ClickStart = ClickStartCook;
                barbecueWnd.ClickFinish = ClickFinishCook;
                _curCookWindowUI = barbecueWnd;
                break;
        }
        
        var groupId = DataProviderModule.Instance.GetQTEGroupId();
        Debug.Log($"loadQTEConfig = {groupId}");   
        var _tbQteInfos = new List<qte_info>(10);
        _tbQteInfos.Clear();
        
        var tmpTb = DataProviderModule.Instance.GetQTEGroupInfo(groupId);
        for (int i = 0; i < tmpTb.Count; i++)
        {
            if (_data.QTESets.Contains(tmpTb[i].QteId))
            {
                _tbQteInfos.Add(tmpTb[i]);
            }
        }
        _curCookWindowUI.LoadQTEConfigTips(_tbQteInfos);
    }

    private void ClickStartCook()
    {
        _curCookModule.StartCook();
    }

    private void ClickFinishCook()
    {
        _machine.ChangeState<PrepareStateNode>();
    }

    private void loadDifficultyConfig(int menuId)
    {
        var tbMenuInfo = DataProviderModule.Instance.GetMenuInfo(menuId);
        if (tbMenuInfo == null)
        {
            Debug.LogError($"MenuId {menuId} == null");
        }

        string filePath = "Assets/GameRes/SOConfigs/Menu/";
        AssetOperationHandle handler = null;
        switch (tbMenuInfo.MakeMethod)
        {
            case cookTools.Fry:
                filePath += "FryMenu/";
                break;
            case cookTools.Barbecue:
                filePath += "BarbecueMenu/";
                break;
        }
        
        switch (tbMenuInfo.Difficulty)
        {
            case cookDifficulty.easy:
                handler = YooAssets.LoadAssetSync<RecipeDifficulty>(filePath+"Low.asset");
                break;
            case cookDifficulty.normal:
                handler = YooAssets.LoadAssetSync<RecipeDifficulty>(filePath+"Middle.asset");
                break;
            case cookDifficulty.hard:
                handler = YooAssets.LoadAssetSync<RecipeDifficulty>(filePath+"High.asset");
                break;
            default:
                Debug.LogError($"handler == null");
                break;
        }
        _currentRecipeDifficulty = handler.AssetObject as RecipeDifficulty;
    }
}
