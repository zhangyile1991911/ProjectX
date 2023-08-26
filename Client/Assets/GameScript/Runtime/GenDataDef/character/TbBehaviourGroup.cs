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

public sealed partial class TbBehaviourGroup
{
    private readonly Dictionary<int, character.BehaviourGroup> _dataMap;
    private readonly List<character.BehaviourGroup> _dataList;
    
    public TbBehaviourGroup(JSONNode _json)
    {
        _dataMap = new Dictionary<int, character.BehaviourGroup>();
        _dataList = new List<character.BehaviourGroup>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = character.BehaviourGroup.DeserializeBehaviourGroup(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, character.BehaviourGroup> DataMap => _dataMap;
    public List<character.BehaviourGroup> DataList => _dataList;

    public character.BehaviourGroup GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public character.BehaviourGroup Get(int key) => _dataMap[key];
    public character.BehaviourGroup this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}