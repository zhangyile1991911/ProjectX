using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class AirplaneAppWidget : BaseAppWidget
{
    private RectTransform bgA;
    private RectTransform bgB;
    private float maxY;
    public AirplaneAppWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        WidgetType = AppType.AirPlane;
    }
    
    public override void OnCreate()
    {
        bgA = Img_bg1.GetComponent<RectTransform>();
        bgB = Img_bg2.GetComponent<RectTransform>();
       
        
        Debug.Log($"bgA.anchorMax = {bgA.anchorMax}");
        Debug.Log($"bgB.anchorMax = {bgB.anchorMax}");
    }
    
    public override void OnDestroy()
    {
        
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        Debug.Log($"sizeDelta = {Img_Touch.rectTransform.rect}");
        Debug.Log($"sizeDelta = {Img_Touch.rectTransform.rect.height}");
        maxY = Img_Touch.rectTransform.rect.height;
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    private float scrollSpeed = 0.2f;
    public override void OnUpdate()
    {
        movebg();
        handletouch();
    }

    private void movebg()
    {
        var pos = bgA.anchoredPosition;
        pos.y += scrollSpeed;
        if (pos.y >= maxY)
        {
            pos.y = -maxY;
        }
        bgA.anchoredPosition = pos;
        
        pos = bgB.anchoredPosition;
        pos.y += scrollSpeed;
        if (pos.y >= maxY)
        {
            pos.y = -maxY;
        }
        bgB.anchoredPosition = pos;
    }

    private void handletouch()
    {
        
    }
}