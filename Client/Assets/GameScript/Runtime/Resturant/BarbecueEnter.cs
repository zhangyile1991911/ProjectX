using System;
using System.Collections.Generic;
using cfg.food;
using GameScript.CookPlay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using UniRx;
using UnityEngine.Pool;

public class BarbecueEnter : MonoBehaviour
{
    public Canvas TestCanvas;
    public Button testBtn;
    public TMP_InputField MenuIdInput;
    public TMP_InputField QteGroupInput;
    public TMP_Dropdown DifficultyDropdown;
    
    public Toggle Salt;
    public Toggle Vinegar;
    public Toggle Sugar;
    public Toggle Spicy; 
    
    private RecipeDifficulty _currentRecipeDifficulty;
    private BarbecueModule module;
    private AssetOperationHandle _handler;
    private HashSet<int> _selectedQte;
    private BarbecueWindow _barbecueWindow;
    
    private void Start()
    {
        var go = GameObject.Find("BarbecueModule");
        module = go.GetComponent<BarbecueModule>();
        _selectedQte = new HashSet<int>(5);
        
        
        Salt.OnValueChangedAsObservable().Skip(1).Subscribe(b=>
        {
            if (b)
                _selectedQte.Add(1);
            else
                _selectedQte.Remove(1);
        });
        
        Vinegar.OnValueChangedAsObservable().Skip(1).Subscribe(b =>
        {
            if(b)
                _selectedQte.Add(2);
            else
                _selectedQte.Remove(2);
        });

        Sugar.OnValueChangedAsObservable().Skip(1).Subscribe(b =>
        {
            if(b)
                _selectedQte.Add(3);
            else
                _selectedQte.Remove(3);
        });
        
        Spicy.OnValueChangedAsObservable().Skip(1).Subscribe(b =>
        {
            if (b)
                _selectedQte.Add(4);
            else
                _selectedQte.Remove(4);
        });

        testBtn.OnClickAsObservable().Subscribe(ClickTest);
        
    }

    private async void ClickTest(Unit param)
    {
        //加载配置
        var menuId = Int32.Parse(MenuIdInput.text);
        var tbMenuInfo = DataProviderModule.Instance.GetMenuInfo(menuId);
        if (tbMenuInfo == null)
        {
            Debug.LogError($"menuId = {menuId} config is empty");
            return;
        }

        if (tbMenuInfo.MakeMethod != cookTools.Barbecue)
        {
            Debug.LogError($"菜谱id错误 不是烧烤");
            return;
        }
        
        string filePath = "Assets/GameRes/SOConfigs/Menu/BarbecueMenu/";
        
        switch (DifficultyDropdown.value)
        {
            case 0:
                _handler = YooAssets.LoadAssetSync<RecipeDifficulty>(filePath+"Low.asset");
                break;
            case 1:
                _handler = YooAssets.LoadAssetSync<RecipeDifficulty>(filePath+"Middle.asset");
                break;
            case 2:
                _handler = YooAssets.LoadAssetSync<RecipeDifficulty>(filePath+"High.asset");
                break;
        }
        
        _currentRecipeDifficulty = _handler.AssetObject as RecipeDifficulty;

        
        var qteIdGroupId = Int32.Parse(QteGroupInput.text);
        List<qte_info> choiceQte = new List<qte_info>();
        var qteTemplate = DataProviderModule.Instance.GetQTEGroupInfo(qteIdGroupId);
        for (int i = 0; i < qteTemplate.Count; i++)
        {
            if (_selectedQte.Contains(qteTemplate[i].QteId))
            {
                choiceQte.Add(qteTemplate[i]);
            }
        }
        
        //打开玩法UI界面
        var openUITask = UIManager.Instance.OpenUI(UIEnum.BarbecueWindow, null);
        await openUITask;

        _barbecueWindow = UIManager.Instance.Get(UIEnum.BarbecueWindow) as BarbecueWindow;
        _barbecueWindow.ClickStart = () =>
        {
            module.StartCook();
        };
        _barbecueWindow.ClickFinish = () =>
        {
            _barbecueWindow.OnHide();
            TestCanvas.gameObject.SetActive(true);
        };
        _barbecueWindow.LoadQTEConfigTips(choiceQte);
        
        module.FinishCook += result =>
        {
            _barbecueWindow.ShowGameOver(result);
        };
        

        // var cookItems = new List<ItemTableData>(5);
        var cookItems = ListPool<ItemTableData>.Get();
        foreach (var one in tbMenuInfo.RelatedMaterial)
        {
            var foodItem = new ItemTableData();
            foodItem.Id = one;
            foodItem.Num = 2;
            cookItems.Add(foodItem);
        }
        
        PickFoodAndTools pat = new();
        pat.CookFoods = cookItems;
        pat.MenuId = menuId;
        pat.QTESets = _selectedQte;
        pat.QTEConfigs = choiceQte;
        
        module.Init(pat,_currentRecipeDifficulty);
        _barbecueWindow.SetDifficulty(_currentRecipeDifficulty);
        
        TestCanvas.gameObject.SetActive(false);
    }
    
}
