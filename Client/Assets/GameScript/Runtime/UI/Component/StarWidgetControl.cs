using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class StarWidget : UIComponent
{
    public StarWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        
    }
    
    public override void OnDestroy()
    {
        
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }

    public void Light()
    {
        Img_starlight.gameObject.SetActive(true);
    }

    public void UnLight()
    {
        Img_starlight.gameObject.SetActive(false);
    }

    public void HalfLight(float val)
    {
        Img_starlight.gameObject.SetActive(true);
        Img_starlight.fillAmount = Mathf.Clamp01(val);
    }
}