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
public partial class FryingFoodWindow : UIWindow
{
    private RectTransform rect_qteArea;
    public override void OnCreate()
    {
        base.OnCreate();
        rect_qteArea = Tran_QTEArea.GetComponent<RectTransform>();
        
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        Btn_Start.OnClickAsObservable().Subscribe(ClickStart).AddTo(handles);
        Btn_Close.OnClickAsObservable().Subscribe(ClickFinish).AddTo(handles);
        Tran_Result.gameObject.SetActive(false);
    }

    public override void OnHide()
    {
        base.OnHide();
        Btn_Start.gameObject.SetActive(true);
        Slider_Progress.gameObject.SetActive(false);
        Slider_Temperature.gameObject.SetActive(false);
        Tran_QTEArea.gameObject.SetActive(false);
        Tran_Result.gameObject.SetActive(false);
        handles.Clear();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    class QTEInfo
    {
        public int Id;
        public QTETips tip;
    }

    private List< QTEInfo> qteList;
    public async void LoadQTEConfigTips(List<qte_info> tbQTEInfos)
    {
        qteList ??= new List<QTEInfo>();
        
        for (int i = 0; i < qteList.Count; i++)
        {
            qteList[i].tip.OnHide();    
        }
        
        for (int i = 0; i < tbQTEInfos.Count; i++)
        {
            if (i >= qteList.Count)
            {
                qteList.Add(new QTEInfo());    
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

    public void SetDifficulty(FryingDifficulty difficulty)
    {
        Slider_Temperature.maxValue = difficulty.maxTemperature;
        Slider_Temperature.minValue = 0;
        
        Slider_Progress.maxValue = difficulty.finishValue;
        Slider_Progress.minValue = 0;
        
        var min = difficulty.temperatureArea.x;
        var max = difficulty.temperatureArea.y;
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
    
    public void ShowGameOver(CookResult cookResult)
    {
        //标题
        var tbMenu = DataProviderModule.Instance.GetMenuInfo(cookResult.menuId);
        Txt_Menu.name = tbMenu.Name; 
        //评分
        StringBuilder sb = new StringBuilder();
        sb.Append(ZString.Format("完成度\t\t{0}\n", cookResult.CompletePercent));
        foreach (var pair in cookResult.QTEResult)
        {
            var tb = DataProviderModule.Instance.GetQTEInfo(pair.Key);
            sb.Append(ZString.Format("{0}\t\t\t{1}\n",tb.Name,pair.Value?"成功":"失败"));
        }

        foreach (var tag in cookResult.Tags)
        {
            var tb = DataProviderModule.Instance.DataBase.TbFlavour.DataMap[(int)tag];
            sb.Append(" ");
            sb.Append(tb.Desc);
        }

        sb.Append("\n");
        Txt_Result.text = sb.ToString();
        Tran_Result.gameObject.SetActive(true);
    }

    public void ShowQteTips(int qteId)
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
    
    private void ClickStart(Unit param)
    {
        EventModule.Instance.CookGameStartTopic.OnNext(true);
        Btn_Start.gameObject.SetActive(false);
        Slider_Progress.gameObject.SetActive(true);
        Slider_Temperature.gameObject.SetActive(true);
        Tran_QTEArea.gameObject.SetActive(true);
        
    }
    
    private void ClickFinish(Unit param)
    {
        EventModule.Instance.ExitKitchenTopic.OnNext(Unit.Default);
        Btn_Start.gameObject.SetActive(true);
        Slider_Progress.gameObject.SetActive(false);
        Slider_Temperature.gameObject.SetActive(false);
        Tran_QTEArea.gameObject.SetActive(false);

        for (int i = 0;i < qteList.Count;i++)
        {
            qteList[i].tip.OnHide();    
        }
    }

    private void UpdateProgressSlider(float progress)
    {
        Slider_Progress.value = progress;
    }

    private void UpdateTemperatureSlider(float temperature)
    {
        Slider_Temperature.value = temperature;
    }

}