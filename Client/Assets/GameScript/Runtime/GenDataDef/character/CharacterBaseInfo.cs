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



namespace cfg.character
{ 

public sealed partial class CharacterBaseInfo :  Bright.Config.BeanBase 
{
    public CharacterBaseInfo(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["desc"].IsString) { throw new SerializationException(); }  Desc = _json["desc"]; }
        { if(!_json["soul"].IsNumber) { throw new SerializationException(); }  Soul = _json["soul"]; }
        { if(!_json["prefab_path"].IsString) { throw new SerializationException(); }  PrefabPath = _json["prefab_path"]; }
        { if(!_json["picture_path"].IsString) { throw new SerializationException(); }  PicturePath = _json["picture_path"]; }
        { var __json0 = _json["like_flavour"]; if(!__json0.IsArray) { throw new SerializationException(); } LikeFlavour = new System.Collections.Generic.HashSet<food.flavorTag>(/*__json0.Count*/); foreach(JSONNode __e0 in __json0.Children) { food.flavorTag __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (food.flavorTag)__e0.AsInt; }  LikeFlavour.Add(__v0); }   }
        { var __json0 = _json["unlike_flavour"]; if(!__json0.IsArray) { throw new SerializationException(); } UnlikeFlavour = new System.Collections.Generic.HashSet<food.flavorTag>(/*__json0.Count*/); foreach(JSONNode __e0 in __json0.Children) { food.flavorTag __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (food.flavorTag)__e0.AsInt; }  UnlikeFlavour.Add(__v0); }   }
        { if(!_json["schedule_group"].IsNumber) { throw new SerializationException(); }  ScheduleGroup = _json["schedule_group"]; }
        PostInit();
    }

    public CharacterBaseInfo(int id, string name, string desc, int soul, string prefab_path, string picture_path, System.Collections.Generic.HashSet<food.flavorTag> like_flavour, System.Collections.Generic.HashSet<food.flavorTag> unlike_flavour, int schedule_group ) 
    {
        this.Id = id;
        this.Name = name;
        this.Desc = desc;
        this.Soul = soul;
        this.PrefabPath = prefab_path;
        this.PicturePath = picture_path;
        this.LikeFlavour = like_flavour;
        this.UnlikeFlavour = unlike_flavour;
        this.ScheduleGroup = schedule_group;
        PostInit();
    }

    public static CharacterBaseInfo DeserializeCharacterBaseInfo(JSONNode _json)
    {
        return new character.CharacterBaseInfo(_json);
    }

    /// <summary>
    /// npcid
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; private set; }
    /// <summary>
    /// 灵魂
    /// </summary>
    public int Soul { get; private set; }
    /// <summary>
    /// 预制体路径
    /// </summary>
    public string PrefabPath { get; private set; }
    /// <summary>
    /// 图片路径
    /// </summary>
    public string PicturePath { get; private set; }
    /// <summary>
    /// 喜欢的口味
    /// </summary>
    public System.Collections.Generic.HashSet<food.flavorTag> LikeFlavour { get; private set; }
    /// <summary>
    /// 讨厌的口味
    /// </summary>
    public System.Collections.Generic.HashSet<food.flavorTag> UnlikeFlavour { get; private set; }
    /// <summary>
    /// 行程组
    /// </summary>
    public int ScheduleGroup { get; private set; }

    public const int __ID__ = -1834518077;
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
        + "Desc:" + Desc + ","
        + "Soul:" + Soul + ","
        + "PrefabPath:" + PrefabPath + ","
        + "PicturePath:" + PicturePath + ","
        + "LikeFlavour:" + Bright.Common.StringUtil.CollectionToString(LikeFlavour) + ","
        + "UnlikeFlavour:" + Bright.Common.StringUtil.CollectionToString(UnlikeFlavour) + ","
        + "ScheduleGroup:" + ScheduleGroup + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
