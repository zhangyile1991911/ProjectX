using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class BarbecueWindow : UIWindow
{
    public Action ClickStart;
    public int GameDuration;
    
    private int _remainDuration;
    private IDisposable _counterHandle;
    public override void OnCreate()
    {
        base.OnCreate();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        Btn_start.OnClickAsObservable().Subscribe(clickStart).AddTo(handles);
    }

    public override void OnHide()
    {
        base.OnHide();
        ClickStart = null;
        _counterHandle?.Dispose();
        _counterHandle = null;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void clickStart(Unit param)
    {
        ClickStart?.Invoke();
        HideGameOver();
        
        _counterHandle?.Dispose();
        _counterHandle = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(Counter);
        _remainDuration = GameDuration;
    }

    private void Counter(long param)
    {
        _remainDuration -= 1;
        if (_remainDuration < 0)
        {
            _counterHandle?.Dispose();
            _counterHandle = null;
            ShowGameOver();
        }
        Txt_timer.text = ZString.Format("{0}", _remainDuration);
    }

    private void HideGameOver()
    {
        
    }
    
    private void ShowGameOver()
    {
        
    }
    
}