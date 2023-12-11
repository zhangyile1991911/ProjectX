using System.Collections.Generic;
using cfg.common;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class WeatherApp : BaseAppWidget
{
    private List<Image> clouds;
    private List<Tween> cloudTweens;

    private Tween sunRotaTween;
    // private CanvasGroup cloudCanvasGroup;
    private CanvasGroup fallCanvasGroup;
    private bool isToday;
    private Animator fallAnimator;
    private RectTransform _panelRT;
    private static readonly int Raining = Animator.StringToHash("raining");
    private static readonly int Sunny = Animator.StringToHash("sunny");
    private Weather curWeather;
    private int start_temperature;
    private int end_temperature;
    public WeatherApp(GameObject go,UIWindow parent):base(go,parent)
    {
        WidgetType = AppType.Weather;
    }
    
    public override void OnCreate()
    {
        clouds = new List<Image>(5);
        cloudTweens = new List<Tween>(5);
        
        for (int i = 0; i < Tran_Cloud.childCount; i++)
        {
            var c = Tran_Cloud.GetChild(i);
            var img = c.GetComponent<Image>();
            clouds.Add(img);
        }

        // cloudCanvasGroup = Tran_Cloud.GetComponent<CanvasGroup>();
        fallCanvasGroup = Tran_falling.GetComponent<CanvasGroup>();

        fallAnimator = Tran_falling.GetComponent<Animator>();
        
        isToday = true;
        curWeather = Weather.None;
        XBtn_arrow.OnClick.Subscribe(switchWeather).AddTo(uiGo);
        _panelRT = Tran_panel.GetComponent<RectTransform>();
    }
    
    public override void OnDestroy()
    {
        clouds.Clear();
        foreach (var one in cloudTweens)
        {
            one.Kill();
        }
        cloudTweens.Clear();
        
        sunRotaTween.Kill();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        foreach (var one in cloudTweens)
        {
            one.Restart();
        }
        switchWeather(null);
    }

    public override void OnHide()
    {
        base.OnHide();
        foreach (var one in cloudTweens)
        {
            one.Pause();
        }

        sunRotaTween?.Pause();
    }

    public override void OnUpdate()
    {
        
    }

    private void playCloudFloating()
    {
        if (cloudTweens.Count <= 0)
        {
            foreach (var one in clouds)
            {
                var rt = one.GetComponent<RectTransform>();
                var oldpos = rt.anchoredPosition;
                oldpos.x += Random.Range(-15f, 15f);
                var tw = rt.DOAnchorPosX(oldpos.x, 3f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
                cloudTweens.Add(tw);
            }
        }
        else
        {
            foreach (var one in cloudTweens)
            {
                one.Restart();
            }
        }
    }

    private void stopCloudFloating()
    {
        foreach (var one in cloudTweens)
        {
            one.Pause();
        }
    }

    private void playSunRotation()
    {
        if (sunRotaTween == null)
        {
            var rt = Img_Sun.GetComponent<RectTransform>();
            rt.DOLocalRotate(new Vector3(0, 0, 360f), 60f, RotateMode.LocalAxisAdd).SetLoops(-1,LoopType.Restart);
        }
        else
        {
            sunRotaTween.Restart();
        }
    }

    private void pauseSunRotation()
    {
        sunRotaTween?.Pause();
    }

    private void playFalling()
    {
        Tran_falling.gameObject.SetActive(true);
        DOTween.To(
            () => fallCanvasGroup.alpha, 
            (x) =>
            {
                fallCanvasGroup.alpha = x;
            },
            1f,1f);
        fallAnimator.enabled = true;
        fallAnimator.Play("Raining");
    }

    private void pauseFalling()
    {
        Tran_falling.gameObject.SetActive(false);
        DOTween.To(
            () => fallCanvasGroup.alpha, 
            (x) =>
            {
                fallCanvasGroup.alpha = x;
            },
            0f,1f);
        fallAnimator.enabled = false;
    }
    
    private void switchWeather(PointerEventData param)
    {
        if(param != null) isToday = !isToday;

        Weather weather;
        if (isToday)
        {
            weather = WeatherMgr.Instance.NowWeather.Weather;
            start_temperature = WeatherMgr.Instance.NowWeather.temperature_start;
            end_temperature = WeatherMgr.Instance.NowWeather.temperature_end;
        }
        else
        {
            weather = WeatherMgr.Instance.NextWeather.Weather;
            start_temperature = WeatherMgr.Instance.NextWeather.temperature_start;
            end_temperature = WeatherMgr.Instance.NextWeather.temperature_end;
        }
        
        
        playCloudFloating();
        switch (weather)
        {
            case Weather.Sunshine:
                playSunShow();
                pauseFalling();
                break;
            case Weather.Rain:
                playSunHide();
                playFalling();
                break;
        }
        
        switchCloud(weather);
        switchSky(weather);
        switchMoutain(weather);
        changeWeatherInfo(weather);
        switchAnnouncer(weather);
        curWeather = weather;
    }
    
    private void playSunShow()
    {
        var sun_seq = DOTween.Sequence();;
        sun_seq.AppendCallback(()=>
        {
            Img_Sun.gameObject.SetActive(true);
        });
        sun_seq.Append(Img_Sun.rectTransform.DOAnchorPosX(149.2f, 1f).SetEase(Ease.OutCubic));
        playSunRotation();
    }
    
    private void playSunHide()
    {
        var sun_seq = DOTween.Sequence();;
        sun_seq.Append(Img_Sun.rectTransform.DOAnchorPosX(-200f, 1f).SetEase(Ease.InCubic));
        sun_seq.AppendCallback(()=>
        {
            Img_Sun.gameObject.SetActive(false);
        });
        pauseSunRotation();
    }

    private async void switchSky(Weather weather)
    {
        if (curWeather == weather) return;
        string sun_img_res = "";
        switch (weather)
        {
            case Weather.Sunshine:
                sun_img_res = "Assets/GameRes/Picture/UI/Phone/Weather/sky.png";
                break;
            case Weather.Rain:
                sun_img_res = "Assets/GameRes/Picture/UI/Phone/Weather/rainingSky.png";
                break;
        }

        Sprite loadSp = null;
        var s1 = ParentWindow.LoadSpriteAsync(
            sun_img_res,
            (sp) => loadSp = sp);
        await UniTask.WhenAll(
            s1,
            Img_Sky.DOFade(0, 1f).ToUniTask());
        
        Img_Sky.sprite = loadSp;
        Img_Sky.DOFade(1f, 1.0f);

    }

    private string CloudStr(Weather weather,string objname)
    {
        if (Weather.Sunshine == weather)
        {
            switch (objname)
            {
                case "c1":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/cloud1.png";
                case "c2":
                case "c3":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/cloud2.png";
                case "c4":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/cloud3.png";
                case "c5":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/cloud4.png";
            }
        }

        if (Weather.Rain == weather)
        {
            switch (objname)
            {
                case "c1":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/raining_cloud1.png";
                case "c2":
                case "c3":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/raining_cloud2.png";
                case "c4":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/raining_cloud3.png";
                case "c5":
                    return "Assets/GameRes/Picture/UI/Phone/Weather/raining_cloud4.png";
            }
        }

        return "";
    }

    private async void switchCloud(Weather weather)
    {
        if (curWeather == weather) return;
        string cloud_img_res = "";
        List<UniTask> alltask = new List<UniTask>(10);
        List<float> cloudMove = new List<float>(5);
        for (int i = 0; i < clouds.Count;i++)
        {
            var one = clouds[i]; 
            cloud_img_res = CloudStr(weather,one.name);
            var s1 = ParentWindow.LoadSpriteAsync(cloud_img_res, (sp) => { one.sprite = sp; });
            alltask.Add(s1);
            if (one.rectTransform.anchoredPosition.x > 0)
            {
                cloudMove.Add(one.rectTransform.anchoredPosition.x);
                var t = one.rectTransform.DOAnchorPosX(500f,1.0f);
                alltask.Add(t.ToUniTask());    
            }
            else
            {
                var t = one.rectTransform.DOAnchorPosX(-500f,1.0f);
                cloudMove.Add(one.rectTransform.anchoredPosition.x);
                alltask.Add(t.ToUniTask());
            }
        }
        stopCloudFloating();
        await UniTask.WhenAll(alltask);
        alltask.Clear();
        for (int i = 0;i < cloudMove.Count;i++)
        {
            var one = clouds[i];
            var t = one.rectTransform.DOAnchorPosX(cloudMove[i], 1.0f).ToUniTask();
            alltask.Add(t);
        }

        await UniTask.WhenAll(alltask);
        playCloudFloating();
    }
    

    private async void switchMoutain(Weather weather)
    {
        if (curWeather == weather) return;
        string moutain_res_path = "";
        switch (weather)
        {
            case Weather.Sunshine:
                moutain_res_path = "Assets/GameRes/Picture/UI/Phone/Weather/mountain.png";
                break;
            case Weather.Rain:
                moutain_res_path = "Assets/GameRes/Picture/UI/Phone/Weather/raining_mountain.png";
                break;
        }
        
        Sprite moutainSprite = null;
        var s1 = ParentWindow.LoadSpriteAsync(moutain_res_path, sp => moutainSprite = sp);
        var s2 = Img_Moutain.rectTransform.DOScaleY(0, 1f).ToUniTask();
        await UniTask.WhenAll(s1, s2);
        Img_Moutain.sprite = moutainSprite;
        Img_Moutain.rectTransform.DOScaleY(1f, 1f);
    }

    private void switchAnnouncer(Weather weather)
    {
        if (curWeather == weather) return;
        switch (weather)
        {
            case Weather.Rain:
                Ani_announcer.SetBool(Raining,true);
                Ani_announcer.SetBool(Sunny,false);
                break;
            case Weather.Sunshine:
                Ani_announcer.SetBool(Sunny,true);
                Ani_announcer.SetBool(Raining,false);
                break;
        }
    }


    private void changeWeatherInfo(Weather weather)
    {
        // if (curWeather == weather) return;
        XBtn_arrow.gameObject.SetActive(false);
        var s1 = _panelRT.DOScaleX(0, 1f).OnComplete(() =>
        {
            switch (weather)
            {
                case Weather.Rain:
                    Txt_condition.text = "雨";
                    ParentWindow.LoadSpriteAsync("Assets/GameRes/Picture/UI/Phone/Weather/rain_mark.png", Img_show);
                    break;
                case Weather.Sunshine:
                    Txt_condition.text = "晴";
                    ParentWindow.LoadSpriteAsync("Assets/GameRes/Picture/UI/Phone/Weather/sun_mark.png", Img_show);
                    break;
            }

            Txt_day.text = isToday ? "今天" : "明天";
            if (start_temperature == end_temperature)
            {
                Txt_temp.text = ZString.Format("{0}°",start_temperature);
            }
            else
            {
                Txt_temp.text = ZString.Format("{0}°/{1}°",start_temperature,end_temperature);    
            }
            
            _panelRT.DOScaleX(1f, 1f).OnComplete(()=>{XBtn_arrow.gameObject.SetActive(true);});
        });
        
    }
    
    
}