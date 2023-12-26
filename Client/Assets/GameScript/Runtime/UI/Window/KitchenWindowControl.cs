using System;
using System.Collections.Generic;
using cfg.food;
using Cysharp.Threading.Tasks;
using SuperScrollView;
using UniRx;
using UnityEngine;


/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class KitchenWindow : UIWindow
{
    public Action<int,bool> StartCook;
    public Action<int> AddFoodCB;
    private List<RefrigeratorCell> _refrigeratorCells;
    private List<PreviewRecipeCell> _previewRecipeCells;
    private cfg.food.materialType _showMaterialType = materialType.Other;

    
    public class PreviewRecipe
    {
        public int RecipeId;
        public bool IsFull;
    }

    private List<PreviewRecipe> _previewRecipes;
    private List<ItemTableData> _showFoodItems;
    private List<ItemTableData> _ownedFoodItems;
    
    
    
    public override void OnCreate()
    {
        base.OnCreate();
        Grid_Refrigerator.InitGridView(0,RefrigeratorItems);
        List2_PreviewRecipt.InitListView(10,PreviewRecipes);
        // Grid_FoodItem.InitGridView(0,getFoodItem);
        //
        // _cacheCells = new List<FoodBtnCell>(10);
        _showFoodItems = new List<ItemTableData>(10);
        _refrigeratorCells = new(10);
        
        
        
        _previewRecipes = new(10);
        _previewRecipeCells = new List<PreviewRecipeCell>(10);

    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        // foreach (var one in _cacheCells)
        // {
        //     one.OnDestroy();
        // }
        // _cacheCells.Clear();
        _refrigeratorCells.Clear();
        _previewRecipeCells.Clear();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);

        //绑定按钮事件
        ButtonClickBind();
        //获取当前玩家拥有的食材
        _ownedFoodItems = UserInfoModule.Instance.OwnFoodMaterials;
        
        // XBtn_pick.OnClick.Subscribe(pickFood).AddTo(handles);
        // XBtn_cancel.OnClick.Subscribe(cancelFood).AddTo(handles);
            
        setFoodView(materialType.Meat);
        Observable.EveryLateUpdate().Subscribe(LateUpdate).AddTo(handles);
        
        // foreach (var t in _canMakeFoodIcons)
        // {
        //     t.uiGo.SetActive(false);
        // }
        //
        // _selectedQte.Clear();
        //
        // EventModule.Instance.StartCookSub.Subscribe(ClearChoice).AddTo(handles);
    }
    
    void LateUpdate(long param)
    {
        List2_PreviewRecipt.UpdateAllShownItemSnapData();
        int count = List2_PreviewRecipt.ShownItemCount;
        for (int i = 0; i < count; ++i)
        {
            LoopListViewItem2 item = List2_PreviewRecipt.GetShownItemByIndex(i);
            // ListItem2 itemScript = item.GetComponent<ListItem2>();
            float scale = 1 - Mathf.Abs(item.DistanceWithViewPortSnapCenter)/ 350f;
            scale = Mathf.Clamp(scale, 0.4f, 1);
            item.gameObject.GetComponent<CanvasGroup>().alpha = scale;
            item.gameObject.transform.localScale = new Vector3(scale, scale, 1);
        }
        // Debug.Log($"anchoredPosition = {List2_PreviewRecipt.ContainerTrans.anchoredPosition}");
        var vec = List2_PreviewRecipt.ContainerTrans.anchoredPosition;
        float maxY = List2_PreviewRecipt.ContainerTrans.sizeDelta.y - 200f;
        if (vec.y <= -200f)
        {
            List2_PreviewRecipt.ContainerTrans.anchoredPosition = new Vector2(0,-200f);
        }
        else if(vec.y >= maxY)
        {
            List2_PreviewRecipt.ContainerTrans.anchoredPosition = new Vector2(0,maxY);
        }
    }

    public override void OnHide()
    {
        base.OnHide();
        XBtn_cancel.gameObject.SetActive(false);
    }
    
    private void ButtonClickBind()
    {
        XBtn_A.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Seafood);
        }).AddTo(handles);
        
        XBtn_B.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Meat);
        }).AddTo(handles);
        XBtn_C.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Vegetable);
        }).AddTo(handles);
        
        XBtn_D.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Other);
        }).AddTo(handles);
        
        XBtn_order.OnClick.Subscribe(param =>
        {
            if (UserInfoModule.Instance.HaveOrder)
            {
                UIManager.Instance.OpenUI(UIEnum.HandleOrderWindow, null, null);    
            }
            else
            {
                TipCommonData data = new TipCommonData();
                data.tipstr = "当前没有订单";
                UIManager.Instance.CreateTip<TipCommon>(data).Forget();
            }
        }).AddTo(handles);

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    
    public void showCancelBtn(Transform pick)
    {
        XBtn_cancel.gameObject.SetActive(true);
        var uipos = UIManager.Instance.WorldPositionToUI(pick.position);
        uipos.x += 80f;
        uipos.y -= 80f;
        var rt = XBtn_cancel.GetComponent<RectTransform>();
        rt.anchoredPosition = uipos;
    }

    public void hideCancelBtn()
    {
        XBtn_cancel.gameObject.SetActive(false);
    }
    
    private void setFoodView(cfg.food.materialType foodType)
    {
        if (foodType == _showMaterialType) return;
        
        _showMaterialType = foodType;
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
        Grid_Refrigerator.SetListItemCount(_showFoodItems.Count);
        Grid_Refrigerator.RefreshAllShownItem();
    }
    

    private LoopGridViewItem RefrigeratorItems(LoopGridView gridView, int itemIndex, int row, int column)
    {
        var item = gridView.NewListViewItem("RefrigeratorCell");
        RefrigeratorCell cell = null;
        
        if (item.IsInitHandlerCalled)
        {
            cell = item.UserObjectData as RefrigeratorCell;
        }
        else
        {
            cell = new RefrigeratorCell(item.gameObject,this);
            item.UserObjectData = cell;
            item.IsInitHandlerCalled = true;
            _refrigeratorCells.Add(cell);
        }
        
        var data = _showFoodItems[itemIndex];
        cell.SetCellInfo(data.Id,data.Num,clickIcon);
        
        return item;
    }

    private LoopListViewItem2 PreviewRecipes(LoopListView2 listView, int index)
    {
        if (index < 0 || index >= _previewRecipes.Count)
            return null;
       
        LoopListViewItem2 item = listView.NewListViewItem("PreviewRecipeCell");
        PreviewRecipeCell cell = null;
        if (item.IsInitHandlerCalled == false)
        {
            cell = new PreviewRecipeCell(item.gameObject, this);
            item.IsInitHandlerCalled = true;
            _previewRecipeCells.Add(cell);
            item.UserObjectData = cell;
        }
        else
        {
            cell = item.UserObjectData as PreviewRecipeCell;
        }

        var data = _previewRecipes[index];
        var tb = DataProviderModule.Instance.GetMenuInfo(data.RecipeId);
        cell.SetRecipe(data.RecipeId,tb.Name,data.IsFull,StartCook);
        
        // ListItem2 itemScript = item.GetComponent<ListItem2>();
        // if (item.IsInitHandlerCalled == false)
        // {
        //     item.IsInitHandlerCalled = true;
        //     itemScript.Init();
        // }
        // itemScript.SetItemData(itemData, index);
        return item;
    }
    
    private void clickRecipe(int recipeId,bool isFull)
    {
        // StartCook?.Invoke(recipeId);
        // if (isFull)
        // {//跳转到做菜
        //     
        // }
        // else
        // {
        //     var data = new TipCommonData();
        //     data.tipstr = "材料不足";
        //     UIManager.Instance.CreateTip<TipCommon>(data).Forget();
        // }
        
        //1 尝试补全材料
        // List<int> lack = new List<int>(5);
        // var tb = DataProviderModule.Instance.GetMenuInfo(recipeId);
        // for (int i = 0; i < tb.RelatedMaterial.Count; i++)
        // {
        //     var needFoodId = tb.RelatedMaterial[i];
        //     if (_pickFoodId.Count(x => x == needFoodId) <= 0)
        //     {
        //         lack.Add(needFoodId);
        //     }
        // }

        // var isEnough = true;
        // for (int i = 0; i < lack.Count; i++)
        // {
        //     isEnough = UserInfoModule.Instance.IsEnoughItem(lack[i], 1);
        //     if (!isEnough) break;
        // }
        
        // if (isEnough)
        // {//先添加原材料
        //     for (int i = 0; i < lack.Count; i++)
        //     {
        //         // _pickFoodId.Add(lack[i]);
        //         AddFoodCB?.Invoke(lack[i]);
        //     }
        //     // checkPreviewRecipe();
        //     StartCook?.Invoke(recipeId);
        // }
        // else
        // {//2 提示材料不足
        //     var data = new TipCommonData();
        //     data.tipstr = "材料不足";
        //     UIManager.Instance.CreateTip<TipCommon>(data).Forget();
        // }
    }
    
    private void clickIcon(int itemId)
    {
        AddFoodCB?.Invoke(itemId);
        // _pickFoodId.Add(itemId);

        // checkPreviewRecipe();
    }
    

    // private void checkPreviewRecipe()
    // {
    //     var recipe = GlobalFunctions.SearchRecipe(_pickFoodId);
    //     if (recipe.Count <= 0)
    //     {
    //         _previewRecipes.Clear();
    //         List2_PreviewRecipt.SetListItemCount(0);
    //         return;
    //     }
    //     
    //     //先剔除掉 失效的
    //     for (int i = _previewRecipes.Count - 1; i >= 0; i--)
    //     {
    //         var one = _previewRecipes[i];
    //         if (!recipe.Contains(one.RecipeId))
    //         {
    //             _previewRecipes.RemoveAt(i);
    //         }
    //     }
    //
    //     foreach (var one in recipe)
    //     {
    //         var ret = _previewRecipes.Find(x => x.RecipeId == one);
    //         if (ret == null)
    //         {
    //             PreviewRecipe newOne = new();
    //             newOne.RecipeId = one;
    //             newOne.IsFull = false;
    //             _previewRecipes.Add(newOne);
    //         }
    //     }
    //     List2_PreviewRecipt.gameObject.SetActive(_previewRecipes.Count > 0);
    //     if (List2_PreviewRecipt.ItemTotalCount != _previewRecipes.Count)
    //     {
    //         List2_PreviewRecipt.SetListItemCount(_previewRecipes.Count);    
    //     }
    //     List2_PreviewRecipt.RefreshAllShownItem();
    // }

    public void RefreshPreviewRecipe(List<PreviewRecipe> recipes)
    {
        _previewRecipes = recipes;
        List2_PreviewRecipt.gameObject.SetActive(_previewRecipes.Count > 0);
        if (List2_PreviewRecipt.ItemTotalCount != _previewRecipes.Count)
        {
            List2_PreviewRecipt.SetListItemCount(_previewRecipes.Count);    
        }
        List2_PreviewRecipt.RefreshAllShownItem();
    }

    public void HidePreviewRecipe()
    {
        List2_PreviewRecipt.gameObject.SetActive(false);
    }

}