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

public sealed partial class TbCharacterBubble
{
    private readonly Dictionary<int, character.CharacterBubble> _dataMap;
    private readonly List<character.CharacterBubble> _dataList;
    
    public TbCharacterBubble(JSONNode _json)
    {
        _dataMap = new Dictionary<int, character.CharacterBubble>();
        _dataList = new List<character.CharacterBubble>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = character.CharacterBubble.DeserializeCharacterBubble(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, character.CharacterBubble> DataMap => _dataMap;
    public List<character.CharacterBubble> DataList => _dataList;

    public character.CharacterBubble GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public character.CharacterBubble Get(int key) => _dataMap[key];
    public character.CharacterBubble this[int key] => _dataMap[key];

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