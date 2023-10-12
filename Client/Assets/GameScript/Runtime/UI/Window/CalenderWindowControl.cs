
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using cfg.common;
using Codice.Client.BaseCommands;
using Cysharp.Threading.Tasks;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class CalenderWindow : UIWindow
{

    public Action ContinueCB;
    private CalenderSheetWidget prevSheet;
    private CalenderSheetWidget nowSheet;
    private bool canContinue;
    private Tweener weatherTweener;
    public override void OnCreate()
    {
        base.OnCreate();
        prevSheet = new CalenderSheetWidget(Ins_PreviouDate.gameObject,this);
        nowSheet = new CalenderSheetWidget(Ins_NowDate.gameObject, this);
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        //Debug.Log("CalenderWindow OnShow");
        //var nowDate = Clocker.Instance.NowDateTime;
        //var prevDate = Clocker.Instance.PrevDateTime;
        //Txt_date.text = $"{(int)nowDate.Season}月{nowDate.Day}日\n{Clocker.Instance.WeekDayStr()}";
        
        //nowSheet.SetDate(nowDate.Season,(int)nowDate.Day,nowDate.DayOfWeek);
        //canContinue = true;
        if (!canContinue)
        {
            //prevSheet.SetDate(prevDate.Season,(int)prevDate.Day,prevDate.DayOfWeek);
            //动画
            doPassDayAnimation();
            //canContinue = false;
        }
        doWeatherAnimation(Weather.Sunshine);
        Btn_continue.OnClickAsObservable().Where(_=>canContinue).Subscribe(clickContinue).AddTo(handles);
    }

    public void Refresh()
    {
        //Debug.Log("CalenderWindow Refresh");
        var nowDate = Clocker.Instance.NowDateTime;
        var prevDate = Clocker.Instance.PrevDateTime;
        Txt_date.text = $"{(int)prevDate.Season}月{prevDate.Day}日\n{prevDate.WeekDayStr()}";
        
        nowSheet.SetDate(nowDate.Season,(int)nowDate.Day,nowDate.WeekDayStr());
        if (nowDate.Day != prevDate.Day)
        {
            canContinue = false;
            prevSheet.SetDate(prevDate.Season,(int)prevDate.Day,prevDate.WeekDayStr());
        }
    }

    public override void OnHide()
    {
        base.OnHide();
        ContinueCB = null;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void doPassDayAnimation()
    {
        var seq = DOTween.Sequence();
        seq.PrependInterval(0.5f);
        seq.Append(prevSheet.uiRectTran.DORotate(new Vector3(0,0,-30f),2f));
        seq.Join(prevSheet.uiRectTran.DOMove(new Vector3(-800f,-1100f,0f),5f));
        seq.InsertCallback(0.5f,() =>
        {
            canContinue = true;
            var nowDate = Clocker.Instance.NowDateTime;
            Txt_date.text = $"{(int)nowDate.Season}月{nowDate.Day}日\n{nowDate.WeekDayStr()}";
        });
    }

    private void doWeatherAnimation(cfg.common.Weather weather)
    {
        weatherTweener?.Kill(false);
        switch (weather)
        {
            case Weather.Sunshine:
                weatherTweener = Img_weather.rectTransform.DOLocalRotate(
                    new Vector3(0, 0, -360f), 15f,RotateMode.LocalAxisAdd).
                    SetEase(Ease.Linear).
                    SetLoops(-1);
                break;
            case Weather.Rain:
                break;
            case Weather.Snow:
                break;
            case Weather.Typhoon:
                break;
            case Weather.SnowStorm:
                break;
        }
    }

    private void clickContinue(Unit param)
    {
        ContinueCB?.Invoke();
    }
    
}