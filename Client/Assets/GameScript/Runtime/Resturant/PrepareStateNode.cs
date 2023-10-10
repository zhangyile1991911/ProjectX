using System.Collections;
using System.Collections.Generic;
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
        }, null);
        EventModule.Instance.StartCookSub.Subscribe(StartCook).AddTo(_handle);
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Kitchen);
        _restaurant.ShowKitchen();
    }

    public void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow)||Input.GetKeyDown(KeyCode.S))
        {
            _machine.ChangeState<WaitStateNode>();
        }

        if (!_isMoving)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                _isMoving = true;
                _kitchenWindow.SwitchMode(KitchenWindow.KitchenMode.PrepareCook);
                _restaurant.KitchenCamera.transform.DOMoveX(25f, 0.5f).OnComplete(() => { _isMoving = false;});
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                _isMoving = true;
                _restaurant.KitchenCamera.transform.DOMoveX(0f, 0.5f).OnComplete(() =>
                {
                    _isMoving = false;
                    _kitchenWindow.SwitchMode(KitchenWindow.KitchenMode.PickFoodMode);
                });
            }
        }
        
        
    }

    public void OnExit()
    {
        UIManager.Instance.CloseUI(UIEnum.KitchenWindow);
        _handle?.Clear();
        _restaurant.HideKitchen();
        
    }

    private void StartCook(PickFoodAndTools foo)
    {
        _machine.ChangeState<ProduceStateNode>(foo);
    }
}
