using System.Collections;
using System.Collections.Generic;
using cfg.common;
using UnityEngine;

public class WeatherMgr : SingletonModule<WeatherMgr>
{
    public Weather NowWeather => Weather.Sunshine; 
    public override void OnCreate(object createParam)
    {
        base.OnCreate(this);
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
    }
}
