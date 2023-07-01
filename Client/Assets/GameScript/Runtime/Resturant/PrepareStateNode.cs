using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class PrepareStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private CompositeDisposable _handle;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handle = new CompositeDisposable();
    }

    public void OnEnter(object param)
    {
        UIManager.Instance.OpenUI(UIEnum.KitchenWindow, (uiBase)=>{}, null);
        EventModule.Instance.StartCookSub.Subscribe(StartCook).AddTo(_handle);
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.A))
        {
            _machine.ChangeState<WaitStateNode>();
        }
    }

    public void OnExit()
    {
        UIManager.Instance.CloseUI(UIEnum.KitchenWindow);
        _handle?.Clear();
    }

    private void StartCook(PickFoodAndTools foo)
    {
        _machine.ChangeState<ProduceStateNode>(foo);
    }
}
