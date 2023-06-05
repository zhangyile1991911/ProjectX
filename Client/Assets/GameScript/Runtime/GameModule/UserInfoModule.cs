using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.item;
using LiteDB;
using UnityEngine;

public class UserInfoModule : SingletonModule<UserInfoModule>
{
    public List<ItemDataDef> OwnFoodMaterials => _foodmaterialDatas.Values.ToList();
    // private Dictionary<int, int> _ownFoods;
    
    private LiteDatabase _liteDatabase;
    private ILiteCollection<ItemDataDef> _bagDatas;
    private ILiteCollection<NPCDataDef> _npcDatas;
    private ILiteCollection<DialogueDataCollection> _dialogueCollection;


    private Dictionary<int,ItemDataDef> _foodmaterialDatas;//key = itemId
    public override void OnCreate(object createParam)
    {
        //连接数据库
        _liteDatabase = new LiteDatabase(Application.dataPath + "/playerDB");

        _foodmaterialDatas = new Dictionary<int,ItemDataDef>();
        
        initPlayerData();
        base.OnCreate(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _liteDatabase.Dispose();
    }

    private void initPlayerData()
    {
        var exist = _liteDatabase.CollectionExists("ItemDataDef");
        _bagDatas = _liteDatabase.GetCollection<ItemDataDef>();
        if (!exist)
        {
            initBagData();
        }
        _npcDatas = _liteDatabase.GetCollection<NPCDataDef>();

        exist = _liteDatabase.CollectionExists("DialogueDataCollection");
        _dialogueCollection = _liteDatabase.GetCollection<DialogueDataCollection>();
        if (!exist)
        {
            var newDialogueData = new DialogueDataCollection();
            newDialogueData.ReadDialogue = new Dictionary<int, OneDialogueDef>();
            _dialogueCollection.Insert(newDialogueData);
        }
        classifyItem();
    }

    private void initBagData()
    {
        ItemDataDef a = new ItemDataDef()
        {
            Id = 1001,
            Num = 10,
        };
        _bagDatas.Insert(a);
        ItemDataDef b = new ItemDataDef()
        {
            Id = 1002,
            Num = 10,
        };
        _bagDatas.Insert(b);
        ItemDataDef c = new ItemDataDef()
        {
            Id = 1003,
            Num = 10,
        };
        _bagDatas.Insert(c);
        ItemDataDef d = new ItemDataDef()
        {
            Id = 1004,
            Num = 10,
        };
        _bagDatas.Insert(d);
        ItemDataDef e = new ItemDataDef()
        {
            Id = 1005,
            Num = 10,
        };
        _bagDatas.Insert(e);
        ItemDataDef f = new ItemDataDef()
        {
            Id = 1006,
            Num = 10,
        };
        _bagDatas.Insert(f);
        
        _foodmaterialDatas.Add(a.Id,a);
        _foodmaterialDatas.Add(b.Id,b);
        _foodmaterialDatas.Add(c.Id,c);
        _foodmaterialDatas.Add(d.Id,d);
        _foodmaterialDatas.Add(e.Id,e);
        _foodmaterialDatas.Add(f.Id,f);
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
        var tmp = result.Num + add;
        if (tmp > 0)
        {
            result.Num = tmp;
            UpdateItem(result);
            if(!_foodmaterialDatas.ContainsKey(itemId))_foodmaterialDatas.Add(itemId,result);
        }
        else
        {
            _bagDatas.Delete(itemId);
            if(_foodmaterialDatas.ContainsKey(itemId)) _foodmaterialDatas.Remove(itemId);
        }
    }
    
    private void UpdateItem(ItemDataDef item)
    {
        _bagDatas.Upsert(item);
    }

    private void classifyItem()
    {
        var p = UniModule.GetModule<DataProviderModule>();
        foreach (var item in _bagDatas.FindAll())
        {
            var info = p.GetItemBaseInfo(item.Id);
            if (info == null) return;
            if (info.Type != itemType.FoodMaterial) return;
            _foodmaterialDatas[item.Id] = item;
        }
        
    }
    // private void AddFoodItem(ItemDataDef item)
    // {
    //     var p = UniModule.GetModule<DataProviderModule>();
    //     var info = p.GetItemBaseInfo(item.Id);
    //     if (info == null) return;
    //     if (info.Type != itemType.FoodMaterial) return;
    //     if (_foodDatas.ContainsKey(item.Id))
    //     {
    //         _foodDatas[item.Id] = item;    
    //     }
    //     
    // }

    // private void ReduceFoodItem(ItemDataDef item)
    // {
    //     
    // }
    
    public NPCDataDef NpcData(int npcId)
    {
        var result = _npcDatas.FindOne(x => x.Id == npcId);
        if (result == null)
        {
            result = new NPCDataDef();
            result.Id = npcId;
            result.FriendlyValue = 0;
            _npcDatas.Insert(result);
        }
        return result;
    }

    public bool HaveReadDialogueId(int did)
    {
        var tmp = _dialogueCollection.FindOne(x=> true);
        return tmp.ReadDialogue.ContainsKey(did);
    }

    public void InsertReadDialogueId(int did)
    {
        var tmp = _dialogueCollection.FindOne(x=> true);
        tmp.ReadDialogue.Add(did,new OneDialogueDef());
    }

    public void SaveAllData()
    {
        
        // private ILiteCollection<ItemDataDef> _bagDatas;
        // private ILiteCollection<NPCDataDef> _npcDatas;
        // private ILiteCollection<DialogueDataCollection> _dialogueCollection;
    }
}
