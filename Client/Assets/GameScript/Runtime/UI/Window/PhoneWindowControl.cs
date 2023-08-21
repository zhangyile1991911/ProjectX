﻿
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class PhoneWindow : UIWindow
{
    public bool IsDebug = false;
    private List<PhoneAppWidget> _appIconList;
    private Clocker _clocker;
    private UIManager _uiManager;
    private List<BaseAppWidget> _appList;
    // private NewsAppWidget NewsApp;
    private UIComponent CurRunApp
    {
        get => _curRunApp;
        set
        {
            _curRunApp = value;
            XBtn_home.gameObject.SetActive(_curRunApp != null);
        }
    }
    private UIComponent _curRunApp;
    public override async void OnCreate()
    {
        _appIconList = new List<PhoneAppWidget>(4);
        _uiManager = UniModule.GetModule<UIManager>();
        
        _appList = new List<BaseAppWidget>(10);
        //根据功能开放
        var newsAPP = await _uiManager.CreateUIComponent<PhoneAppWidget>(null,Tran_AppGroup,this);
        newsAPP.SetAPPInfo("新闻",OnClickNewsApp);
        _appIconList.Add(newsAPP);    
        
        var airplane = await _uiManager.CreateUIComponent<PhoneAppWidget>(null,Tran_AppGroup,this);
        airplane.SetAPPInfo("小飞机",OnClickAirPlane);
        _appIconList.Add(airplane);    
        
    }
    
    public override void OnDestroy()
    {
        foreach (var one in _appIconList)
        {
            one.OnHide();
            one.OnDestroy();
        }
        _appIconList.Clear();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        _clocker = UniModule.GetModule<Clocker>();
        XBtn_home.OnClick.Subscribe(clickHome).AddTo(handles);
        uiGo.UpdateAsObservable().Subscribe(UpdateApp).AddTo(handles);
        // _clocker.Topic.Subscribe(nowms =>
        // {
        //     
        // }).AddTo(handles);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }

    private void clickHome(PointerEventData param)
    {
        if (CurRunApp != null && CurRunApp.IsActive)
        {
            CurRunApp.OnHide();
            CurRunApp = null;
            CostTimeOnOpenApp(60);
            Tran_AppGroup.gameObject.SetActive(true);
        }
    }
    

    private void UpdateApp(Unit param)
    {
        // Debug.Log("UpdateApp");
        CurRunApp?.OnUpdate();
    }
    
    private async void OnClickNewsApp()
    {
        Debug.Log("点击了新闻应用");
        var newsApp = _appList.Find(one=>one.WidgetType==BaseAppWidget.AppType.News);
        var costSecond = 0;
        if (newsApp == null)
        {
            newsApp = await _uiManager.CreateUIComponent<NewsAppWidget>(null,Tran_AppRun,this);
            _appList.Add(newsApp);
            costSecond = 60;
        }
        else
        {
            newsApp.OnShow(null);
            costSecond = 10;
        }
        Tran_AppGroup.gameObject.SetActive(false);
        CostTimeOnOpenApp(costSecond);
        CurRunApp = newsApp;
    }

    private async void OnClickAirPlane()
    {
        Debug.Log("点击了小飞机");
        var airPlaneApp = _appList.Find(one=>one.WidgetType==BaseAppWidget.AppType.AirPlane);
        var costSecond = 0;
        if (airPlaneApp == null)
        {
            airPlaneApp = await _uiManager.CreateUIComponent<AirplaneAppWidget>(null,Tran_AppRun,this);
            _appList.Add(airPlaneApp);
            costSecond = 60;
        }
        else
        {
            costSecond = 10;
            airPlaneApp.OnShow(null);
        }
        Tran_AppGroup.gameObject.SetActive(false);
        
        CostTimeOnOpenApp(costSecond);
        
        
        CurRunApp = airPlaneApp;
    }

    private void CostTimeOnOpenApp(int costSecond)
    {
        if (!IsDebug)
        {
            _clocker.AddSecond(costSecond);
        }
    }
    
    
}