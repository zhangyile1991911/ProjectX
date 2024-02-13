using System;
using System.Collections.Generic;
using cfg.food;
using Cysharp.Threading.Tasks;
using SuperScrollView;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;


/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class KitchenWindow : UIWindow
{
    public Action<int,bool> StartCook;
    public Action<int> AddFoodCB;
    private List<RefrigeratorCell> _refrigeratorCells;
    private List<OptionalReceipt> _optionalMenuCells;
    private List<int> _choicedMaterial;
    private materialType _showMaterialType = materialType.Other;
    

    private List<int> _previewReceiptId;
    private List<ItemTableData> _showFoodItems;
    private List<ItemTableData> _ownedFoodItems;
    
    
    
    public override void OnCreate()
    {
        base.OnCreate();
        Grid_Refrigerator.InitGridView(0,RefrigeratorItems);
        List2_PreviewRecipt.InitListView(10,PreviewRecipes);
        
        _choicedMaterial = ListPool<int>.Get();
        _optionalMenuCells = ListPool<OptionalReceipt>.Get();
        _showFoodItems = ListPool<ItemTableData>.Get();
        _ownedFoodItems = ListPool<ItemTableData>.Get();
        _refrigeratorCells = ListPool<RefrigeratorCell>.Get();
        _previewReceiptId = ListPool<int>.Get();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();

        ListPool<int>.Release(_choicedMaterial);
        ListPool<OptionalReceipt>.Release(_optionalMenuCells);
        ListPool<ItemTableData>.Release(_showFoodItems);
        ListPool<ItemTableData>.Release(_ownedFoodItems);
        ListPool<RefrigeratorCell>.Release(_refrigeratorCells);
        ListPool<int>.Release(_previewReceiptId);
        
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
        // List2_PreviewRecipt.UpdateAllShownItemSnapData();
        // int count = List2_PreviewRecipt.ShownItemCount;
        // for (int i = 0; i < count; ++i)
        // {
        //     LoopListViewItem2 item = List2_PreviewRecipt.GetShownItemByIndex(i);
        //
        //     float scale = 1 - Mathf.Abs(item.DistanceWithViewPortSnapCenter)/ 350f;
        //     scale = Mathf.Clamp(scale, 0.4f, 1);
        //     item.gameObject.GetComponent<CanvasGroup>().alpha = scale;
        //     item.gameObject.transform.localScale = new Vector3(scale, scale, 1);
        // }
        //
        // var vec = List2_PreviewRecipt.ContainerTrans.anchoredPosition;
        // float maxY = List2_PreviewRecipt.ContainerTrans.sizeDelta.y - 200f;
        // if (vec.y <= -200f)
        // {
        //     List2_PreviewRecipt.ContainerTrans.anchoredPosition = new Vector2(0,-200f);
        // }
        // else if(vec.y >= maxY)
        // {
        //     List2_PreviewRecipt.ContainerTrans.anchoredPosition = new Vector2(0,maxY);
        // }
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
        if (index < 0 || index >= _previewReceiptId.Count)
            return null;
       
        LoopListViewItem2 item = listView.NewListViewItem("OptionalReceipt");
        OptionalReceipt cell = null;
        if (item.IsInitHandlerCalled == false)
        {
            cell = new OptionalReceipt(item.gameObject, this);
            // cell.BgButton.OnClick.Subscribe((PointerEventData param) =>
            // {
            //     StartCook(cell.ReceiptId,cell.IsFull);
            // }).AddTo(handles);
            item.IsInitHandlerCalled = true;
            _optionalMenuCells.Add(cell);
            item.UserObjectData = cell;
        }
        else
        {
            cell = item.UserObjectData as OptionalReceipt;
        }
        
        var receiptId = _previewReceiptId[index];
        cell.StartCook = StartCook;
        cell.SetReceipt(receiptId,_choicedMaterial);
        
        return item;
    }
    
    private void clickIcon(int itemId)
    {
        _choicedMaterial.Add(itemId);
        foreach (var one in _optionalMenuCells)
        {
            one.RefreshMaterial(_choicedMaterial);
        }
        AddFoodCB?.Invoke(itemId);
    }

    public void RefreshPreviewRecipe(List<int> recipes)
    {
        _previewReceiptId = recipes;
        List2_PreviewRecipt.gameObject.SetActive(_previewReceiptId.Count > 0);
        if (List2_PreviewRecipt.ItemTotalCount != _previewReceiptId.Count)
        {
            List2_PreviewRecipt.SetListItemCount(_previewReceiptId.Count);    
        }
        else
        {
            List2_PreviewRecipt.RefreshAllShownItem();
        }
    }

    public void HidePreviewRecipe()
    {
        List2_PreviewRecipt.gameObject.SetActive(false);
    }

}