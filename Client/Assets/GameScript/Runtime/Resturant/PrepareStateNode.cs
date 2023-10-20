using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class PrepareStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private CompositeDisposable _handle;
    private bool _isMoving;
    private KitchenWindow _kitchenWindow;
    // private Camera _mainCamera;
    // private LayerMask _foodLayerMask;
    // private Material _foodOutlineMaterial;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handle = new CompositeDisposable();

    }

    public void OnEnter(object param)
    {
        UIManager.Instance.OpenUI(UIEnum.KitchenWindow, (uiBase) =>
        {
            _kitchenWindow = uiBase as KitchenWindow;
            _kitchenWindow.StartCook = StartCook;
        }, null);
        // EventModule.Instance.StartCookSub.Subscribe(StartCook).AddTo(_handle);
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Kitchen);
        _restaurant.ShowKitchen();
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
        // _mainCamera = null;

    }

    private void StartCook(int recipeId)
    {
        var data = new PickFoodAndTools
        {
            MenuId = recipeId,
        };
        data.QTESets = new HashSet<int>();
        foreach (var one in _restaurant.Flavors)
        {
            if (one.IsEnable)
            {
                data.QTESets.Add((int)one.Tag);
            }
        }
        _machine.ChangeState<ProduceStateNode>(data);
    }
}
