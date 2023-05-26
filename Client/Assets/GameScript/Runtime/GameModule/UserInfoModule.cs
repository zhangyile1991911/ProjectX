using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using UnityEngine;

public class UserInfoModule : IModule
{
    public List<int> OwnFoods => _ownFoods.Keys.ToList();
    private Dictionary<int, int> _ownFoods;

    private LiteDatabase _liteDatabase;
    private ILiteCollection<ItemDataDef> _bagDatas;
    private ILiteCollection<NPCDataDef> _npcDatas;
    public void OnCreate(object createParam)
    {
        //todo 改成读database数据
        // _ownFoods = new Dictionary<int, int>();
        // _ownFoods.Add(20001,10);
        // _ownFoods.Add(20002,10);
        // _ownFoods.Add(20003,10);
        // _ownFoods.Add(20004,10);
        // _ownFoods.Add(20005,10);
        // _ownFoods.Add(20006,10);
        // _ownFoods.Add(20007,10);
        // _ownFoods.Add(20008,10);
        // _ownFoods.Add(20009,10);
        _liteDatabase = new LiteDatabase(Application.dataPath + "/playerDB");
        initPlayerData();
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        _liteDatabase.Dispose();
    }

    private void initPlayerData()
    {
        _bagDatas = _liteDatabase.GetCollection<ItemDataDef>();
        _npcDatas = _liteDatabase.GetCollection<NPCDataDef>();
    }

    public bool IsEnoughItem(int itemId,int needNum)
    {
        int curNum = ItemNum(itemId);
        return curNum >= needNum;
    }
    
    public int ItemNum(int itemId)
    {
        var result = _bagDatas.FindOne(x => x.Id == itemId);
        return result?.Num ?? 0;
    }

    public void AddItemNum(int itemId,int add)
    {
        var result = _bagDatas.FindOne(x => x.Id == itemId);
        result.Num += add;
        UpdateItem(result);
    }
    private void UpdateItem(ItemDataDef item)
    {
        _bagDatas.Upsert(item);
    }
    
    public NPCDataDef NpcData(int npcId)
    {
        var result = _npcDatas.FindOne(x => x.Id == npcId);
        return result;
    }
    
    // public int FoodStackNum(int foodId)
    // {
    //     if (_ownFoods.ContainsKey(foodId))
    //     {
    //         return _ownFoods[foodId];
    //     }
    //
    //     return 0;
    // }
}
