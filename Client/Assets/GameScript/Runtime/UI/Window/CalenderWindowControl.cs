
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using cfg.common;
using Codice.Client.BaseCommands;
using Cysharp.Threading.Tasks;
using YooAsset;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class CalenderWindow : UIWindow
{

    // public Action ContinueCB;
    private CalenderSheetWidget prevSheet;
    private CalenderSheetWidget nowSheet;
    // private bool canContinue;
    // private Tweener weatherTweener;
    public Animation FlipPageAnimation;
    public override void OnCreate()
    {
        base.OnCreate();
        prevSheet = new CalenderSheetWidget(Ins_PreviouDate.gameObject,this);
        nowSheet = new CalenderSheetWidget(Ins_NowDate.gameObject, this);
        FlipPageAnimation = Img_previous.GetComponent<Animation>();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        //Debug.Log("CalenderWindow OnShow");
        var nowDate = Clocker.Instance.NowDateTime;
        var prevDate = Clocker.Instance.PrevDateTime;
        Txt_date.text = $"{(int)nowDate.Season}月{nowDate.Day}日\n{Clocker.Instance.NowDateTime.WeekDayStr()}";
        
        nowSheet.SetDate(nowDate.Season,(int)nowDate.Day,nowDate.WeekDayStr());
        if (nowDate.Year == prevDate.Year && nowDate.Day == prevDate.Day)
        {
            prevSheet.ShowBlank();
        }
        //canContinue = true;
        // if (!canContinue)
        // {
        //     //prevSheet.SetDate(prevDate.Season,(int)prevDate.Day,prevDate.DayOfWeek);
        //     //动画
        //     doPassDayAnimation();
        //     //canContinue = false;
        // }
        
        // Btn_continue.OnClickAsObservable().Subscribe(clickContinue).AddTo(handles);
        
        createTempPictureOfPreviousDay();
        doWeatherAnimation(WeatherMgr.Instance.NowWeather.Weather);
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
            prevSheet.SetDate(prevDate.Season,(int)prevDate.Day,prevDate.WeekDayStr());
        }
    }

    public override void OnHide()
    {
        base.OnHide();
        // ContinueCB = null;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    // private void doPassDayAnimation()
    // {
    //     var nowDate = Clocker.Instance.NowDateTime;
    //     if (nowDate.Year == GameDateTime.InitialYear && nowDate.Season == Season.Spring && nowDate.Day == 1)
    //     {
    //         prevSheet.OnHide();
    //         Txt_date.text = $"{(int)nowDate.Season}月{nowDate.Day}日\n{nowDate.WeekDayStr()}";
    //     }
    //     else
    //     {
    //         prevSheet.OnShow(null);
    //         var seq = DOTween.Sequence();
    //         seq.PrependInterval(0.5f);
    //         seq.Append(prevSheet.uiRectTran.DORotate(new Vector3(0,0,-30f),2f));
    //         seq.Join(prevSheet.uiRectTran.DOMove(new Vector3(-800f,-1100f,0f),5f));
    //         seq.InsertCallback(0.5f,() =>
    //         {
    //             Txt_date.text = $"{(int)nowDate.Season}月{nowDate.Day}日\n{nowDate.WeekDayStr()}";
    //         });    
    //     }
    // }

    private void doWeatherAnimation(Weather weather)
    {
        // weatherTweener?.Kill(false);
        switch (weather)
        {
            case Weather.Sunshine:
                // weatherTweener = Img_weather.rectTransform.DOLocalRotate(
                //     new Vector3(0, 0, -360f), 15f,RotateMode.LocalAxisAdd).
                //     SetEase(Ease.Linear).
                //     SetLoops(-1);
                break;
            case Weather.Rain:
                LoadSpriteAsync("Assets/GameRes/Picture/UI/Phone/Weather/rain_mark.png", Img_weather);
                break;
            case Weather.Snow:
                break;
            case Weather.Typhoon:
                break;
            case Weather.SnowStorm:
                break;
        }
    }

    // private async void clickContinue(Unit param)
    // {
    //     _animation.Play("FlipPage");
    //     await UniTask.Delay(TimeSpan.FromMilliseconds(1300));
    //     // ContinueCB?.Invoke();
    // }

    
    
    private async void createTempPictureOfPreviousDay()
    {
        await UniTask.NextFrame();
        
        var task =  YooAssets.LoadAssetAsync<RenderTexture>("Assets/GameRes/Picture/CalenderTexture.renderTexture");
        await task.ToUniTask();
        
        var renderTexture = task.AssetObject as RenderTexture;
        var canvas = UIManager.Instance.RootCanvas;
        Camera camera = UIManager.Instance.UICamera;
        camera.targetTexture = renderTexture;
        
        // 将Canvas的渲染目标设置为这个RenderTexture
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = camera;

        // 渲染Canvas
        camera.Render();
        
        // 读取RenderTexture的像素并保存为图片
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D((int)canvas.pixelRect.width,(int)canvas.pixelRect.height ,TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(0,0, canvas.pixelRect.width,canvas.pixelRect.height), 0, 0);
        screenshot.Apply();

        
        
        var anchoredPosition = prevSheet.uiRectTran.anchoredPosition;
        float x = anchoredPosition.x;
        float y = anchoredPosition.y;
        var sizeDelta = prevSheet.uiRectTran.sizeDelta;
        float rectWidth = sizeDelta.x;
        float rectHeight = sizeDelta.y;
        
        var sp = Sprite.Create(screenshot,new Rect(x,y,rectWidth,rectHeight),new Vector2(0.5f, 0.5f));
        Img_previous.sprite = sp;
        Img_previous.gameObject.SetActive(true);
        Ins_PreviouDate.gameObject.SetActive(false);
        camera.targetTexture = null;
        
        Go_weather.gameObject.SetActive(true);
        RenderTexture.active = null;
        
        Img_previous.material.SetTexture("_MainTex",sp.texture);
        Img_previous.material.SetTexture("_BackTex",nowSheet.Img_bg.sprite.texture);
        // Img_previous.material.SetVector("_DragPoint",new Vector4(400,0,0,0));
        // 将图片保存为文件
        // byte[] bytes = screenshot.EncodeToPNG();
        // System.IO.File.WriteAllBytes(imagePath, bytes);

        // 恢复Canvas设置
        // canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // canvas.worldCamera = null;
    }
    
    
}