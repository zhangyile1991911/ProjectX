using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAppWidget : UIComponent
{
    public enum AppType
    {
        News,
        AirPlane,
    }

    public AppType WidgetType
    {
        get;protected set;
    }


    public long PreviewStart { get; protected set; }
    private Clocker _clocker;
    public BaseAppWidget(GameObject go, UIWindow parent) : base(go, parent)
    {
        _clocker = UniModule.GetModule<Clocker>();
    }


    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        PreviewStart = _clocker.Now;
    }
    
    
    
    
}
