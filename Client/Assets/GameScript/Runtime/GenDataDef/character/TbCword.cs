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

public sealed partial class TbCword
{
    private readonly Dictionary<int, character.Crowd> _dataMap;
    private readonly List<character.Crowd> _dataList;
    
    public TbCword(JSONNode _json)
    {
        _dataMap = new Dictionary<int, character.Crowd>();
        _dataList = new List<character.Crowd>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = character.Crowd.DeserializeCrowd(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, character.Crowd> DataMap => _dataMap;
    public List<character.Crowd> DataList => _dataList;

    public character.Crowd GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public character.Crowd Get(int key) => _dataMap[key];
    public character.Crowd this[int key] => _dataMap[key];

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