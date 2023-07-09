using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using cfg;
using cfg.character;
using cfg.food;
using cfg.item;
using Cysharp.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimpleJSON;
using UnityEngine;
using Random = System.Random;

public class DataProviderModule : SingletonModule<DataProviderModule>
{
    public cfg.Tables DataBase=>_database;
    private cfg.Tables _database;
    public override void OnCreate(object createParam)
    {
#if UNITY_EDITOR
        _database = new cfg.Tables(LoadJsonBuf);
#endif
        initScheduleTable();
        base.OnCreate(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
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

    public ItemBaseInfo GetItemBaseInfo(int itemId)
    {
        if (_database.TbItem.DataMap.ContainsKey(itemId))
        {
            return _database.TbItem.DataMap[itemId];
        }

        return null;
    }

    public bool IsFood(int itemId)
    {
        var tmp = GetItemBaseInfo(itemId);
        return tmp.Type == itemType.FoodMaterial;
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

    public FoodMaterial GetFoodBaseInfo(int foodId)
    {
        if (_database.TbMaterial.DataMap.TryGetValue(foodId, out var info))
        {
            return info;
        }

        return null;
    }

    public CharacterBubble GetCharacterBubble(int chatId)
    {
        if (_database.TbCharacterBubble.DataMap.TryGetValue(chatId, out var bubble))
        {
            return bubble;
        }

        return null;
    }

    public MenuInfo GetMenuInfo(int menuId)
    {
        if (_database.TbMenuInfo.DataMap.TryGetValue(menuId, out var menuInfo))
        {
            return menuInfo;
        }

        return null;
    }

    public QTEInfo GetQTEInfo(int qteId)
    {
        if (_database.TbQTE.DataMap.TryGetValue(qteId, out var qteInfo))
        {
            return qteInfo;
        }

        return null;
    }

    public int GetQTEGroupId()
    {
        var r = new Random();
        var result = r.Next(0, _database.TbQTEGroup.DataList.Count);
        return _database.TbQTEGroup.DataList[result].Id;
    }

    public List<qte_info> GetQTEGroupInfo(int groupId)
    {
        if (_database.TbQTEGroup.DataMap.TryGetValue(groupId, out var value))
        {
            return value.QteAppearInfos;
        }

        return null;
    }

    public CookPrefabInfo GetCookPrefabInfo(cookTools tools)
    {
        foreach (var one in _database.TbCookPrefab.DataList)
        {
            if (one.Tools == tools)
                return one;
        }

        return null;
    }
    

}
