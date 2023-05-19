using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Cysharp.Text;
using SimpleJSON;
using UnityEngine;

public class DataProviderModule : IModule
{
    private cfg.Tables _database;
    public void OnCreate(object createParam)
    {
#if UNITY_EDITOR
        _database = new cfg.Tables(LoadJsonBuf);
#endif
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    private JSONNode LoadJsonBuf(string file)
    {
        var fullPath = ZString.Format("{0}/{1}{2}{3}", Application.dataPath, "/Assets/GameRes/ExcelJson", file, ".json");
        return JSONNode.Parse(File.ReadAllText(fullPath));
    }
}
