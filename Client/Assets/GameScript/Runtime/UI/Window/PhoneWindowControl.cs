
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
    private ReactiveProperty<bool> _isLoading;
    public override async void OnCreate()
    {
        _appIconList = new List<PhoneAppWidget>(4);
        _uiManager = UniModule.GetModule<UIManager>();

        _isLoading = new ReactiveProperty<bool>(false);
        _appList = new List<BaseAppWidget>(10);
        //todo 根据功能开放
        var newsAPP = await _uiManager.CreateUIComponent<PhoneAppWidget>(null,Tran_AppGroup,this);
        newsAPP.SetAPPInfo(10001);
        _appIconList.Add(newsAPP);
        
        var airplane = await _uiManager.CreateUIComponent<PhoneAppWidget>(null,Tran_AppGroup,this);
        airplane.SetAPPInfo(10002);
        _appIconList.Add(airplane);
        
        var weather = await _uiManager.CreateUIComponent<PhoneAppWidget>(null,Tran_AppGroup,this);
        weather.SetAPPInfo(10003);
        _appIconList.Add(weather);
        _isLoading.Value = true;
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
        handles = new CompositeDisposable(10);
        uiGo.SetActive(true);
        _isLoading.Subscribe(_=>
        {
            bindEvent();
        }).AddTo(handles);
    }

    public override void OnHide()
    {
        uiGo.SetActive(false);
        handles.Dispose();
        handles.Clear();
    }

    public override void OnUpdate()
    {
        
    }

    private void bindEvent()
    {
        uiGo.UpdateAsObservable().Subscribe(UpdateApp).AddTo(handles);
        _clocker = UniModule.GetModule<Clocker>();
        XBtn_home.OnClick.Subscribe(clickHome).AddTo(handles);
        foreach (var app in _appIconList)
        {
            switch (app.APPID)
            {
                case 10001:
                    app.XBtn_App.OnClick.Subscribe(OnClickNewsApp).AddTo(handles);
                    break;
                case 10002:
                    app.XBtn_App.OnClick.Subscribe(OnClickAirPlane).AddTo(handles);
                    break;
                case 10003:
                    app.XBtn_App.OnClick.Subscribe(OnClickWeather).AddTo(handles);
                    break;
            }
        }
        
    }


    public void ReleaseAllApp()
    {
        _curRunApp = null;
        for (int i = 0; i < _appList.Count; i++)
        {
            GameObject.Destroy(_appList[i].uiGo);
        }
        _appList.Clear();
        Tran_AppGroup.gameObject.SetActive(true);
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
    
    private async void OnClickNewsApp(PointerEventData param)
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

    private async void OnClickAirPlane(PointerEventData param)
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
    
    private async void OnClickWeather(PointerEventData param)
    {
        Debug.Log("点击了天气");
        var weatherApp = _appList.Find(one=>one.WidgetType==BaseAppWidget.AppType.Weather);
        var costSecond = 0;
        if (weatherApp == null)
        {
            weatherApp = await _uiManager.CreateUIComponent<WeatherApp>(null,Tran_AppRun,this);
            _appList.Add(weatherApp);
            costSecond = 60;
        }
        else
        {
            costSecond = 10;
            weatherApp.OnShow(null);
        }
        Tran_AppGroup.gameObject.SetActive(false);
        
        CostTimeOnOpenApp(costSecond);
        
        CurRunApp = weatherApp;
    }

    private void CostTimeOnOpenApp(int costSecond)
    {
        if (!IsDebug)
        {
            _clocker.AddSecond(costSecond);
        }
    }
    
    
}