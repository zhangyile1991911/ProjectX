using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CalenderStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private CalenderWindow _calenderWindow;
    private CompositeDisposable _disposable;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _disposable = new CompositeDisposable();
    }

    public void OnEnter(object param = null)
    {
        UIManager.Instance.OpenUI(UIEnum.CalenderWindow, (uiBase) =>
        {
            _calenderWindow = uiBase as CalenderWindow;
            _calenderWindow.XBtn_continue.OnClick.Subscribe(playFlipPage).AddTo(_disposable);
            // _calenderWindow.ContinueCB = () =>
            // {
            //     _machine.ChangeState<WaitStateNode>();
            // };
        }, null);
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {   
            // _calenderWindow.ContinueCB.Invoke();
            // _machine.ChangeState<WaitStateNode>();
            playFlipPage(null);
        }
    }

    private async void playFlipPage(PointerEventData param)
    {
        _calenderWindow.FlipPageAnimation.Play("FlipPage");
        await UniTask.Delay(TimeSpan.FromMilliseconds(1300));
        _machine.ChangeState<WaitStateNode>();
    }

    public void OnExit()
    {
        UIManager.Instance.CloseUI(UIEnum.CalenderWindow);
        _disposable.Dispose();
        _disposable = null;
    }
}
