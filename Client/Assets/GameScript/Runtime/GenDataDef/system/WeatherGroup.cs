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



namespace cfg.system
{ 

public sealed partial class WeatherGroup :  Bright.Config.BeanBase 
{
    public WeatherGroup(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["季节"].IsNumber) { throw new SerializationException(); }  季节 = (common.Season)_json["季节"].AsInt; }
        { if(!_json["weather"].IsObject) { throw new SerializationException(); }  Weather = common.weather_group.Deserializeweather_group(_json["weather"]);  }
        PostInit();
    }

    public WeatherGroup(int id, common.Season 季节, common.weather_group weather ) 
    {
        this.Id = id;
        this.季节 = 季节;
        this.Weather = weather;
        PostInit();
    }

    public static WeatherGroup DeserializeWeatherGroup(JSONNode _json)
    {
        return new system.WeatherGroup(_json);
    }

    public int Id { get; private set; }
    public common.Season 季节 { get; private set; }
    public common.weather_group Weather { get; private set; }

    public const int __ID__ = 752220266;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        Weather?.Resolve(_tables);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        Weather?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "季节:" + 季节 + ","
        + "Weather:" + Weather + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}