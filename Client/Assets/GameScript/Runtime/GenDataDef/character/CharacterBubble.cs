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

public sealed partial class CharacterBubble :  Bright.Config.BeanBase 
{
    public CharacterBubble(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["title"].IsString) { throw new SerializationException(); }  Title = _json["title"]; }
        { if(!_json["comment"].IsString) { throw new SerializationException(); }  Comment = _json["comment"]; }
        { if(!_json["npc_id"].IsNumber) { throw new SerializationException(); }  NpcId = _json["npc_id"]; }
        { if(!_json["repeated"].IsBoolean) { throw new SerializationException(); }  Repeated = _json["repeated"]; }
        { var __json0 = _json["pre_condition"]; if(!__json0.IsArray) { throw new SerializationException(); } PreCondition = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  PreCondition.Add(__v0); }   }
        { var __json0 = _json["week_day"]; if(!__json0.IsArray) { throw new SerializationException(); } WeekDay = new System.Collections.Generic.List<common.WeekDay>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { common.WeekDay __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (common.WeekDay)__e0.AsInt; }  WeekDay.Add(__v0); }   }
        { if(!_json["friend_value"].IsObject) { throw new SerializationException(); }  FriendValue = common.value_region.Deserializevalue_region(_json["friend_value"]);  }
        { if(!_json["bubble_type"].IsNumber) { throw new SerializationException(); }  BubbleType = (common.bubbleType)_json["bubble_type"].AsInt; }
        { if(!_json["menu_id"].IsNumber) { throw new SerializationException(); }  MenuId = _json["menu_id"]; }
        { if(!_json["bubble_bg"].IsString) { throw new SerializationException(); }  BubbleBg = _json["bubble_bg"]; }
        { var _j = _json["dialogue_content_res"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsString) { throw new SerializationException(); }  DialogueContentRes = _j; } } else { DialogueContentRes = null; } }
        { var _j = _json["dialogue_start_node"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsString) { throw new SerializationException(); }  DialogueStartNode = _j; } } else { DialogueStartNode = null; } }
        PostInit();
    }

    public CharacterBubble(int id, string title, string comment, int npc_id, bool repeated, System.Collections.Generic.List<int> pre_condition, System.Collections.Generic.List<common.WeekDay> week_day, common.value_region friend_value, common.bubbleType bubble_type, int menu_id, string bubble_bg, string dialogue_content_res, string dialogue_start_node ) 
    {
        this.Id = id;
        this.Title = title;
        this.Comment = comment;
        this.NpcId = npc_id;
        this.Repeated = repeated;
        this.PreCondition = pre_condition;
        this.WeekDay = week_day;
        this.FriendValue = friend_value;
        this.BubbleType = bubble_type;
        this.MenuId = menu_id;
        this.BubbleBg = bubble_bg;
        this.DialogueContentRes = dialogue_content_res;
        this.DialogueStartNode = dialogue_start_node;
        PostInit();
    }

    public static CharacterBubble DeserializeCharacterBubble(JSONNode _json)
    {
        return new character.CharacterBubble(_json);
    }

    /// <summary>
    /// 自增id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; private set; }
    /// <summary>
    /// 注释(方便规划)
    /// </summary>
    public string Comment { get; private set; }
    /// <summary>
    /// 关联id
    /// </summary>
    public int NpcId { get; private set; }
    public character.CharacterBaseInfo NpcId_Ref { get; private set; }
    /// <summary>
    /// 是否反复阅读
    /// </summary>
    public bool Repeated { get; private set; }
    /// <summary>
    /// 前置条件
    /// </summary>
    public System.Collections.Generic.List<int> PreCondition { get; private set; }
    public System.Collections.Generic.List<common.WeekDay> WeekDay { get; private set; }
    public common.value_region FriendValue { get; private set; }
    /// <summary>
    /// 气泡类型
    /// </summary>
    public common.bubbleType BubbleType { get; private set; }
    /// <summary>
    /// 下单
    /// </summary>
    public int MenuId { get; private set; }
    /// <summary>
    /// 气泡背景图
    /// </summary>
    public string BubbleBg { get; private set; }
    /// <summary>
    /// 剧本路径
    /// </summary>
    public string DialogueContentRes { get; private set; }
    /// <summary>
    /// 剧本开始对话节点
    /// </summary>
    public string DialogueStartNode { get; private set; }

    public const int __ID__ = 918845488;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        this.NpcId_Ref = (_tables["character.TbBaseInfo"] as character.TbBaseInfo).GetOrDefault(NpcId);
        FriendValue?.Resolve(_tables);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        FriendValue?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Title:" + Title + ","
        + "Comment:" + Comment + ","
        + "NpcId:" + NpcId + ","
        + "Repeated:" + Repeated + ","
        + "PreCondition:" + Bright.Common.StringUtil.CollectionToString(PreCondition) + ","
        + "WeekDay:" + Bright.Common.StringUtil.CollectionToString(WeekDay) + ","
        + "FriendValue:" + FriendValue + ","
        + "BubbleType:" + BubbleType + ","
        + "MenuId:" + MenuId + ","
        + "BubbleBg:" + BubbleBg + ","
        + "DialogueContentRes:" + DialogueContentRes + ","
        + "DialogueStartNode:" + DialogueStartNode + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
