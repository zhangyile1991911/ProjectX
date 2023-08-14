
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class PhoneWindow : UIWindow
{
    private List<PhoneAppWidget> _appIconList;
    private Clocker _clocker;
    private UIManager _uiManager;
    private List<BaseAppWidget> _appList;
    // private NewsAppWidget NewsApp;
    private UIComponent curRunApp;
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
        airplane.SetAPPInfo("打飞机",OnClickAirPlane);
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
        Btn_Home.OnClickAsObservable().Subscribe(_ =>
        {
            if (curRunApp != null && curRunApp.IsActive)
            {
                curRunApp.OnHide();
                curRunApp = null;
                _clocker.AddMinute(1);
                Tran_AppGroup.gameObject.SetActive(true);
            }
        }).AddTo(handles);
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

    private void UpdateApp(Unit param)
    {
        // Debug.Log("UpdateApp");
        curRunApp?.OnUpdate();
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
        _clocker.AddSecond(costSecond);
        curRunApp = newsApp;
    }

    private async void OnClickAirPlane()
    {
        Debug.Log("点击了打飞机");
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
        
        _clocker.AddSecond(costSecond);
        
        
        curRunApp = airPlaneApp;
    }
    
    
    
}