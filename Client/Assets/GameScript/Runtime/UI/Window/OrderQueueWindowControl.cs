using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class OrderQueueWindow : UIWindow
{
    private EventModule _eventModule;
    private const float top_padding = 10f;
    private const float left_padding = 20f;
    public override void OnCreate()
    {
        base.OnCreate();
        _mealOrderList = new List<MealOrderComponent>(10);

    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        _eventModule = UniModule.GetModule<EventModule>();
        _eventModule.OrderMealSub.Subscribe(handleOrderMeal).AddTo(handles);
        _eventModule.CharacterLeaveSub.Subscribe(RemoveCharacterOrder).AddTo(handles);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    private List<MealOrderComponent> _mealOrderList;
    private void handleOrderMeal(OrderMealInfo mealInfo)
    {
        switch (mealInfo.operation)
        {
            case 0:
                newOrderMeal(mealInfo);
                break;
            case 1:
                delOderMeal(mealInfo);
                break;
        }
        
       
    }

    private async void newOrderMeal(OrderMealInfo mealInfo)
    {
        var uiManager = UIManager.Instance;
        var orderPrefab = await uiManager.CreateUIComponent<MealOrderComponent>(null,Tran_Queue,this);
        Vector3 startPos = new Vector3(
            uiManager.RootCanvas.pixelRect.width, 
            0,
            0);
        Vector3 endPos = new Vector3();
        endPos.x = _mealOrderList.Count * left_padding;
        orderPrefab.SetMealOrderInfo(mealInfo,startPos,endPos);
        // Debug.Log($"uiManager.RootCanvas.pixelRect.width ={uiManager.RootCanvas.pixelRect.width}");
        // Debug.Log($"uiManager.RootCanvas.pixelRect.height ={uiManager.RootCanvas.pixelRect.height}");
        _mealOrderList.Add(orderPrefab);
    }

    private void delOderMeal(OrderMealInfo mealInfo)
    {
        foreach (var one in _mealOrderList)
        {
            if (one.OrderMealInfo.Equals(mealInfo))
            {
                _mealOrderList.Remove(one);
                break;
            }
        }
    }

    private void RemoveCharacterOrder(RestaurantCharacter character)
    {
        var uiManager = UniModule.GetModule<UIManager>();
        for (int i = _mealOrderList.Count - 1; i >= 0; i--)
        {
            var one = _mealOrderList[i];
            if (one.OrderMealInfo.Customer == character)
            {
                _mealOrderList.RemoveAt(i);
                uiManager.DestroyUIComponent(one);
            }
        }
    }
}