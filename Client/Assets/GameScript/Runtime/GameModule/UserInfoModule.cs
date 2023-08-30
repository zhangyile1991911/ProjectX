using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cfg.common;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using SQLite;
using UniRx;
using Unity.Collections;
using UnityEngine.PlayerLoop;

public class UserInfoModule : SingletonModule<UserInfoModule>
{
    public List<ItemTableData> OwnFoodMaterials { get; private set; }
    public List<OwnMenu> OwnMenus { get; private set; }

    public long Now => _userTableData.now;

    //--------------------------------------------------------------------
    private SQLiteConnection _sqLite;

    private UserTableData _userTableData;

    private Dictionary<int, NPCTableData> _npcTableDatas;

    private Dictionary<int, ItemTableData> _itemTableDatas;

    private Dictionary<int, DialogueTableData> _dialogueTableDatas;

    private Dictionary<int, OwnMenu> _ownMenuTableDatas;
    private RestaurantTableData _restaurantTableData;
    private RestaurantRuntimeData _restaurantRuntimeData;
    public List<int> RestaurantWaitingCharacter => _restaurantRuntimeData.WaitingCustomers;
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
        _sqLite.CreateTable<OwnMenu>();

        var userQuery = $"SELECT * FROM UserTableData;";
        _userTableData = _sqLite.Query<UserTableData>(userQuery).FirstOrDefault();
        if (_userTableData == null)
        {
            _userTableData = new ();
            _userTableData.Id = 11111;
            _userTableData.now = 0;
            _userTableData.money = 500;
            // _userTableData.season = (int)Season.Spring;
            // _userTableData.day_of_month = 0;
            // _userTableData.second_of_day = 20*60*60;
            _userTableData.today_weather = (int)Weather.Sunshine;
            _userTableData.next_weather = (int)Weather.Sunshine;
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

        var ownMenuQuery = $"select * from OwnMenu";
        OwnMenus = _sqLite.Query<OwnMenu>(ownMenuQuery);
        initOwnMenuData(OwnMenus);

        // EventModule.Instance.ToNextWeekSub.Subscribe(ResetNPCWeekDay);
        // EventModule.Instance.ToNextDaySub.Subscribe(ResetNPCDaily);
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

    private void initNPC(List<NPCTableData> npcList)
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

    private void initDialogue(List<DialogueTableData> dialogueList)
    {
        _dialogueTableDatas ??= new Dictionary<int, DialogueTableData>();
        foreach (var one in dialogueList)
        {
            _dialogueTableDatas.Add(one.Id,one);
        }
    }

    private void initRestaurantData()
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
            _restaurantRuntimeData.WaitingCustomers = new List<int>();
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

    private void initOwnMenuData(List<OwnMenu> ownMenus)
    {
        _ownMenuTableDatas ??= new Dictionary<int, OwnMenu>(20);
        
        if (ownMenus.Count > 0)
        {
            foreach (var menu in ownMenus)
            {
                _ownMenuTableDatas.Add(menu.MenuId,menu);
            }

            return;
        }
        /*10001 10002 10003 10004 10005*/
        // var one = new OwnMenu()
        // {
        //     MenuId = 10001,
        //     level = 1,
        //     exp = 0
        // };
        // _ownMenuTableDatas.Add(one.MenuId,one);
        // OwnMenus.Add(one);
        var two = new OwnMenu()
        {
            MenuId = 10002,
            level = 1,
            exp = 0
        };
        _ownMenuTableDatas.Add(two.MenuId,two);
        OwnMenus.Add(two);
        // var three = new OwnMenu()
        // {
        //     MenuId = 10003,
        //     level = 1,
        //     exp = 0
        // };
        // _ownMenuTableDatas.Add(three.MenuId,three);
        // OwnMenus.Add(one);
        // var four = new OwnMenu()
        // {
        //     MenuId = 10004,
        //     level = 1,
        //     exp = 0
        // };
        // _ownMenuTableDatas.Add(four.MenuId,four);
        // OwnMenus.Add(one);
        var five = new OwnMenu()
        {
            MenuId = 10005,
            level = 1,
            exp = 0
        };
        _ownMenuTableDatas.Add(five.MenuId,five);
        OwnMenus.Add(five);
        _sqLite.InsertAll(_ownMenuTableDatas.Values);
    }

    public void AddNewMenu(int menuId)
    {
        var newMenu = new OwnMenu();
        newMenu.MenuId = menuId;
        newMenu.level = 1;
        newMenu.exp = 0;
        _ownMenuTableDatas.Add(menuId,newMenu);
        OwnMenus.Add(newMenu);
        _sqLite.Insert(newMenu);
    }

    public bool IsMenuExist(int menuId)
    {
        return _ownMenuTableDatas.ContainsKey(menuId);
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

    public void AddCharacterArrivedAndWaiting(int characterId)
    {
        var count = _restaurantRuntimeData.HaveArrivedCustomer.Count(one=>one == characterId);
        if (count <= 0)
        {
            _restaurantRuntimeData.HaveArrivedCustomer.Add(characterId);
            updateRestaurantRuntimeData();
        }
        count = _restaurantRuntimeData.WaitingCustomers.Count(one=>one == characterId);
        if (count <= 0)
        {
            _restaurantRuntimeData.WaitingCustomers.Add(characterId);
            updateRestaurantRuntimeData();
        }
    }
    

    public bool GetWaitingCharacter(int characterId)
    {
        return _restaurantRuntimeData.WaitingCustomers.Count(one => one == characterId) > 0;
    }

    public void RemoveWaitingCharacter(int characterId)
    {
        _restaurantRuntimeData.WaitingCustomers.Remove(characterId);
        updateRestaurantRuntimeData();
        // foreach (var one in _restaurantRuntimeData.WaitingCustomers)
        // {
        //     if (one.CharacterId == characterId)
        //     {
        //         _restaurantRuntimeData.WaitingCustomers.Remove(one);
        //         updateRestaurantRuntimeData();
        //         break;
        //     }
        // }
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
        newNpc.patient = 0;
        newNpc.Behaviour = 0;
        _npcTableDatas.Add(npcId,newNpc);
        _sqLite.Insert(newNpc);
        return newNpc;
    }
    
    public void UpdateNPCData(int npcId)
    {
        var d = NPCData(npcId);
        if (d == null) return;
        _sqLite.Update(d);
    }

    // private void ResetNPCWeekDay(GameDateTime gameDateTime)
    // {
    //     foreach(var one in _npcTableDatas.Values)
    //     {
    //         one.AccumulateFriendAtWeek = 0;
    //     }
    // }

    // private void ResetNPCDaily(GameDateTime time)
    // {
    //     foreach(var one in _npcTableDatas.Values)
    //     {
    //         one.TodayOrderCount = 0;
    //     }
    // }
    

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

    public Weather TodayWeather()
    {
        return (Weather)_userTableData.today_weather;
    }

    public void RefreshWeather()
    {
        _userTableData.today_weather = _userTableData.next_weather;
        
        _userTableData.next_weather = (int)DataProviderModule._instance.DayWeather(
            Clocker.Instance.NowDateTime.Season,
            (int)Clocker.Instance.NowDateTime.Day);
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
