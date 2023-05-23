using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using cfg.character;
using cfg.food;
using Cysharp.Text;
using SimpleJSON;
using UnityEngine;

public class DataProviderModule : IModule
{
    public cfg.Tables DataBase=>_database;
    private cfg.Tables _database;
    public void OnCreate(object createParam)
    {
#if UNITY_EDITOR
        _database = new cfg.Tables(LoadJsonBuf);
#endif
        initScheduleTable();
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    private JSONNode LoadJsonBuf(string file)
    {
        var fullPath = ZString.Format("{0}/{1}{2}{3}", Application.dataPath, "/GameRes/ExcelJson/", file, ".json");
        return JSONNode.Parse(File.ReadAllText(fullPath));
    }

    public CharacterBaseInfo GetCharacterBaseInfo(int characterId)
    {
        if (_database.TbBaseInfo.DataMap.ContainsKey(characterId))
        {
            return _database.TbBaseInfo.DataMap[characterId];
        }

        return null;
    }
    
    private List<List<int>> weekdaySchedule;
    private void initScheduleTable()
    {
        weekdaySchedule = new List<List<int>>(7);
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        foreach(var one in _database.TbSchedule.DataList)
        {
            foreach (var day in one.CharacterAppearInfos)
            {
                weekdaySchedule[(int)day.Weekday].Add(one.Id);
            }
        }
    }

    public List<int> AtWeekDay(int weekDay)
    {
        return weekdaySchedule[weekDay];
    }

    public FoodBaseInfo GetFoodBaseInfo(int foodId)
    {
        FoodBaseInfo info;
        if (_database.TbFoodInfo.DataMap.TryGetValue(foodId, out info))
        {
            return info;
        }

        return null;
    }
    
}
