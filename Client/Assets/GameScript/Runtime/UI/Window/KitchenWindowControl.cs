using System.Collections;
using System.Collections.Generic;
using cfg.food;
using SuperScrollView;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class KitchenWindow : UIWindow
{
    
    // private List<FoodBaseInfo> choicedFood;
    private Object choicedTools;
    private List<int> _ownerFood;
    private List<int> _showFood;
    private List<FoodBtnCell> _cacheCells;
    private cfg.food.rawType _foodType;
    private UserInfoModule _userInfoModule;
    public override void OnCreate()
    {
        base.OnCreate();
        Grid_FoodItem.InitGridView(0,getFoodItem);
        _userInfoModule = UniModule.GetModule<UserInfoModule>();
        _cacheCells = new List<FoodBtnCell>(10);
        _showFood = new(10);
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
            clickTool(1);
        }).AddTo(handles);
        
        Btn_ToolB.OnClickAsObservable().Subscribe((param)=>
        {
            clickTool(2);
        }).AddTo(handles);
        Btn_ToolC.OnClickAsObservable().Subscribe((param)=>
        {
            clickTool(3);
        }).AddTo(handles);
        Btn_ToolD.OnClickAsObservable().Subscribe((param)=>
        {
            clickTool(4);
        }).AddTo(handles);
        Btn_ToolE.OnClickAsObservable().Subscribe((param)=>
        {
            clickTool(5);
        }).AddTo(handles);
        _ownerFood = _userInfoModule.OwnFoods;
        // Grid_FoodItem.SetListItemCount(_ownerFood.Count);
        
        setFoodView(cfg.food.rawType.Meat);
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
        var data = _showFood[itemIndex];
        cell.SetFoodInfo(data,1,clickFood);
        
        return item;
    }

    private void setFoodView(cfg.food.rawType foodType)
    {
        if (foodType == _foodType) return;
        
        _foodType = foodType;
        _showFood.Clear();

        var provider = UniModule.GetModule<DataProviderModule>();
        
        foreach (var one in _ownerFood)
        {
            var tb = provider.GetFoodBaseInfo(one);
            if (tb.Type == foodType)
            {
                _showFood.Add(tb.Id);
            }
        }
        Grid_FoodItem.SetListItemCount(_showFood.Count);
    }
    
    private void clickFood(int foodId)
    {
        
    }

    private void clickTool(int toolId)
    {
        
    }
}