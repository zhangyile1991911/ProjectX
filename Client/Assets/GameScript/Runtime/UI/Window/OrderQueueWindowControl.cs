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
    private const float top_padding = -15f;
    private const float left_padding = 15f;
    public override void OnCreate()
    {
        base.OnCreate();
        // _mealOrderList = new List<MealOrderComponent>(10);
        // EventModule.Instance.OrderMealSub.Subscribe(handleOrderMeal).AddTo(uiTran);
        // EventModule.Instance.CharacterLeaveSub.Subscribe(RemoveCharacterOrder).AddTo(uiTran);
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        // clearAllOrderMeal();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    // private void clearAllOrderMeal()
    // {
    //     foreach (var one in _mealOrderList)
    //     {
    //            UIManager.Instance.DestroyUIComponent(one);
    //     }
    //     _mealOrderList.Clear();
    // }
    //
    // private List<MealOrderComponent> _mealOrderList;
    // private void handleOrderMeal(OrderMealInfo mealInfo)
    // {
    //     switch (mealInfo.operation)
    //     {
    //         case 0:
    //             newOrderMeal(mealInfo);
    //             break;
    //         case 1:
    //             delOderMeal(mealInfo);
    //             break;
    //     }
    //     
    //    
    // }
    //
    // private async void newOrderMeal(OrderMealInfo mealInfo)
    // {
    //     var uiManager = UIManager.Instance;
    //     var orderPrefab = await uiManager.CreateUIComponent<MealOrderComponent>(null,uiTran,this);
    //     // Vector3 startPos = new Vector3(
    //     //     uiManager.RootCanvas.pixelRect.width, 
    //     //     top_padding,
    //     //     0);
    //     // Vector3 endPos = new Vector3();
    //     // endPos.x = (_mealOrderList.Count+1) * left_padding + _mealOrderList.Count * 100f;
    //     // endPos.y = top_padding;
    //     // Debug.Log($"newOrdermeal {endPos}");
    //     // orderPrefab.SetMealOrderInfo(mealInfo,startPos,endPos);
    //     _mealOrderList.Add(orderPrefab);
    // }
    //
    // private void delOderMeal(OrderMealInfo mealInfo)
    // {
    //     RemoveCharacterOrder(mealInfo.CharacterId);
    // }
    //
    // private void RemoveCharacterOrder(RestaurantRoleBase character)
    // {
    //     RemoveCharacterOrder(character.CharacterId);
    // }
    //
    // private void RemoveCharacterOrder(int characterId)
    // {
    //     var uiManager = UniModule.GetModule<UIManager>();
    //     for (int i = _mealOrderList.Count - 1; i >= 0; i--)
    //     {
    //         var one = _mealOrderList[i];
    //         if (one.OrderMealInfo.CharacterId == characterId)
    //         {
    //             _mealOrderList.RemoveAt(i);
    //             uiManager.DestroyUIComponent(one);
    //         }
    //     }
    //
    //     Vector3 newPos = new Vector3();
    //     for (int i = 0; i < _mealOrderList.Count; i++)
    //     {
    //         var one = _mealOrderList[i];
    //         newPos.x = (i+1) * left_padding + i * 100f;
    //         newPos.y = top_padding;
    //         one.RearrangeOrderInfo(newPos);
    //     }
    // }
    
}