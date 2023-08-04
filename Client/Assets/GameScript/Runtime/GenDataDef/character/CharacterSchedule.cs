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

public sealed partial class CharacterSchedule :  Bright.Config.BeanBase 
{
    public CharacterSchedule(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["partner_id"].IsNumber) { throw new SerializationException(); }  PartnerId = _json["partner_id"]; }
        { var __json0 = _json["character_appear_infos"]; if(!__json0.IsArray) { throw new SerializationException(); } CharacterAppearInfos = new System.Collections.Generic.List<common.appear_time>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { common.appear_time __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = common.appear_time.Deserializeappear_time(__e0);  }  CharacterAppearInfos.Add(__v0); }   }
        PostInit();
    }

    public CharacterSchedule(int id, string name, int partner_id, System.Collections.Generic.List<common.appear_time> character_appear_infos ) 
    {
        this.Id = id;
        this.Name = name;
        this.PartnerId = partner_id;
        this.CharacterAppearInfos = character_appear_infos;
        PostInit();
    }

    public static CharacterSchedule DeserializeCharacterSchedule(JSONNode _json)
    {
        return new character.CharacterSchedule(_json);
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public int PartnerId { get; private set; }
    public System.Collections.Generic.List<common.appear_time> CharacterAppearInfos { get; private set; }

    public const int __ID__ = -810615493;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var _e in CharacterAppearInfos) { _e?.Resolve(_tables); }
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var _e in CharacterAppearInfos) { _e?.TranslateText(translator); }
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "PartnerId:" + PartnerId + ","
        + "CharacterAppearInfos:" + Bright.Common.StringUtil.CollectionToString(CharacterAppearInfos) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
