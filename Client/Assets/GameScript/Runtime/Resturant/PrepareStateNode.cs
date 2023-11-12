using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using YooAsset;
using System.Linq;

public class PrepareStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private CompositeDisposable _handle;
    private bool _isMoving;
    private KitchenWindow _kitchenWindow;
    private Camera _mainCamera;
    
    struct PickFoodInfo
    {
        public OutlineControl PickFood;
        public int PickIndex;
    }
    private List<int> _pickFoodId;
    private PickFoodInfo _curPickInfo;
    private LayerMask _foodLayerMask;
    
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handle = new CompositeDisposable();
        _pickFoodId = new(10);

    }

    public void OnEnter(object param)
    {
        UIManager.Instance.OpenUI(UIEnum.KitchenWindow, (uiBase) =>
        {
            _kitchenWindow = uiBase as KitchenWindow;
            _kitchenWindow.StartCook = StartCook;
            _kitchenWindow.XBtn_pick.OnClick.Subscribe(checkPickFood).AddTo(_handle);
            _kitchenWindow.XBtn_cancel.OnClick.Subscribe(cancelPickFood).AddTo(_handle);
            _kitchenWindow.AddFoodCB = addFood;
            recalucate();
        }, null);
        
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Kitchen);
        _restaurant.ShowKitchen();
        
        _mainCamera = Camera.main;
        
        _foodLayerMask = 1<<6;
        _curPickInfo = new PickFoodInfo();
        
        _restaurant.GetAllFoodObjectId(ref _pickFoodId);
    }

    public void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow)||Input.GetKeyDown(KeyCode.S))
        {
            _machine.ChangeState<WaitStateNode>();
        }
    }

    public void OnExit()
    {
        UIManager.Instance.CloseUI(UIEnum.KitchenWindow);
        _handle?.Clear();
        _restaurant.HideKitchen();
        
        _curPickInfo.PickFood = null;
        _curPickInfo.PickIndex = -1;
        
        _pickFoodId.Clear();
    }

    private void SwitchToProduce(int recipeId)
    {
        
        var data = new PickFoodAndTools
        {
            MenuId = recipeId,
        };
        
        data.CookFoods = new List<ItemTableData>(5);
        foreach (var id in _pickFoodId)
        {
            data.CookFoods.Add(new ItemTableData(){Id = id,Num = 1});
        }
        
        data.QTESets = new HashSet<int>();
        foreach (var one in _restaurant.Flavors)
        {
            if (one.IsEnable)
            {
                data.QTESets.Add((int)one.Tag);
            }
        }
        _restaurant.ResetFoodOutline();
        _machine.ChangeState<ProduceStateNode>(data);
    }
    
    private void StartCook(int recipeId,bool isFull)
    {
        if (isFull)
        {//跳转到做菜
            SwitchToProduce(recipeId);
            return;
        }
        
        //1 尝试补全材料
        List<int> lack = new List<int>(5);
        var tb = DataProviderModule.Instance.GetMenuInfo(recipeId);
        foreach (var needFoodId in tb.RelatedMaterial)
        {
            var id = needFoodId;
            if (_pickFoodId.Count(x => x == id) <= 0)
            {
                lack.Add(needFoodId);
            }
        }
        var isEnough = true;
        foreach (var t in lack)
        {
            isEnough = UserInfoModule.Instance.IsEnoughItem(t, 1);
            if (!isEnough) break;
        }
        
        if (isEnough)
        {//先添加原材料
            for (int i = 0; i < lack.Count; i++)
            {
                addFood(lack[i]);
            }
            SwitchToProduce(recipeId);
        }
        else
        {//2 提示材料不足
            var data = new TipCommonData();
            data.tipstr = "材料不足";
            UIManager.Instance.CreateTip<TipCommon>(data).Forget();
        }
    }
    
    private void checkPickFood(PointerEventData param)
    {
        Ray ray = _mainCamera.ScreenPointToRay(param.position);
        // 执行射线检测
        if (Physics.Raycast(ray, out var hit, _foodLayerMask))
        {
            // 检测到碰撞
            if (!hit.collider.CompareTag("BasketFood")) return;
            
            OutlineControl tmp;
            var index = _restaurant.CheckPickFood(hit.transform,out tmp);

            if (tmp == null) return;
            
            if (_curPickInfo.PickFood != null && _curPickInfo.PickFood != tmp)
            {
                _curPickInfo.PickFood.HideOutline();
                _curPickInfo.PickFood = tmp;
                _curPickInfo.PickFood.ShowOutline();
                _curPickInfo.PickIndex = index;
                _kitchenWindow.showCancelBtn(_curPickInfo.PickFood.transform);
            }
            else if (_curPickInfo.PickFood == null && tmp != null)
            {
                _curPickInfo.PickFood = tmp;
                _curPickInfo.PickFood.ShowOutline();
                _curPickInfo.PickIndex = index;
                _kitchenWindow.showCancelBtn(_curPickInfo.PickFood.transform);
            }
            else if (tmp == _curPickInfo.PickFood)
            {
                _curPickInfo.PickFood.HideOutline();
                _curPickInfo.PickFood = null;
                _curPickInfo.PickIndex = -1;
                _kitchenWindow.hideCancelBtn();
            }
            
        }
    }
    
    private void cancelPickFood(PointerEventData param)
    {
        if (_curPickInfo.PickFood == null)
        {
            return;
        }
        _restaurant.RemoveFoodObject(_curPickInfo.PickFood.gameObject);
        _curPickInfo.PickFood = null;
        _kitchenWindow.hideCancelBtn();
        _pickFoodId.RemoveAt(_curPickInfo.PickIndex);
        recalucate();
    }

    private void addFood(int foodId)
    {
        _restaurant.AddFoodObject(foodId);
        _pickFoodId.Add(foodId);

        recalucate();
    }

    private void recalucate()
    {
        if (_pickFoodId.Count <= 0)
        {
            _kitchenWindow.HidePreviewRecipe();
            return;   
        }
        
        var previewRecipe = GlobalFunctions.SearchRecipe(_pickFoodId);
        List<KitchenWindow.PreviewRecipe> recipes = new List<KitchenWindow.PreviewRecipe>();
        foreach (var recipeId in previewRecipe)
        {
            var tb = DataProviderModule.Instance.GetMenuInfo(recipeId);
            var isFull = checkRecipeFull(tb.RelatedMaterial);   
            recipes.Add(new KitchenWindow.PreviewRecipe(){IsFull = isFull,RecipeId = recipeId});
        }
        _kitchenWindow.RefreshPreviewRecipe(recipes);
    }

    private bool checkRecipeFull(List<int> material)
    {
        var isFull = _pickFoodId.Count >= material.Count;
        if (!isFull)
        {
            return false;
        }
        
        foreach (var one in material)
        {
            if (_pickFoodId.Count(x => x == one) <= 0)
            {
                return false;
            }
        }
        return true;
    }
    
}
