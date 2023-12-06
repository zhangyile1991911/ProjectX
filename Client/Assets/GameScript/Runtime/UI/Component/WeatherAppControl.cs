using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class WeatherApp : BaseAppWidget
{
    private List<Image> clouds;
    private List<Tween> cloudTweens;

    private Tween sunRotaTween;
    
    public WeatherApp(GameObject go,UIWindow parent):base(go,parent)
    {
		
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

        playCloudFloating();
        playSunRotation();
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
    
    
}