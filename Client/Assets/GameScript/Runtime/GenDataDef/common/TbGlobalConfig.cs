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



namespace cfg.common
{ 

public sealed partial class TbGlobalConfig
{
    private readonly Dictionary<string, common.GlobalConfig> _dataMap;
    private readonly List<common.GlobalConfig> _dataList;
    
    public TbGlobalConfig(JSONNode _json)
    {
        _dataMap = new Dictionary<string, common.GlobalConfig>();
        _dataList = new List<common.GlobalConfig>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = common.GlobalConfig.DeserializeGlobalConfig(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<string, common.GlobalConfig> DataMap => _dataMap;
    public List<common.GlobalConfig> DataList => _dataList;

    public common.GlobalConfig GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public common.GlobalConfig Get(string key) => _dataMap[key];
    public common.GlobalConfig this[string key] => _dataMap[key];

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