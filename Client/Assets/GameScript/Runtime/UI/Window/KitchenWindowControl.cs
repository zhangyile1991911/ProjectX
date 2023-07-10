using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.food;
using SuperScrollView;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class KitchenWindow : UIWindow
{
    private cfg.food.cookTools _choicedTools;
    private List<ItemTableData> _ownedFoodItems;
    private List<ItemTableData> _showFoodItems;
    private List<FoodBtnCell> _cacheCells;
    private cfg.food.materialType _foodMaterialType = materialType.Other;

    private int _curSelectMenuId;
    private List<FoodIcon> _choicedFoodIcons;
    private List<MenuIcon> _canMakeFoodIcons;
    private List<CookToolIcon> _cookToolIcons;
    private HashSet<flavorTag> _oppositeTags;
    private DataProviderModule _dataProvider;
    private HashSet<int> _selectedQte;
    public override void OnCreate()
    {
        base.OnCreate();
        Grid_FoodItem.InitGridView(0,getFoodItem);
        
        _cacheCells = new List<FoodBtnCell>(10);
        _showFoodItems = new List<ItemTableData>(10);
        _oppositeTags = new HashSet<flavorTag>(10);

        _choicedFoodIcons = new List<FoodIcon>(5);
        _choicedFoodIcons.Add(new FoodIcon(Ins_FoodIconA.gameObject,this));
        _choicedFoodIcons.Add(new FoodIcon(Ins_FoodIconB.gameObject,this));
        _choicedFoodIcons.Add(new FoodIcon(Ins_FoodIconC.gameObject,this));
        _choicedFoodIcons.Add(new FoodIcon(Ins_FoodIconD.gameObject,this));
        _choicedFoodIcons.Add(new FoodIcon(Ins_FoodIconE.gameObject,this));

        _canMakeFoodIcons = new List<MenuIcon>(8);
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon1.gameObject,this));
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon2.gameObject,this));
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon3.gameObject,this));
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon4.gameObject,this));
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon5.gameObject,this));
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon6.gameObject,this));
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon7.gameObject,this));
        _canMakeFoodIcons.Add(new MenuIcon(Ins_MenuIcon8.gameObject,this));

        _cookToolIcons = new List<CookToolIcon>(5);
        _cookToolIcons.Add(new CookToolIcon(Ins_CookToolIconA.gameObject,this));
        _cookToolIcons.Add(new CookToolIcon(Ins_CookToolIconB.gameObject,this));
        _cookToolIcons.Add(new CookToolIcon(Ins_CookToolIconC.gameObject,this));
        _cookToolIcons.Add(new CookToolIcon(Ins_CookToolIconD.gameObject,this));
        _cookToolIcons.Add(new CookToolIcon(Ins_CookToolIconF.gameObject,this));
        
        _dataProvider = UniModule.GetModule<DataProviderModule>();

        _selectedQte = new();
        
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var one in _cacheCells)
        {
            one.OnDestroy();
        }
        _cacheCells.Clear();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);

        //绑定按钮事件
        ButtonClickBind();
        //获取当前玩家拥有的食材
        _ownedFoodItems = UserInfoModule.Instance.OwnFoodMaterials;
        
        setFoodView(cfg.food.materialType.Meat);

        foreach (var t in _canMakeFoodIcons)
        {
            t.uiGo.SetActive(false);
        }

        _selectedQte.Clear();
        
        EventModule.Instance.StartCookSub.Subscribe(ClearChoice).AddTo(handles);

    }

    public override void OnHide()
    {
        base.OnHide();
        foreach (var one in _cacheCells)
        {
            one.OnHide();
        }
        handles.Clear();
    }

    private void ButtonClickBind()
    {
        _cookToolIcons[0].Btn_Click.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Cook,0);
        }).AddTo(handles);
        
        _cookToolIcons[1].Btn_Click.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Barbecue,1);
        }).AddTo(handles);
        _cookToolIcons[2].Btn_Click.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Steam,2);
        }).AddTo(handles);
        _cookToolIcons[3].Btn_Click.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Fry,3);
        }).AddTo(handles);
        _cookToolIcons[4].Btn_Click.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Boil,4);
        }).AddTo(handles);

        Btn_ClassifyA.OnClickAsObservable().Subscribe((param) =>
        {
            setFoodView(cfg.food.materialType.Seafood);
        }).AddTo(handles);
        Btn_ClassifyB.OnClickAsObservable().Subscribe((param) =>
        {
            setFoodView(cfg.food.materialType.Meat);
        }).AddTo(handles);
        Btn_ClassifyC.OnClickAsObservable().Subscribe((param) =>
        {
            setFoodView(cfg.food.materialType.Vegetable);
        }).AddTo(handles);
        Btn_ClassifyD.OnClickAsObservable().Subscribe((param) =>
        {
            setFoodView(cfg.food.materialType.Other);
        }).AddTo(handles);
        
        Btn_produce.OnClickAsObservable().Subscribe(EnterProduce).AddTo(handles);
        
        Toggle_A.OnValueChangedAsObservable().Skip(1).Subscribe(b=>
        {
            AddQTE(1);
        }).AddTo(handles);
        
        Toggle_B.OnValueChangedAsObservable().Skip(1).Subscribe(b =>
        {
            AddQTE(2);
        }).AddTo(handles);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private LoopGridViewItem getFoodItem(LoopGridView gridView, int itemIndex, int row, int column)
    {
        var item = gridView.NewListViewItem("FoodBtnCell");
        FoodBtnCell cell = null;
        if (item.IsInitHandlerCalled)
        {
            cell = item.UserObjectData as FoodBtnCell;
        }
        else
        {
            cell = new FoodBtnCell(item.gameObject,this);
            item.UserObjectData = cell;
            item.IsInitHandlerCalled = true;
            _cacheCells.Add(cell);
        }
        cell.OnShow(null);
        var data = _showFoodItems[itemIndex];
        var tbFood = DataProviderModule.Instance.GetFoodBaseInfo(data.Id);
        var canChoice = true;
        for (int i = 0; i < tbFood.Tag.Count; i++)
        {
            if (_oppositeTags.Contains(tbFood.Tag[i]))
            {
                canChoice = false;
                break;
            }
        }
        cell.SetFoodInfo(data.Id,data.Num,canChoice,clickFoodMaterial);
        
        return item;
    }

    private void setFoodView(cfg.food.materialType foodType)
    {
        if (foodType == _foodMaterialType) return;
        
        _foodMaterialType = foodType;
        _showFoodItems.Clear();

        var provider = UniModule.GetModule<DataProviderModule>();
        
        foreach (var one in _ownedFoodItems)
        {
            var tb = provider.GetFoodBaseInfo(one.Id);
            if (tb.Type == foodType)
            {
                _showFoodItems.Add(one);
            }
        }
        Grid_FoodItem.SetListItemCount(_showFoodItems.Count);
        Grid_FoodItem.RefreshAllShownItem();
    }
    
    private void clickFoodMaterial(int foodId)
    {
        int emptyIndex = -1;
        for (int i = 0; i < _choicedFoodIcons.Count; i++)
        {
            if (_choicedFoodIcons[i].FoodId == 0)
            {
                emptyIndex = i;
                break;
            }
        }

        if (emptyIndex < 0) return;
        _choicedFoodIcons[emptyIndex].FoodMaterialInfo(foodId,checkCanProduce);

        addOppositeTag(foodId);
        
        checkCanProduce();
    }

    private void clickCookTool(cfg.food.cookTools toolType,int index)
    {
        _choicedTools = toolType;
        for (int i = 0; i < _cookToolIcons.Count; i++)
        {
            _cookToolIcons[i].HideHighlight();
        }
        _cookToolIcons[index].ShowHighlight();
        checkCanProduce();
    }

    private void addOppositeTag(int foodId)
    {
        var tbFood = DataProviderModule.Instance.GetFoodBaseInfo(foodId);
        for (int i = 0; i < tbFood.OppositeTag.Count; i++)
        {
            _oppositeTags.Add(tbFood.OppositeTag[i]);
        }
    }

    private void clearOppositeTag()
    {
        _oppositeTags.Clear();
    }
    
    private void checkCanProduce()
    {
        foreach (var t in _canMakeFoodIcons)
        {
            t.uiGo.SetActive(false);
        }

        clearOppositeTag();
        var tmp = 0;
        foreach (var icon in _choicedFoodIcons)
        {
            if (icon.FoodId > 0)
            {
                tmp++;
                addOppositeTag(icon.FoodId);    
            }
        }
        if (tmp <= 0) return;

        _curSelectMenuId = 0;
        
        var menuList = _dataProvider.DataBase.TbMenuInfo.DataList;
        //通过厨具删选出可以制作的料理
        var toolsCanMakeMenu = new List<MenuInfo>(10);
        foreach (var one in menuList)
        {
            if (one.MakeMethod == _choicedTools)
            {
                toolsCanMakeMenu.Add(one);
            }
        }

        HashSet<int> needMaterial = new ();
        int showMenuIndex = 0;
        //判断当前食材是否满足
        foreach (var one in toolsCanMakeMenu)
        {
            needMaterial.Clear();
            foreach (var m in one.RelatedMaterial)
            {
                needMaterial.Add(m);
            }
            
            int checkCount = 0;
            foreach (var choiced in _choicedFoodIcons)
            {
                if (needMaterial.Contains(choiced.FoodId))
                {
                    checkCount++;
                }
            }

            if (checkCount == needMaterial.Count)
            {//满足显示
                var tbItem = _dataProvider.GetItemBaseInfo(one.Id);
                _canMakeFoodIcons[showMenuIndex].SetMenuInfo(tbItem,ClickMenuIcon,showMenuIndex);
                showMenuIndex++;
            }
        }
    }

    private void ClickMenuIcon(int menuId,int menuIndex)
    {
        if (_curSelectMenuId == menuId) return;

        foreach (var icon in _canMakeFoodIcons)
        {
            icon.HideHighlight();
        }
        _canMakeFoodIcons[menuIndex].ShowHighlight();
        
        _curSelectMenuId = menuId;
    }

    private void EnterProduce(Unit param)
    {
        switch (_choicedTools)
        {
            case cookTools.Cook:
                break;
            case cookTools.Barbecue:
                break;
            case cookTools.Boil:
                break;
            case cookTools.Steam:
                break;
        }

        if (_choicedTools == 0) return;
        if (_curSelectMenuId == 0) return;
        
        var foodReceipt = new PickFoodAndTools();
        foodReceipt.MenuId = _curSelectMenuId;
        foodReceipt.CookTools = _choicedTools;
        foodReceipt.CookFoods = new List<ItemTableData>(_choicedFoodIcons.Count);
        foreach (var icon in _choicedFoodIcons)
        {
            if(icon.FoodId <= 0)continue;
            var tmp = new ItemTableData()
            {
                Id = icon.FoodId,
                Num = 1
            };
            foodReceipt.CookFoods.Add(tmp);
        }

        foodReceipt.QTESets = _selectedQte;
        
        EventModule.Instance.StartCookTopic.OnNext(foodReceipt);
    }

    private void ClearChoice(PickFoodAndTools param)
    {
        foreach (var one in _choicedFoodIcons)
        {
            one.ClearMaterialInfo();
        }

        foreach (var one in _canMakeFoodIcons)
        {
            one.ClearMenuInfo();
            one.HideHighlight();
        }

        foreach (var one in _cookToolIcons)
        {
            one.HideHighlight();
        }
        _choicedTools = 0;
        Toggle_A.isOn = false;
        Toggle_B.isOn = false;
    }

    private void AddQTE(int qteId)
    {
        if (_selectedQte.Contains(qteId))
        {
            _selectedQte.Remove(qteId);
        }
        else
        {
            _selectedQte.Add(qteId);
        }
    }

}