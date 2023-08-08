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



namespace cfg.food
{ 

public sealed partial class MenuInfo :  Bright.Config.BeanBase 
{
    public MenuInfo(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["make_method"].IsNumber) { throw new SerializationException(); }  MakeMethod = (food.cookTools)_json["make_method"].AsInt; }
        { var __json0 = _json["related_material"]; if(!__json0.IsArray) { throw new SerializationException(); } RelatedMaterial = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  RelatedMaterial.Add(__v0); }   }
        { var __json0 = _json["tag"]; if(!__json0.IsArray) { throw new SerializationException(); } Tag = new System.Collections.Generic.List<food.flavorTag>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { food.flavorTag __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (food.flavorTag)__e0.AsInt; }  Tag.Add(__v0); }   }
        { var __json0 = _json["opposite_tag"]; if(!__json0.IsArray) { throw new SerializationException(); } OppositeTag = new System.Collections.Generic.List<food.flavorTag>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { food.flavorTag __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (food.flavorTag)__e0.AsInt; }  OppositeTag.Add(__v0); }   }
        { if(!_json["difficulty"].IsNumber) { throw new SerializationException(); }  Difficulty = (food.cookDifficulty)_json["difficulty"].AsInt; }
        { if(!_json["cost_time"].IsNumber) { throw new SerializationException(); }  CostTime = _json["cost_time"]; }
        PostInit();
    }

    public MenuInfo(int id, string name, food.cookTools make_method, System.Collections.Generic.List<int> related_material, System.Collections.Generic.List<food.flavorTag> tag, System.Collections.Generic.List<food.flavorTag> opposite_tag, food.cookDifficulty difficulty, int cost_time ) 
    {
        this.Id = id;
        this.Name = name;
        this.MakeMethod = make_method;
        this.RelatedMaterial = related_material;
        this.Tag = tag;
        this.OppositeTag = opposite_tag;
        this.Difficulty = difficulty;
        this.CostTime = cost_time;
        PostInit();
    }

    public static MenuInfo DeserializeMenuInfo(JSONNode _json)
    {
        return new food.MenuInfo(_json);
    }

    /// <summary>
    /// 菜谱id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 制作方式
    /// </summary>
    public food.cookTools MakeMethod { get; private set; }
    /// <summary>
    /// 关联食材
    /// </summary>
    public System.Collections.Generic.List<int> RelatedMaterial { get; private set; }
    public System.Collections.Generic.List<food.FoodMaterial> RelatedMaterial_Ref { get; private set; }
    /// <summary>
    /// 标签
    /// </summary>
    public System.Collections.Generic.List<food.flavorTag> Tag { get; private set; }
    /// <summary>
    /// 反标签
    /// </summary>
    public System.Collections.Generic.List<food.flavorTag> OppositeTag { get; private set; }
    /// <summary>
    /// 难度
    /// </summary>
    public food.cookDifficulty Difficulty { get; private set; }
    /// <summary>
    /// 消耗时间
    /// </summary>
    public int CostTime { get; private set; }

    public const int __ID__ = -566658467;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        { food.TbMaterial __table = (food.TbMaterial)_tables["food.TbMaterial"]; this.RelatedMaterial_Ref = new System.Collections.Generic.List<food.FoodMaterial>(); foreach(var __e in RelatedMaterial) { this.RelatedMaterial_Ref.Add(__table.GetOrDefault(__e)); } }
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
        + "MakeMethod:" + MakeMethod + ","
        + "RelatedMaterial:" + Bright.Common.StringUtil.CollectionToString(RelatedMaterial) + ","
        + "Tag:" + Bright.Common.StringUtil.CollectionToString(Tag) + ","
        + "OppositeTag:" + Bright.Common.StringUtil.CollectionToString(OppositeTag) + ","
        + "Difficulty:" + Difficulty + ","
        + "CostTime:" + CostTime + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
