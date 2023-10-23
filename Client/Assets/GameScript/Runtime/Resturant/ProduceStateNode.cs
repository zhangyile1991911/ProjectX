using System;
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
    private GameObject _toolNode;
    private CompositeDisposable _handle;
    private Camera _mainCamera;
    private cookTools _curCookTools;
    private UIEnum _curUIEnum;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handle = new CompositeDisposable();
    }

    public void OnEnter(object param)
    {
        _data = param as PickFoodAndTools;
        var menuTb = DataProviderModule.Instance.GetMenuInfo(_data.MenuId);
        
        _curUIEnum = (UIEnum)Enum.Parse(typeof(UIEnum), menuTb.UiName);
        
        _mainCamera = _restaurant.MainCamera;
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Cook);
        _restaurant.ShowKitchen();
        loadQTEConfig();
        loadDifficultyConfig(_data.MenuId);
        loadCookTools(menuTb.MakeMethod);
        
        UIManager.Instance.OpenUI(_curUIEnum,OnLoadUIComplete,null,UILayer.Bottom);
        
        // _curCookModule.FinishCook += result =>
        // {
        //     foreach (var one in _data.CookFoods)
        //     {
        //         UserInfoModule.Instance.SubItemNum(one.Id,(uint)one.Num);    
        //     }
        //     _curCookWindowUI.ShowGameOver(result);
        //     UserInfoModule.Instance.AddCookedMeal(result);
        // };
        //
        // _curCookModule.Init(_data,_currentRecipeDifficulty);

        //_restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Kitchen);
    }
    
    public void OnExit()
    {
        UIManager.Instance.CloseUI(_curUIEnum);
        _restaurant.HideKitchen();
        
        // _handle.Clear();
        //
        // switch (_data.CookTools)
        // {
        //     case cookTools.Fry:
        //         UIManager.Instance.CloseUI(UIEnum.FryingFoodWindow);
        //         _restaurant.HideCookGamePrefab(_data.CookTools);
        //         break;
        //     case cookTools.Barbecue:
        //         UIManager.Instance.CloseUI(UIEnum.BarbecueWindow);
        //         _restaurant.HideCookGamePrefab(_data.CookTools);
        //         break;
        //     default:
        //         Debug.LogError("其他玩法暂时没有实现");
        //         break;
        // }
        // _data = null;
        
    }
    
    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&!_curCookModule.IsCooking)
        {
            _machine.ChangeState<PrepareStateNode>();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 执行射线检测
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider == null )return;
            if(!hit.collider.CompareTag("FlavourObject"))return;
            foreach (var one in _restaurant.Flavors)
            {
                if (one.gameObject == hit.transform.gameObject)
                {
                    if (one.IsEnable)
                    {
                        one.DisableTag();
                    }
                    else
                    {
                        one.EnableTag(); 
                    }
                    break;
                }
            }
        }
    }

    private void OnLoadUIComplete(IUIBase wnd)
    {
        var tb = DataProviderModule.Instance.GetMenuInfo(_data.MenuId);
        switch (tb.MakeMethod)
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
        
        _curCookWindowUI.SetDifficulty(_currentRecipeDifficulty);
        _curCookWindowUI.LoadQTEConfigTips(_data.QTEConfigs);
    }

    private async void ClickStartCook()
    {
        var duration = 1.0f;
        //将食材渐隐消失
        _restaurant.FoodDoDisappear(duration);
        await UniTask.Delay(TimeSpan.FromSeconds(duration+0.2f));
        //加载锅里食物
        _curCookModule.Init(_data,_currentRecipeDifficulty);
        //开始烹饪游戏
        _curCookModule.StartCook();
    }

    private void ClickFinishCook()
    {
        _machine.ChangeState<PrepareStateNode>();
    }

    private async void loadCookTools(cookTools tool)
    {
        switch (tool)
        {
            case cookTools.Fry:
                _toolNode = await _restaurant.ShowCookGamePrefab(tool);
                _curCookModule = _toolNode.GetComponent<FryModule>();        
                break;
            case cookTools.Barbecue:
                _toolNode = await _restaurant.ShowCookGamePrefab(tool);
                _curCookModule = _toolNode.GetComponent<BarbecueModule>();        
                break;
            case cookTools.Steam:
                break;
        }
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

    private void loadQTEConfig()
    {
        var groupId = DataProviderModule.Instance.GetQTEGroupId();
        _data.QTEConfigs = new List<qte_info>();
        var tmpTb = DataProviderModule.Instance.GetQTEGroupInfo(groupId);
        for (int i = 0; i < tmpTb.Count; i++)
        {
            if (_data.QTESets.Contains(tmpTb[i].QteId))
            {
                _data.QTEConfigs.Add(tmpTb[i]);
            }
        }
    }
}
