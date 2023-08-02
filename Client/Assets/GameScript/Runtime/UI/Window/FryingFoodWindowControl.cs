using System;
using System.Collections.Generic;
using System.Text;
using cfg.food;
using Cysharp.Text;
using UniRx;
using UnityEngine;
using YooAsset;


/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class FryingFoodWindow : UIWindow,CookWindowUI
{
    public Action ClickStart;
    public Action ClickFinish;
    private RectTransform rect_qteArea;
    // private StateMachine _stateMachine;
    private CookResultWidget _resultWidget;
    public override void OnCreate()
    {
        base.OnCreate();
        rect_qteArea = Tran_QTEArea.GetComponent<RectTransform>();
        _resultWidget = new CookResultWidget(Ins_CookResultWidget.gameObject, this);
        
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        Btn_Start.OnClickAsObservable().Subscribe(clickStart).AddTo(handles);
        _resultWidget.Btn_Close.OnClickAsObservable().Subscribe(clickFinish).AddTo(handles);
        
        _resultWidget.OnHide();
        // var tmp = openParam as CookWindowParamData;
        // _stateMachine = tmp.StateMachine;
    }

    public override void OnHide()
    {
        base.OnHide();
        // _stateMachine = null;
        Btn_Start.gameObject.SetActive(true);
        Slider_Progress.gameObject.SetActive(false);
        Slider_Temperature.gameObject.SetActive(false);
        Tran_QTEArea.gameObject.SetActive(false);
        handles.Clear();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
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
                    this,false);
            }

            qteList[i].tip.Txt_tips.text = info.Desc;
            qteList[i].tip.uiRectTran.anchoredPosition = new Vector2(0,rect_qteArea.sizeDelta.y * tb.StartArea); 
            Debug.Log($"anchoredPosition = {qteList[i].tip.uiRectTran.anchoredPosition}");
            qteList[i].tip.OnShow(null);
        }
    }
    
    

    public void SetDifficulty(RecipeDifficulty difficulty)
    {
        var tmp = difficulty as FryingDifficulty;
        Slider_Temperature.maxValue = tmp.maxTemperature;
        Slider_Temperature.minValue = 0;
        Debug.Log($"Slider_Temperature.maxValue = {Slider_Temperature.maxValue}");
        Slider_Progress.maxValue = tmp.finishValue;
        Slider_Progress.minValue = 0;
        
        var min = tmp.temperatureArea.x;
        var max = tmp.temperatureArea.y;
        Img_Temperature.material.SetFloat("_low",min);
        Img_Temperature.material.SetFloat("_medium",max);
    }

    public void SetProgressListener(IReadOnlyReactiveProperty<float> progressVal)
    {
        progressVal.Subscribe(UpdateProgressSlider).AddTo(handles);
    }

    public void SetTemperatureListener(IReadOnlyReactiveProperty<float> temperatureVal)
    {
        temperatureVal.Subscribe(UpdateTemperatureSlider).AddTo(handles);
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
    
    private void clickStart(Unit param)
    {
        // EventModule.Instance.CookGameStartTopic.OnNext(true);
        Btn_Start.gameObject.SetActive(false);
        Slider_Progress.gameObject.SetActive(true);
        Slider_Temperature.gameObject.SetActive(true);
        Tran_QTEArea.gameObject.SetActive(true);
        ClickStart?.Invoke();
    }
    
    private void clickFinish(Unit param)
    {
        // EventModule.Instance.ExitKitchenTopic.OnNext(Unit.Default);
        // _stateMachine.ChangeState<PrepareStateNode>();
        
        Btn_Start.gameObject.SetActive(true);
        Slider_Progress.gameObject.SetActive(false);
        Slider_Temperature.gameObject.SetActive(false);
        Tran_QTEArea.gameObject.SetActive(false);

        for (int i = 0;i < qteList.Count;i++)
        {
            qteList[i].tip.OnHide();    
        }
        ClickFinish?.Invoke();
    }
    

    private void UpdateProgressSlider(float progress)
    {
        Slider_Progress.value = progress;
    }

    private void UpdateTemperatureSlider(float temperature)
    {
        Debug.Log($"FryingFoodWindowControl UpdateTemperatureSlider = {temperature}");
        Slider_Temperature.value = temperature;
    }

    public void ShowGameOver(CookResult cookResult)
    {
        _resultWidget.OnShow(null);
        _resultWidget.ShowGameOver(cookResult);
    }

}