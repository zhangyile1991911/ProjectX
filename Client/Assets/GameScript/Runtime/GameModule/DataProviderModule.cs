using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using cfg;
using cfg.character;
using cfg.common;
using cfg.food;
using cfg.item;
using cfg.system;
using Codice.Client.BaseCommands;
using Cysharp.Text;
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
        // initScheduleTable();
        initCharacterBubble();
        initCrowdTable();
        InitDayScheduler();
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

    // public CharacterSchedule GetCharacterScheduler(int characterId)
    // {
    //     if (_database.TbSchedule.DataMap.ContainsKey(characterId))
    //     {
    //         return _database.TbSchedule.DataMap[characterId];
    //     }
    //     
    //     return null;
    // }
    
    

    public ItemBaseInfo GetItemBaseInfo(int itemId)
    {
        if (_database.TbItem.DataMap.ContainsKey(itemId))
        {
            return _database.TbItem.DataMap[itemId];
        }

        return null;
    }

    public FoodMaterial GetFoodMaterial(int foodId)
    {
        if (_database.TbMaterial.DataMap.ContainsKey(foodId))
        {
            return _database.TbMaterial.DataMap[foodId];
        }

        return null;
    }

    public bool IsFood(int itemId)
    {
        var tmp = GetItemBaseInfo(itemId);
        return tmp.Type == itemType.FoodMaterial;
    }
    
    // private List<List<int>> weekdaySchedule;
    // private void initScheduleTable()
    // {
    //     weekdaySchedule = new List<List<int>>(7);
    //     weekdaySchedule.Add(new List<int>(10));
    //     weekdaySchedule.Add(new List<int>(10));
    //     weekdaySchedule.Add(new List<int>(10));
    //     weekdaySchedule.Add(new List<int>(10));
    //     weekdaySchedule.Add(new List<int>(10));
    //     weekdaySchedule.Add(new List<int>(10));
    //     weekdaySchedule.Add(new List<int>(10));
    //     foreach(var one in _database.TbSchedule.DataList)
    //     {
    //         foreach (var day in one.CharacterAppearInfos)
    //         {
    //             weekdaySchedule[(int)day.Weekday-1].Add(one.Id);
    //         }
    //     }
    // }
    
    // public List<int> AtWeekDay(int weekDay)
    // {
    //     return weekdaySchedule[weekDay-1];
    // }
    
    

    private Dictionary<int, List<CharacterBubble>> characterBubbleDict;
    private void initCharacterBubble()
    {
        characterBubbleDict = new Dictionary<int, List<CharacterBubble>>();
        foreach (var one in _database.TbCharacterBubble.DataList)
        {
            if (!characterBubbleDict.TryGetValue(one.NpcId, out var value))
            {
                value = new List<CharacterBubble>();
                characterBubbleDict.Add(one.NpcId,value);
            }
            value.Add(one);
        }
    }

    public List<CharacterBubble> GetCharacterBubbleList(int characterId)
    {
        characterBubbleDict.TryGetValue(characterId, out var value);
        return value;
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

    public BehaviourGroup GetBehaviourGroup(int groupId)
    {
        if (_database.TbBehaviourGroup.DataMap.TryGetValue(groupId, out var group))
        {
            return group;
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

    private Dictionary<WeekDay, List<Crowd>> weekDayToCrowd;
    private void initCrowdTable()
    {
        weekDayToCrowd = new Dictionary<WeekDay, List<Crowd>>(7);
        foreach (var one in _database.TbCword.DataList)
        {
            if (!weekDayToCrowd.ContainsKey(one.WeekDay))
            {
                weekDayToCrowd[one.WeekDay] = new List<Crowd>(10);
            }
            weekDayToCrowd[one.WeekDay].Add(one);
        }
    }

    public List<Crowd> WeekdayCrowd(WeekDay day)
    {
        return weekDayToCrowd[day];
    }

    public Crowd WeekDayHourCrowd(WeekDay day,int hourIn24)
    {
        var tmp = weekDayToCrowd[day];
        foreach (var one in tmp)
        {
            if (one.Hour == hourIn24)
            {
                return one;
            }
        }

        return null;
    }

    private Dictionary<Season, List<DayScheduler>> SeasonScheduler;

    private void InitDayScheduler()
    {
        SeasonScheduler = new Dictionary<Season, List<DayScheduler>>();
        foreach (var one in _database.TbDayScheduler.DataList)
        {
            if (SeasonScheduler.ContainsKey(one.Seaon) == false)
            {
                SeasonScheduler.Add(one.Seaon,new List<DayScheduler>(30));
            }
            SeasonScheduler[one.Seaon].Add(one);
        }
    }
    public Weather DayWeather(Season season,int day)
    {
        var scheduler = SeasonScheduler[season][day];
        var weatherGroup = _database.TbWeatherGroup.DataMap[scheduler.WeatherGroupId];
        int totalWeight = 0;
        totalWeight += weatherGroup.Weather.WeatherWeightA;
        totalWeight += weatherGroup.Weather.WeatherWeightB;
        totalWeight += weatherGroup.Weather.WeatherWeightC;
        totalWeight += weatherGroup.Weather.WeatherWeightD;
        totalWeight += weatherGroup.Weather.WeatherWeightE;

        Random a = new Random(DateTime.UtcNow.Millisecond);
        var result = a.Next(0, totalWeight);
        totalWeight = weatherGroup.Weather.WeatherWeightA;
        if (result <= totalWeight)
        {
            return weatherGroup.Weather.WeatherA;
        }

        if (result >= totalWeight && result <= (totalWeight + weatherGroup.Weather.WeatherWeightB))
        {
            return weatherGroup.Weather.WeatherB;
        }
        totalWeight += weatherGroup.Weather.WeatherWeightB;
        
        if (result >= totalWeight && result <= (totalWeight + weatherGroup.Weather.WeatherWeightC))
        {
            return weatherGroup.Weather.WeatherC;
        }
        totalWeight += weatherGroup.Weather.WeatherWeightC;
        
        if (result >= totalWeight && result <= (totalWeight + weatherGroup.Weather.WeatherWeightD))
        {
            return weatherGroup.Weather.WeatherD;
        }
        totalWeight += weatherGroup.Weather.WeatherWeightE;
        
        if (result >= totalWeight && result <= (totalWeight + weatherGroup.Weather.WeatherWeightE))
        {
            return weatherGroup.Weather.WeatherE;
        }

        return 0;
    }

    public ScheduleGroup GetScheduleGroup(int groupId)
    {
        var exist = _database.TbScheduleGroup.DataMap.ContainsKey(groupId);
        if (exist)
        {
            return _database.TbScheduleGroup.DataMap[groupId];
        }

        return null;
    }

    //通过好感度 找到NPC当前行动组
    public ScheduleGroup GetCharacterPhaseSchedule(int characterId,int friendVal)
    {
        var exist = _database.TbCharacterPhase.DataMap.ContainsKey(characterId);
        if (!exist) return null;
        
        var tb = _database.TbCharacterPhase.DataMap[characterId];
        for (int i = 0;i < tb.Regions.Count;i++)
        {
            if (tb.Regions[i].StartValue >= friendVal && friendVal <= tb.Regions[i].EndValue)
            {
                var groupId = tb.Regions[i].Param;
                return _database.TbScheduleGroup[groupId];
            }
        }

        return null;
    }

    public ScheduleGroup GetCharacterPhaseSchedule(int groupId)
    {
        var exist = _database.TbScheduleGroup.DataMap.ContainsKey(groupId);
        if (exist) return null;
        
        return _database.TbScheduleGroup[groupId];
    }

    public int FriendValLimit()
    {
        return _database.TbGlobalConfig.DataMap["friend_val_limit"].IntVal;
    }

    public int OrderCountLimit()
    {
        return _database.TbGlobalConfig.DataMap["oneday_order_limit"].IntVal;
    }

    public int CustomerEnterInterval()
    {
        return _database.TbGlobalConfig.DataMap["customer_enter_interval"].IntVal;
    }

    public int AttenuatePatientValue()
    {
        return _database.TbGlobalConfig.DataMap["attenuate_patient"].IntVal;
    }
    
    public int MaxPatientValue()
    {
        return _database.TbGlobalConfig.DataMap["patient_max_value"].IntVal;
    }
}
