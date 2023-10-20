using System;
using System.Collections.Generic;
using System.Linq;
using cfg.food;
using Cysharp.Threading.Tasks;
using SuperScrollView;
using UniRx;
using Unity.Plastic.Newtonsoft.Json.Serialization;

using UnityEngine;
using UnityEngine.EventSystems;

using YooAsset;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class KitchenWindow : UIWindow
{

    public Action<int> StartCook;
    private List<ItemTableData> _ownedFoodItems;
    private List<ItemTableData> _showFoodItems;
    
    private List<RefrigeratorCell> _refrigeratorCells;
    private List<PreviewRecipeCell> _previewRecipeCells;
    private cfg.food.materialType _showMaterialType = materialType.Other;

    private List<OutlineControl> _foodGameObjects;

    class PreviewRecipe
    {
        public int RecipeId;
        public bool IsFull;
    }

    private List<PreviewRecipe> _previewRecipes;
    struct PickFoodInfo
    {
        public OutlineControl PickFood;
        public int index;
    }

    private PickFoodInfo _curPickInfo;
    
    
    private Camera _mainCamera;
    private LayerMask _foodLayerMask;

    private List<int> _pickFoodId;
    
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

        _foodGameObjects = new List<OutlineControl>(10);
        _pickFoodId = new(10);
        _foodLayerMask = 1<<6;
        
        _curPickInfo = new PickFoodInfo();
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
        _mainCamera = Camera.main;
        
        setFoodView(materialType.Meat);
        Observable.EveryLateUpdate().Subscribe(LateUpdate).AddTo(handles);
        //
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
        // foreach (var one in _cacheCells)
        // {
        //     one.OnHide();
        // }
        // handles.Clear();
    }

    // public enum KitchenMode
    // {
    //     PickFoodMode,
    //     PrepareCook,
    // }
    //
    // private KitchenMode curKitchenMode;
    // public void SwitchMode(KitchenMode next)
    // {
    //     curKitchenMode = next;
    //     
    //     Tran_refrigerator.gameObject.SetActive(curKitchenMode == KitchenMode.PickFoodMode);
    //     
    // }

    
    
    private void ButtonClickBind()
    {
        XBtn_A.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Seafood);
            // tmp("Assets/GameRes/Prefabs/AllFood/SceneFood/potato.prefab");
        }).AddTo(handles);
        
        XBtn_B.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Meat);
            // tmp("Assets/GameRes/Prefabs/AllFood/SceneFood/dabaicai.prefab");
        }).AddTo(handles);
        XBtn_C.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Vegetable);
            // tmp("Assets/GameRes/Prefabs/AllFood/SceneFood/xiaoqingcai.prefab");
        }).AddTo(handles);
        
        XBtn_D.OnClick.Subscribe(param =>
        {
            setFoodView(cfg.food.materialType.Other);
        }).AddTo(handles);
        
        XBtn_pick.OnClick.Subscribe(checkPickFood).AddTo(handles);
        XBtn_cancel.OnClick.Subscribe(cancelPickFood).AddTo(handles);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void checkPickFood(PointerEventData param)
    {
        Ray ray = _mainCamera.ScreenPointToRay(param.position);
        // 执行射线检测
        if (Physics.Raycast(ray, out var hit, _foodLayerMask))
        {
            // 检测到碰撞
            if (!hit.collider.CompareTag("BasketFood")) return;
            
            OutlineControl tmp = null;
            int index = 0;
            for (;index < _foodGameObjects.Count;index++)
            {
                var one = _foodGameObjects[index];
                if (one.transform == hit.transform)
                {
                    tmp = one;
                    break;
                }
            }

            if (tmp == null) return;
            
            if (_curPickInfo.PickFood != null && _curPickInfo.PickFood != tmp)
            {
                _curPickInfo.PickFood.HideOutline();
                _curPickInfo.PickFood = tmp;
                _curPickInfo.PickFood.ShowOutline();
                _curPickInfo.index = index;
                showCancelBtn(_curPickInfo.PickFood.transform);
            }
            else if (_curPickInfo.PickFood == null && tmp != null)
            {
                _curPickInfo.PickFood = tmp;
                _curPickInfo.PickFood.ShowOutline();
                _curPickInfo.index = index;
                showCancelBtn(_curPickInfo.PickFood.transform);
            }
            else if (tmp == _curPickInfo.PickFood)
            {
                _curPickInfo.PickFood.HideOutline();
                _curPickInfo.PickFood = null;
                _curPickInfo.index = -1;
                hideCancelBtn();
            }
            
        }
    }

    private void cancelPickFood(PointerEventData param)
    {
        if (_curPickInfo.PickFood == null || _curPickInfo.index < 0)
        {
            return;
        }

        //todo 之后考虑用对象池
        var tmp = _foodGameObjects[_curPickInfo.index];
        Object.Destroy(tmp.gameObject);
        
        _foodGameObjects.RemoveAt(_curPickInfo.index);
        _pickFoodId.RemoveAt(_curPickInfo.index);

        _curPickInfo.PickFood = null;
        _curPickInfo.index = -1;
        XBtn_cancel.gameObject.SetActive(false);
        
        checkPreviewRecipe();

    }
    
    private void showCancelBtn(Transform pick)
    {
        XBtn_cancel.gameObject.SetActive(true);
        var uipos = UIManager.Instance.WorldPositionToUI(pick.position);
        uipos.x += 80f;
        uipos.y -= 80f;
        var rt = XBtn_cancel.GetComponent<RectTransform>();
        rt.anchoredPosition = uipos;
    }

    private void hideCancelBtn()
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
        var isFull = _pickFoodId.Count >= tb.RelatedMaterial.Count;
        if (isFull)
        {
            foreach (var one in tb.RelatedMaterial)
            {
                if (_pickFoodId.Count(x => x == one) <= 0)
                {
                    isFull = false;
                    break;
                }
            }
        }
        cell.SetRecipe(data.RecipeId,tb.Name,isFull,choseRecipe);
        
        // ListItem2 itemScript = item.GetComponent<ListItem2>();
        // if (item.IsInitHandlerCalled == false)
        // {
        //     item.IsInitHandlerCalled = true;
        //     itemScript.Init();
        // }
        // itemScript.SetItemData(itemData, index);
        return item;
    }

    private void resetFoodOutline()
    {
        foreach (var one in _foodGameObjects)
        {
            one.HideOutline();
        }
        XBtn_cancel.gameObject.SetActive(false);
    }
    
    private async void choseRecipe(int recipeId,bool isFull)
    {
        if (isFull)
        {//跳转到做菜
            resetFoodOutline();
            StartCook?.Invoke(recipeId);
            return;
        }
        
        //1 尝试补全材料
        List<int> lack = new List<int>(5);
        var tb = DataProviderModule.Instance.GetMenuInfo(recipeId);
        for (int i = 0; i < tb.RelatedMaterial.Count; i++)
        {
            var needFoodId = tb.RelatedMaterial[i];
            if (_pickFoodId.Count(x => x == needFoodId) <= 0)
            {
                lack.Add(needFoodId);
            }
        }

        var isEnough = true;
        for (int i = 0; i < lack.Count; i++)
        {
            isEnough = UserInfoModule.Instance.IsEnoughItem(lack[i], 1);
            if (!isEnough) break;
        }
        
        if (isEnough)
        {//先添加原材料
            for (int i = 0; i < lack.Count; i++)
            {
                _pickFoodId.Add(lack[i]);
                await loadFoodObject(lack[i]);
            }
            checkPreviewRecipe();
            // await UniTask.Delay(TimeSpan.FromSeconds(2));
            //跳转界面
            resetFoodOutline();
            StartCook?.Invoke(recipeId);
        }
        else
        {//2 提示材料不足
            var data = new TipCommonData();
            data.tipstr = "材料不足";
            UIManager.Instance.CreateTip<TipCommon>(data).Forget();
        }

    }
    
    private void clickIcon(int itemId)
    {
        if (_foodGameObjects.Count >= 10)
        {
            var data = new TipCommonData();
            data.tipstr = "超过上限";
            UIManager.Instance.CreateTip<TipCommon>(data).Forget();
            return;   
        }
        
        _pickFoodId.Add(itemId);
        
        loadFoodObject(itemId).Forget();

        checkPreviewRecipe();
    }

    private async UniTask loadFoodObject(int itemId)
    {
        var itemTb = DataProviderModule.Instance.GetItemBaseInfo(itemId);
        if (itemTb == null)
        {
            Debug.Log($"itemId = {itemId} itemTb == null");
            return;
        }
        var res = YooAssets.LoadAssetAsync<GameObject>(itemTb.SceneResPath);
        await res.ToUniTask();
        var go = res.AssetObject as GameObject;
        var obj = GameObject.Instantiate(go);
        obj.transform.position = new Vector3(Random.Range(7f,17f),Random.Range(1007f,997f),-6f);
        _foodGameObjects.Add(obj.GetComponent<OutlineControl>());
    }
    

    private void checkPreviewRecipe()
    {
        var recipe = GlobalFunctions.SearchRecipe(_pickFoodId);
        if (recipe.Count <= 0)
        {
            _previewRecipes.Clear();
            List2_PreviewRecipt.SetListItemCount(0);
            return;
        }
        
        //先剔除掉 失效的
        for (int i = _previewRecipes.Count - 1; i >= 0; i--)
        {
            var one = _previewRecipes[i];
            if (!recipe.Contains(one.RecipeId))
            {
                _previewRecipes.RemoveAt(i);
            }
        }

        foreach (var one in recipe)
        {
            var ret = _previewRecipes.Find(x => x.RecipeId == one);
            if (ret == null)
            {
                PreviewRecipe newOne = new();
                newOne.RecipeId = one;
                newOne.IsFull = false;
                _previewRecipes.Add(newOne);
            }
        }
        List2_PreviewRecipt.gameObject.SetActive(_previewRecipes.Count > 0);
        if (List2_PreviewRecipt.ItemTotalCount != _previewRecipes.Count)
        {
            List2_PreviewRecipt.SetListItemCount(_previewRecipes.Count);    
        }
        List2_PreviewRecipt.RefreshAllShownItem();
    }
    
    
    
    // private void can

}