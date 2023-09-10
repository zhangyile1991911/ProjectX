using System.Collections;
using System.Collections.Generic;
using cfg.character;
using cfg.common;
using UnityEngine;

public class CharacterScheduler : SingletonModule<CharacterScheduler>
{
    private List<List<int>> weekdaySchedule;
    private Dictionary<int, ScheduleGroup> characterMapSchedule;//key = characterId
    public override void OnCreate(object createParam)
    {
        base.OnCreate(this);

        characterMapSchedule = new Dictionary<int,ScheduleGroup>(50);
        
        weekdaySchedule = new List<List<int>>(7);
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        weekdaySchedule.Add(new List<int>(10));
        
        var characterBaseTb = DataProviderModule.Instance.DataBase.TbBaseInfo;
        foreach (var cid in characterBaseTb.DataMap.Keys)
        {
            var npcData = UserInfoModule.Instance.NPCData(cid);
            int groupId = 0;
            if (npcData == null)
            {
                var characterTb = DataProviderModule.Instance.GetCharacterBaseInfo(cid);
                groupId = characterTb.ScheduleGroup;
            }
            else
            {
                groupId = npcData.ScheduleId;
            }
            
            var scheduleGroup = DataProviderModule.Instance.GetScheduleGroup(groupId);
            if(scheduleGroup == null)continue;
            
            characterMapSchedule[cid] = scheduleGroup;
            foreach (var appearInfo in scheduleGroup.CharacterAppearInfos)
            {
                int weekDayIndex = (int)appearInfo.Weekday - 1;
                //为了解决星期八 这个特殊日期
                if(weekDayIndex >= weekdaySchedule.Count)continue;
                weekdaySchedule[weekDayIndex].Add(cid);
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    // public void AddCharacterSchedule(int characterId,int friendVal)
    // {
    //     var newSchedule = DataProviderModule.Instance.GetCharacterPhaseSchedule(characterId,friendVal);
    //     foreach (var appearInfo in newSchedule.CharacterAppearInfos)
    //     {
    //         int weekDayIndex = (int)appearInfo.Weekday - 1;
    //         weekdaySchedule[weekDayIndex].Add(characterId);
    //     }
    // }

    public ScheduleGroup CharacterScheduleId(int characterId)
    {
        if (characterMapSchedule.TryGetValue(characterId, out var group))
        {
            return group;
        }

        return null;
    }
    
    
    public int UpdateCharacterSchedule(int characterId,int scheduleGroupId,int friendVal)
    {
        var curSchedule = DataProviderModule.Instance.GetCharacterPhaseSchedule(scheduleGroupId);
        var nextSchedule = DataProviderModule.Instance.GetCharacterPhaseSchedule(characterId,friendVal);
        if (curSchedule.Id == nextSchedule.Id) return curSchedule.Id;
        //先清理之前的
        foreach (var appearInfo in curSchedule.CharacterAppearInfos)
        {
            int weekDayIndex = (int)appearInfo.Weekday - 1;
            weekdaySchedule[weekDayIndex].Remove(characterId);
        }

        characterMapSchedule[characterId] = nextSchedule;
        //添加新的
        foreach (var appearInfo in nextSchedule.CharacterAppearInfos)
        {
            int weekDayIndex = (int)appearInfo.Weekday - 1;
            weekdaySchedule[weekDayIndex].Add(characterId);
        }

        return nextSchedule.Id;
    }
    
    public List<int> AppearCharacterIdAtWeekDay(WeekDay weekDay)
    {

        int weekDayIndex = (int)weekDay - 1;
        return weekdaySchedule[weekDayIndex];
    }
    
}
