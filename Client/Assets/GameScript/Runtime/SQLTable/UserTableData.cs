using System.Collections;
using System.Collections.Generic;
using SQLite;
using UnityEngine.Serialization;

[Table("UserTableData")]
public class UserTableData
{
    [PrimaryKey]
    [Column("Id")]
    public long Id { get; set; }
    
    // [Column("now")]
    // public long now { get; set; }
    
    [Column("money")]
    public long money { get; set; }

    [Column("today_weather")]
    public int today_weather { get; set; }
    
    [Column("next_weather")]
    public int next_weather { get; set; }

    [Column("now")]
    public long now { get; set; }
    //这个时间戳是从游戏开始时间2045-1-1开始的
}



[Table("RestaurantTableData")]
public class RestaurantTableData
{
    [PrimaryKey]
    [AutoIncrement]
    [Column("Id")]
    public long Id { get; set; }

    public string RestaurantRuntimeData { get; set; }
}

//记录当前饭店一些状态
//1 当天来过的客人
//3 记录当前还在店里的客人
//2 记录当前已经做好的菜
//4 记录已经卖给客人的菜
[System.Serializable]
public class RestaurantRuntimeData
{
    public List<int> HaveArrivedCustomer;
    public List<int> WaitingCustomers;
    public List<int> SoldMenuId;
}
// [System.Serializable]
// public class WaitingCustomerInfo
// {
//     public int CharacterId;
//     // public int SeatOccupy;
// }
[Table("CookResultTable")]
public class CookResultTable
{
    [Column("MenuId")] 
    public int MenuId{ get; set; }

    [Column("Score")] 
    public float Score { get; set; }

    [Column("Tags")] 
    public string Tags { get; set; }
    
    [Column("QTEResult")]
    public string QTEResult { get; set; }
    
    [Column("CharacterId")] 
    public int characterId { get; set; } //记录卖给谁了

    [PrimaryKey]
    [Column("CreateTimestamp")]
    public long createTimestamp { get; set; }
}

public class CookResult
{
    public int MenuId;
    public float Score;
    public float MaxScore;
    public HashSet<cfg.food.flavorTag> Tags;
    public Dictionary<int, bool> QTEResult;//int = QTEId
    public int characterId;
    public long create_timestamp;
}

//已经学会的菜谱
[Table("OwnMenu")]
public class OwnMenu
{
    [PrimaryKey] [Column("Id")] 
    public int MenuId { get; set; }
    public int level { get; set; } //当前等级
    public int exp { get; set; } //当前经验值
}

