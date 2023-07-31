using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using SQLite;
using Unity.Collections;
using UnityEngine.PlayerLoop;

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

    private RestaurantTableData _restaurantTableData;
    private RestaurantRuntimeData _restaurantRuntimeData;
    public List<WaitingCustomerInfo> RestaurantWaitingCharacter => _restaurantRuntimeData.WaitingCustomers;
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
        _sqLite.CreateTable<RestaurantTableData>();

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

        var restaurant = $"select * from RestaurantTableData";
        _restaurantTableData =_sqLite.Query<RestaurantTableData>(restaurant).FirstOrDefault();
        initRestaurantData();
        
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

    public void initRestaurantData()
    {
        if (_restaurantTableData == null)
        {
            _restaurantTableData = new RestaurantTableData();
            _restaurantTableData.Id = 1;
        }

        if (string.IsNullOrEmpty(_restaurantTableData.RestaurantRuntimeData))
        {
            _restaurantRuntimeData = new RestaurantRuntimeData();
            _restaurantRuntimeData.HaveArrivedCustomer = new List<int>();
            _restaurantRuntimeData.WaitingCustomers = new List<WaitingCustomerInfo>();
            _restaurantRuntimeData.cookedMeal = new List<CookResult>();
            _restaurantRuntimeData.SoldMenuId = new List<int>();
            _restaurantTableData.RestaurantRuntimeData = JsonUtility.ToJson(_restaurantRuntimeData);
            _sqLite.Insert(_restaurantTableData);
        }
        else
        {
            _restaurantRuntimeData = JsonUtility.FromJson<RestaurantRuntimeData>(_restaurantTableData.RestaurantRuntimeData);    
        }
    }

    public void ClearRestaurantData()
    {
        _restaurantRuntimeData.SoldMenuId.Clear();
        _restaurantRuntimeData.WaitingCustomers.Clear();
        _restaurantRuntimeData.cookedMeal.Clear();
        _restaurantRuntimeData.HaveArrivedCustomer.Clear();
        updateRestaurantRuntimeData();
    }
    
    public List<int> GetRestaurantSoldMenuId()
    {
        return _restaurantRuntimeData.SoldMenuId;
    }

    public bool RestaurantCharacterArrived(int characterId)
    {
        return _restaurantRuntimeData.HaveArrivedCustomer.Count((one)=> one == characterId) > 0;
    }

    public void AddCharacterArrived(int characterId)
    {
        var count = _restaurantRuntimeData.HaveArrivedCustomer.Count(one=>one == characterId);
        if (count <= 0)
        {
            _restaurantRuntimeData.HaveArrivedCustomer.Add(characterId);
            updateRestaurantRuntimeData();
        }
    }

    public void AddWaitingCharacter(int characterId,int seatOccupy)
    {
        var count = _restaurantRuntimeData.WaitingCustomers.Count(one=>one.CharacterId == characterId);
        if (count <= 0)
        {
            _restaurantRuntimeData.WaitingCustomers.Add(new WaitingCustomerInfo()
            {
                CharacterId = characterId,
                SeatOccupy = seatOccupy
            });
            updateRestaurantRuntimeData();
        }
    }

    public WaitingCustomerInfo GetWaitingCharacter(int characterId)
    {
        var result = _restaurantRuntimeData.WaitingCustomers.Find(one => one.CharacterId == characterId);
        return result;
    }

    public void RemoveWaitingCharacter(int characterId)
    {
        foreach (var one in _restaurantRuntimeData.WaitingCustomers)
        {
            if (one.CharacterId == characterId)
            {
                _restaurantRuntimeData.WaitingCustomers.Remove(one);
                updateRestaurantRuntimeData();
                break;
            }
        }
    }

    public List<CookResult> CookResults => _restaurantRuntimeData.cookedMeal;
    public void AddCookedMeal(CookResult oneMeal)
    {
        if (oneMeal == null) return;
        
        _restaurantRuntimeData.cookedMeal.Add(oneMeal);
    }

    public void RemoveCookedMeal(CookResult oneMeal)
    {
        if (oneMeal == null) return;
        _restaurantRuntimeData.cookedMeal.Remove(oneMeal);
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

    public NPCTableData NPCData(int npcId)
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
        _userTableData.now += sec*1000;
        _sqLite.Update(_userTableData);
    }

    public void AddMoney(int num)
    {
        if (num > 0)
        {
            _userTableData.money += num;
        }
        else if (num < 0)
        {
            if (_userTableData.money > Mathf.Abs(num))
            {
                _userTableData.money += num;
            }
        }

        _sqLite.Update(_userTableData);
    }
    

    public void UpdateNPCData(int npcId)
    {
        var d = NPCData(npcId);
        if (d == null) return;
        _sqLite.Update(d);
    }
    
    void updateRestaurantRuntimeData()
    {//todo 这个地方可能是个性能点 考虑用异步完成
        _restaurantTableData.RestaurantRuntimeData = JsonUtility.ToJson(_restaurantRuntimeData);
        _sqLite.Update(_restaurantTableData);
    }
 
    public void SoldMealId(int menuId)
    {
        _restaurantRuntimeData.SoldMenuId ??= new List<int>();
        _restaurantRuntimeData.SoldMenuId.Add(menuId);
        
        updateRestaurantRuntimeData();
    }
    

    public List<int> SoldMealIdList()
    {
        return _restaurantRuntimeData.SoldMenuId;
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
