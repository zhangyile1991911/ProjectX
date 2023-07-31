using System;
using System.Collections;
using System.Collections.Generic;
using cfg.food;
using Cysharp.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class BarbecueWindow : UIWindow,CookWindowUI
{
    public Action ClickStart;
    public Action ClickFinish;
    // public int GameDuration;
    
    private int _remainDuration;
    private IDisposable _counterHandle;
    private CookWindowParamData _openData;
    private CookResultWidget _cookResultWidget;
    private RectTransform rect_qteArea;
    public override void OnCreate()
    {
        base.OnCreate();
        _cookResultWidget = new CookResultWidget(Ins_CookResultWidget.gameObject, this);
        rect_qteArea = Tran_QTEArea.GetComponent<RectTransform>();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        
        _openData = openParam as CookWindowParamData;

        Slider_Progress.maxValue = _openData.Difficulty.duration;
        Slider_Progress.value = 0;
        
        Btn_start.gameObject.SetActive(true);
        Btn_start.OnClickAsObservable().Subscribe(clickStart).AddTo(handles);
        _cookResultWidget.Btn_Close.OnClickAsObservable().Subscribe(clickFinish).AddTo(handles);
        _cookResultWidget.OnHide();
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
        
        Btn_start.gameObject.SetActive(false);

        _counterHandle?.Dispose();
        _counterHandle = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(Counter).AddTo(handles);
        _remainDuration = 0;
    }

    private void clickFinish(Unit param)
    {
        ClickFinish?.Invoke();
    }

    private void Counter(long param)
    {
        _remainDuration += 1;
        if (_remainDuration >= _openData.Difficulty.duration)
        {
            _counterHandle?.Dispose();
            _counterHandle = null;
        }

        Slider_Progress.value = _remainDuration;
        Txt_timer.text = ZString.Format("{0}", _openData.Difficulty.duration - _remainDuration);
    }
    

    public void ShowGameOver(CookResult cookResult)
    {
        _cookResultWidget.OnShow(null);
        _cookResultWidget.ShowGameOver(cookResult);
    }
    
    private List< QTEInfoRecord> qteList;
    public async void LoadQTEConfigTips(List<qte_info> tbQTEInfos)
    {
        qteList ??= new List<QTEInfoRecord>();
        
        for (int i = 0; i < qteList.Count; i++)
        {
            qteList[i].tip.OnHide();    
        }
        
        for (int i = 0; i < tbQTEInfos.Count; i++)
        {
            if (i >= qteList.Count)
            {
                qteList.Add(new QTEInfoRecord());    
            }

            var tb = tbQTEInfos[i];
            
            var info = DataProviderModule.Instance.GetQTEInfo(tb.QteId);
            if(info.Id == qteList[i].Id)continue;

            qteList[i].Id = tb.QteId;
            if (qteList[i].tip == null)
            {
                qteList[i].tip = await UIManager.Instance.CreateUIComponent<QTETips>(null,Tran_QTEArea,
                    this);
            }

            qteList[i].tip.Txt_tips.text = info.Desc;
            qteList[i].tip.uiRectTran.anchoredPosition = new Vector2(0,rect_qteArea.sizeDelta.y * tb.StartArea); 
            Debug.Log($"anchoredPosition = {qteList[i].tip.uiRectTran.anchoredPosition}");
            // qteList[i].tip.OnShow(null);
            qteList[i].tip.OnHide();
        }
    }

    public void ShowQTETip(int qteId)
    {
        foreach (var one in qteList)
        {
            if (one.Id == qteId)
            {
                one.tip.OnShow(null);
                break;
            }
        }
    }

    public void HideQTETip(int qteId)
    {
        foreach (var one in qteList)
        {
            if (one.Id == qteId)
            {
                one.tip.OnHide();
                break;
            }
        }
    }
    
    
}