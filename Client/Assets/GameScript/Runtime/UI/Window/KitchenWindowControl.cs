using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.food;
using SuperScrollView;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class KitchenWindow : UIWindow
{
    private cfg.food.cookTools _choicedTools;
    private List<ItemDataDef> _ownedFoodItems;
    private List<ItemDataDef> _showFoodItems;
    private List<FoodBtnCell> _cacheCells;
    private cfg.food.materialType _foodMaterialType;
    private UserInfoModule _userInfoModule;

    private List<FoodIcon> _choicedFoodIcons;
    private List<MenuIcon> _canMakeFoodIcons;
    private DataProviderModule _dataProvider;
    public override void OnCreate()
    {
        base.OnCreate();
        Grid_FoodItem.InitGridView(0,getFoodItem);
        _userInfoModule = UniModule.GetModule<UserInfoModule>();
        _cacheCells = new List<FoodBtnCell>(10);
        _showFoodItems = new List<ItemDataDef>(10);

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

        _dataProvider = UniModule.GetModule<DataProviderModule>();

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
        Btn_ToolA.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Cook);
        }).AddTo(handles);
        
        Btn_ToolB.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Barbecue);
        }).AddTo(handles);
        Btn_ToolC.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Steam);
        }).AddTo(handles);
        Btn_ToolD.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Fry);
        }).AddTo(handles);
        Btn_ToolE.OnClickAsObservable().Subscribe((param)=>
        {
            clickCookTool(cfg.food.cookTools.Boil);
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
        
        //获取当前玩家拥有的食材
        _ownedFoodItems = _userInfoModule.OwnFoodMaterials;

        setFoodView(cfg.food.materialType.Meat);

        foreach (var t in _canMakeFoodIcons)
        {
            t.uiGo.SetActive(false);
        }
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
        cell.SetFoodInfo(data.Id,data.Num,clickFoodMaterial);
        
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

        if (Grid_FoodItem.ItemTotalCount != _showFoodItems.Count)
        {
            Grid_FoodItem.SetListItemCount(_showFoodItems.Count);    
        }
        else
        {
            Grid_FoodItem.RefreshAllShownItem();
        }
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
        checkCanProduce();
    }

    private void clickCookTool(cfg.food.cookTools toolType)
    {
        _choicedTools = toolType;
        checkCanProduce();
    }

    private void checkCanProduce()
    {
        foreach (var t in _canMakeFoodIcons)
        {
            t.uiGo.SetActive(false);
        }

        var tmp = 0;
        foreach (var icon in _choicedFoodIcons)
        {
            if (icon.FoodId > 0) tmp++;
        }
        if (tmp <= 0) return;
        

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
                _canMakeFoodIcons[showMenuIndex].SetMenuInfo(tbItem);
                showMenuIndex++;
            }
        }

        
    }
}