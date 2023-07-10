using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SQLite;

public class UserInfoModule : SingletonModule<UserInfoModule>
{
    public List<ItemTableData> OwnFoodMaterials { get; private set; }

    public long Now => _userTableData.now;

    //--------------------------------------------------------------------
    private SQLiteConnection _sqLite;

    private UserTableData _userTableData;

    private Dictionary<int, NPCTableData> _npcTableDatas;

    private Dictionary<int, ItemTableData> _itemTableDatas;

    private Dictionary<int, DialogueTableData> _dialogueTableDatas;
    //--------------------------------------------------------------------
    public override void OnCreate(object createParam)
    {
        base.OnCreate(this);
        //连接数据库
        var dbPath = Application.dataPath + "/userDB.db";
        Debug.Log($"dbPath = {dbPath}");
        var options = new SQLiteConnectionString(dbPath, false);
        _sqLite = new SQLiteConnection(options);
        initPlayerData();
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        // _liteDatabase.Dispose();
        _sqLite.Close();
    }

    private void initPlayerData()
    {
        _sqLite.CreateTable<UserTableData>();
        _sqLite.CreateTable<NPCTableData>();
        _sqLite.CreateTable<ItemTableData>();
        _sqLite.CreateTable<DialogueTableData>();

        var userQuery = $"SELECT * FROM UserTableData;";
        _userTableData = _sqLite.Query<UserTableData>(userQuery).FirstOrDefault();
        if (_userTableData == null)
        {
            _userTableData = new ();
            _userTableData.Id = 11111;
            _userTableData.now = 1682155171369;
            _userTableData.money = 500;
            _sqLite.Insert(_userTableData);
        }

        var npcQuery = $"select * from NPCTableData;";
        var npcList = _sqLite.Query<NPCTableData>(npcQuery);
        initNPC(npcList);
        
        var itemQuery = $"select * from ItemTableData;";
        var itemList = _sqLite.Query<ItemTableData>(itemQuery);
        initItem(itemList);

        var dialogue = $"select * from DialogueTableData";
        var dialogueList = _sqLite.Query<DialogueTableData>(dialogue);
        initDialogue(dialogueList);
        
    }

    private void initItem(List<ItemTableData> items)
    {
        if (items.Count <= 0)
        {
            items.Add(new ItemTableData(){Id = 1001,Num = 10});
            items.Add(new ItemTableData(){Id = 1002,Num = 10});
            items.Add(new ItemTableData(){Id = 1003,Num = 10});
            items.Add(new ItemTableData(){Id = 1004,Num = 10});
            items.Add(new ItemTableData(){Id = 1005,Num = 10});
            items.Add(new ItemTableData(){Id = 1006,Num = 10});
            _sqLite.InsertAll(items);
        }

        _itemTableDatas ??= new Dictionary<int, ItemTableData>();
        OwnFoodMaterials ??= new(10);
        foreach (var one in items)
        {
            _itemTableDatas.Add(one.Id,one);
            if (DataProviderModule.Instance.IsFood(one.Id))
            {
                OwnFoodMaterials.Add(one);
            }
        }
    }

    public void initNPC(List<NPCTableData> npcList)
    {
        _npcTableDatas ??= new();
        if (npcList.Count <= 0)
        {
            return;
        }
        
        foreach (var one in npcList)
        {
            _npcTableDatas.Add(one.Id,one);
        }
        
    }

    public void initDialogue(List<DialogueTableData> dialogueList)
    {
        _dialogueTableDatas ??= new Dictionary<int, DialogueTableData>();
        foreach (var one in dialogueList)
        {
            _dialogueTableDatas.Add(one.Id,one);
        }
    }
    
    public bool IsEnoughItem(int itemId,int needNum)
    {
        int curNum = ItemNum(itemId);
        return curNum >= needNum;
    }
    
    public int ItemNum(int itemId)
    {
        if (_itemTableDatas.TryGetValue(itemId, out var data))
        {
            return data.Num;
        }

        return 0;
    }

    public void AddItemNum(int itemId,uint num)
    {
        if (_itemTableDatas.TryGetValue(itemId, out var data))
        {
            data.Num += (int)num;
            _sqLite.Update(data);
        }
        else
        {
            var tmp = new ItemTableData() { Id = itemId, Num = (int)num };
            _itemTableDatas.Add(itemId,tmp);
            if (DataProviderModule.Instance.IsFood(itemId))
            {
                OwnFoodMaterials.Add(tmp);
            }
            _sqLite.Insert(tmp);
        }
    }

    public bool SubItemNum(int itemId,uint num)
    {
        if (!_itemTableDatas.TryGetValue(itemId, out var data))
        {
            return false;
        }

        if (data.Num > num)
        {
            data.Num -= (int)num;
            _sqLite.Update(data);
            return true;
        }
        else if (data.Num == num)
        {
            _itemTableDatas.Remove(itemId);
            if (DataProviderModule.Instance.IsFood(itemId))
            {
                OwnFoodMaterials.Remove(data);
            }
            _sqLite.Delete(data);
            return true;
        }

        return false;
    }

    public NPCTableData NpcData(int npcId)
    {
        if (_npcTableDatas.TryGetValue(npcId, out var data))
        {
            return data;
        }

        var newNpc = new NPCTableData();
        newNpc.Id = npcId;
        newNpc.FriendlyValue = 0;
        newNpc.AppearCount = 0;
        _npcTableDatas.Add(npcId,newNpc);
        _sqLite.Insert(newNpc);
        return newNpc;
    }

    public bool HaveReadDialogueId(int did)
    {
        return _dialogueTableDatas.ContainsKey(did);
    }

    public void InsertReadDialogueId(int did)
    {
        if (_dialogueTableDatas.ContainsKey(did))
            return;
        var newDialogue = new DialogueTableData();
        newDialogue.Id = did;
        _dialogueTableDatas.Add(did,newDialogue);
        _sqLite.Insert(newDialogue);
    }

    public void AddSecond(int sec)
    {
        _userTableData.now += sec;
        _sqLite.Update(_userTableData);
    }
    // public void SaveAllData()
    // {
    //     updateUserTableData();
    //     updateDialogueTableData();
    //     updateNPCTableData();
    //     updateItemTableData();
    // }
    //
    // private void updateUserTableData()
    // {
    //     _sqLite.Update(_userTableData);
    // }
    //
    // private void updateDialogueTableData()
    // {
    //     //todo 可能有性能问题 只更新需要更新的
    //     _sqLite.UpdateAll(_dialogueTableDatas.Values);
    // }
    //
    // private void updateNPCTableData()
    // {
    //     //todo 可能有性能问题 只更新需要更新的
    //     _sqLite.UpdateAll(_npcTableDatas.Values);
    // }
    //
    // private void updateItemTableData()
    // {
    //     //todo 可能有性能问题 只更新需要更新的        
    //     _sqLite.UpdateAll(_itemTableDatas.Values);
    // }
}
