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
        { if(!_json["npc_id"].IsNumber) { throw new SerializationException(); }  NpcId = _json["npc_id"]; }
        { if(!_json["friend_value"].IsNumber) { throw new SerializationException(); }  FriendValue = _json["friend_value"]; }
        { if(!_json["bubble_type"].IsNumber) { throw new SerializationException(); }  BubbleType = (common.bubbleType)_json["bubble_type"].AsInt; }
        { if(!_json["bubble_bg"].IsString) { throw new SerializationException(); }  BubbleBg = _json["bubble_bg"]; }
        { var _j = _json["dialogue_content_res"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsString) { throw new SerializationException(); }  DialogueContentRes = _j; } } else { DialogueContentRes = null; } }
        PostInit();
    }

    public CharacterBubble(int id, string title, int npc_id, int friend_value, common.bubbleType bubble_type, string bubble_bg, string dialogue_content_res ) 
    {
        this.Id = id;
        this.Title = title;
        this.NpcId = npc_id;
        this.FriendValue = friend_value;
        this.BubbleType = bubble_type;
        this.BubbleBg = bubble_bg;
        this.DialogueContentRes = dialogue_content_res;
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
    /// 关联id
    /// </summary>
    public int NpcId { get; private set; }
    public character.CharacterBaseInfo NpcId_Ref { get; private set; }
    public int FriendValue { get; private set; }
    /// <summary>
    /// 气泡类型
    /// </summary>
    public common.bubbleType BubbleType { get; private set; }
    /// <summary>
    /// 气泡背景图
    /// </summary>
    public string BubbleBg { get; private set; }
    /// <summary>
    /// 剧本路径
    /// </summary>
    public string DialogueContentRes { get; private set; }

    public const int __ID__ = 918845488;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        this.NpcId_Ref = (_tables["character.TbBaseInfo"] as character.TbBaseInfo).GetOrDefault(NpcId);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Title:" + Title + ","
        + "NpcId:" + NpcId + ","
        + "FriendValue:" + FriendValue + ","
        + "BubbleType:" + BubbleType + ","
        + "BubbleBg:" + BubbleBg + ","
        + "DialogueContentRes:" + DialogueContentRes + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}