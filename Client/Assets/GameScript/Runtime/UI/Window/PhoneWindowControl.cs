
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class PhoneWindow : UIWindow
{
    private List<PhoneAppWidget> _appList;
    private Clocker _clocker;
    private UIManager _uiManager;
    private NewsAppWidget NewsApp;
    private UIComponent curRunApp;
    public override async void OnCreate()
    {
        _appList = new List<PhoneAppWidget>(4);
        _uiManager = UniModule.GetModule<UIManager>();
        
        var newsAPP = await _uiManager.CreateUIComponent<PhoneAppWidget>(null,Tran_AppGroup,this);
        newsAPP.SetAPPInfo("新闻",OnClickNewsApp);
        _appList.Add(newsAPP);
        
        
    }
    
    public override void OnDestroy()
    {
        foreach (var one in _appList)
        {
            one.OnHide();
            one.OnDestroy();
        }
        _appList.Clear();
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
                _clocker.AddMinute(1);
            }
        }).AddTo(handles);
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
    
    private async void OnClickNewsApp()
    {
        Debug.Log("点击了新闻应用");
        if (NewsApp == null)
        {
            _clocker.AddMinute(1);
            NewsApp = await _uiManager.CreateUIComponent<NewsAppWidget>(null,Tran_AppRun,this);
            curRunApp = NewsApp;
        }
        else
        {
            NewsApp.OnShow(null);
            _clocker.AddSecond(10);
        }
        
    }
}