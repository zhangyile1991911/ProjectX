//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg.common
{ 

public sealed partial class weather_group :  Bright.Config.BeanBase 
{
    public weather_group(JSONNode _json) 
    {
        { if(!_json["WeatherA"].IsNumber) { throw new SerializationException(); }  WeatherA = (common.Weather)_json["WeatherA"].AsInt; }
        { if(!_json["WeatherWeightA"].IsNumber) { throw new SerializationException(); }  WeatherWeightA = _json["WeatherWeightA"]; }
        { if(!_json["WeatherB"].IsNumber) { throw new SerializationException(); }  WeatherB = (common.Weather)_json["WeatherB"].AsInt; }
        { if(!_json["WeatherWeightB"].IsNumber) { throw new SerializationException(); }  WeatherWeightB = _json["WeatherWeightB"]; }
        { if(!_json["WeatherC"].IsNumber) { throw new SerializationException(); }  WeatherC = (common.Weather)_json["WeatherC"].AsInt; }
        { if(!_json["WeatherWeightC"].IsNumber) { throw new SerializationException(); }  WeatherWeightC = _json["WeatherWeightC"]; }
        { if(!_json["WeatherD"].IsNumber) { throw new SerializationException(); }  WeatherD = (common.Weather)_json["WeatherD"].AsInt; }
        { if(!_json["WeatherWeightD"].IsNumber) { throw new SerializationException(); }  WeatherWeightD = _json["WeatherWeightD"]; }
        { if(!_json["WeatherE"].IsNumber) { throw new SerializationException(); }  WeatherE = (common.Weather)_json["WeatherE"].AsInt; }
        { if(!_json["WeatherWeightE"].IsNumber) { throw new SerializationException(); }  WeatherWeightE = _json["WeatherWeightE"]; }
        PostInit();
    }

    public weather_group(common.Weather WeatherA, int WeatherWeightA, common.Weather WeatherB, int WeatherWeightB, common.Weather WeatherC, int WeatherWeightC, common.Weather WeatherD, int WeatherWeightD, common.Weather WeatherE, int WeatherWeightE ) 
    {
        this.WeatherA = WeatherA;
        this.WeatherWeightA = WeatherWeightA;
        this.WeatherB = WeatherB;
        this.WeatherWeightB = WeatherWeightB;
        this.WeatherC = WeatherC;
        this.WeatherWeightC = WeatherWeightC;
        this.WeatherD = WeatherD;
        this.WeatherWeightD = WeatherWeightD;
        this.WeatherE = WeatherE;
        this.WeatherWeightE = WeatherWeightE;
        PostInit();
    }

    public static weather_group Deserializeweather_group(JSONNode _json)
    {
        return new common.weather_group(_json);
    }

    /// <summary>
    /// 晴天
    /// </summary>
    public common.Weather WeatherA { get; private set; }
    /// <summary>
    /// 权重
    /// </summary>
    public int WeatherWeightA { get; private set; }
    /// <summary>
    /// 雨天
    /// </summary>
    public common.Weather WeatherB { get; private set; }
    /// <summary>
    /// 权重
    /// </summary>
    public int WeatherWeightB { get; private set; }
    /// <summary>
    /// 雪
    /// </summary>
    public common.Weather WeatherC { get; private set; }
    /// <summary>
    /// 权重
    /// </summary>
    public int WeatherWeightC { get; private set; }
    /// <summary>
    /// 台风
    /// </summary>
    public common.Weather WeatherD { get; private set; }
    /// <summary>
    /// 权重
    /// </summary>
    public int WeatherWeightD { get; private set; }
    /// <summary>
    /// 暴风雪
    /// </summary>
    public common.Weather WeatherE { get; private set; }
    /// <summary>
    /// 权重
    /// </summary>
    public int WeatherWeightE { get; private set; }

    public const int __ID__ = 1029502897;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "WeatherA:" + WeatherA + ","
        + "WeatherWeightA:" + WeatherWeightA + ","
        + "WeatherB:" + WeatherB + ","
        + "WeatherWeightB:" + WeatherWeightB + ","
        + "WeatherC:" + WeatherC + ","
        + "WeatherWeightC:" + WeatherWeightC + ","
        + "WeatherD:" + WeatherD + ","
        + "WeatherWeightD:" + WeatherWeightD + ","
        + "WeatherE:" + WeatherE + ","
        + "WeatherWeightE:" + WeatherWeightE + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}