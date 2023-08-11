using System;
using System.Collections.Generic;
using cfg.food;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using YooAsset;

public class FryEnter : MonoBehaviour
{
    public Canvas TestCanvas;
    public Button TestBtn;
    public TMP_InputField MenuIdInput;
    public TMP_InputField QteGroupInput;
    public TMP_Dropdown DifficultyDropdown;


    public Toggle Salt;
    public Toggle Vinegar;
    public Toggle Sugar;
    public Toggle Spicy; 
    
    private RecipeDifficulty _currentRecipeDifficulty;
    private FryingFoodWindow _fryingFoodWindow;
    private CookWindowUI _curCookWindowUI;
    // private CompositeDisposable _handle;
    private FryModule _fryModule;
    private AssetOperationHandle _handler;
    private HashSet<int> _selectedQte;
    private void Awake()
    {
        // _handle = new CompositeDisposable();
        
    }

    private void Start()
    {
        var go = GameObject.Find("FryModule");
        _fryModule = go.GetComponent<FryModule>();
        _fryModule.IsDebugMode = true;
        _selectedQte = new(5);
        
        
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

        TestBtn.OnClickAsObservable().Subscribe(ClickTest);
    }

    private async void ClickTest(Unit param)
    {
        var menuId = Int32.Parse(MenuIdInput.text);
        var tbMenuInfo = DataProviderModule.Instance.GetMenuInfo(menuId);
        if (tbMenuInfo == null)
        {
            Debug.LogError($"menuId = {menuId} config is empty");
            return;
        }
        
        if (tbMenuInfo.MakeMethod != cookTools.Fry)
        {
            Debug.LogError($"菜谱id错误 不是烧烤");
            return;
        }

        //打开界面
        var openUITask = UIManager.Instance.OpenUI(UIEnum.FryingFoodWindow,null);
        await openUITask;
        _fryingFoodWindow = UIManager.Instance.Get(UIEnum.FryingFoodWindow) as FryingFoodWindow;
        _fryingFoodWindow.ClickStart = () =>
        {
            _fryModule.StartCook();
        };

        _fryingFoodWindow.ClickFinish = () =>
        {
            TestCanvas.gameObject.SetActive(true);
            UIManager.Instance.CloseUI(UIEnum.FryingFoodWindow);
        };
        
        //生成食物
        List<ItemTableData> cookItems = new(10);
        foreach (var one in tbMenuInfo.RelatedMaterial)
        {
            ItemTableData item = new();
            item.Id = one;
            item.Num = 5;
            cookItems.Add(item);
        }

        string filePath = "Assets/GameRes/SOConfigs/Menu/FryMenu/";
        Debug.Log($"DiffcultyDropdown.value = {DifficultyDropdown.value}");
        
        _handler?.Release();
        // _handle?.Dispose();

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
        _fryingFoodWindow.LoadQTEConfigTips(choiceQte);

        TestCanvas.gameObject.SetActive(false);
        
        PickFoodAndTools pat = new();
        pat.CookFoods = cookItems;
        pat.MenuId = menuId;
        pat.QTESets = _selectedQte;
        pat.QTEConfigs = choiceQte;
        
        _fryModule.Init(pat,_currentRecipeDifficulty);
        _fryingFoodWindow.SetDifficulty(_currentRecipeDifficulty);

        _fryModule.FinishCook += result =>
        {
            TestCanvas.gameObject.SetActive(true);
            _fryingFoodWindow.clickFinish(Unit.Default);
        };
        
        
    }

    // private void OnLoadUIComplete(IUIBase wnd)
    // {
    //     wnd.OnHide();
    //     _fryingFoodWindow = wnd as FryingFoodWindow;
    //     
    //     _fryingFoodWindow.ClickStart += () =>
    //     {
    //         _fryModule.StartCook();
    //     };
    //
    //     _fryingFoodWindow.ClickFinish += () =>
    //     {
    //         TestCanvas.gameObject.SetActive(true);
    //         UIManager.Instance.CloseUI(UIEnum.FryingFoodWindow);
    //     };
    // }
    
}
