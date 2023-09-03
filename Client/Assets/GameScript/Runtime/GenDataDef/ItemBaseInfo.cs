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



namespace cfg
{ 

public sealed partial class ItemBaseInfo :  Bright.Config.BeanBase 
{
    public ItemBaseInfo(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["buy"].IsNumber) { throw new SerializationException(); }  Buy = _json["buy"]; }
        { if(!_json["sell"].IsNumber) { throw new SerializationException(); }  Sell = _json["sell"]; }
        { if(!_json["type"].IsNumber) { throw new SerializationException(); }  Type = (item.itemType)_json["type"].AsInt; }
        { var _j = _json["desc"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsString) { throw new SerializationException(); }  Desc = _j; } } else { Desc = null; } }
        { if(!_json["ui_res_path"].IsString) { throw new SerializationException(); }  UiResPath = _json["ui_res_path"]; }
        { if(!_json["scene_res_path"].IsString) { throw new SerializationException(); }  SceneResPath = _json["scene_res_path"]; }
        { var _j = _json["get_method"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsString) { throw new SerializationException(); }  GetMethod = _j; } } else { GetMethod = null; } }
        PostInit();
    }

    public ItemBaseInfo(int id, string name, int buy, int sell, item.itemType type, string desc, string ui_res_path, string scene_res_path, string get_method ) 
    {
        this.Id = id;
        this.Name = name;
        this.Buy = buy;
        this.Sell = sell;
        this.Type = type;
        this.Desc = desc;
        this.UiResPath = ui_res_path;
        this.SceneResPath = scene_res_path;
        this.GetMethod = get_method;
        PostInit();
    }

    public static ItemBaseInfo DeserializeItemBaseInfo(JSONNode _json)
    {
        return new ItemBaseInfo(_json);
    }

    /// <summary>
    /// 食材id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 买入
    /// </summary>
    public int Buy { get; private set; }
    /// <summary>
    /// 卖出
    /// </summary>
    public int Sell { get; private set; }
    /// <summary>
    /// 种类
    /// </summary>
    public item.itemType Type { get; private set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; private set; }
    /// <summary>
    /// 资源路径
    /// </summary>
    public string UiResPath { get; private set; }
    /// <summary>
    /// 场景资源
    /// </summary>
    public string SceneResPath { get; private set; }
    /// <summary>
    /// 获取方式
    /// </summary>
    public string GetMethod { get; private set; }

    public const int __ID__ = -929071246;
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
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "Buy:" + Buy + ","
        + "Sell:" + Sell + ","
        + "Type:" + Type + ","
        + "Desc:" + Desc + ","
        + "UiResPath:" + UiResPath + ","
        + "SceneResPath:" + SceneResPath + ","
        + "GetMethod:" + GetMethod + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
