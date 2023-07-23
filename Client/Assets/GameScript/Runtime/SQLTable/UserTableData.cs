using System.Collections;
using System.Collections.Generic;
using SQLite;

[Table("UserTableData")]
public class UserTableData
{
    [PrimaryKey]
    [Column("Id")]
    public long Id { get; set; }
    
    [Column("now")]
    public long now { get; set; }
    
    [Column("money")]
    public long money { get; set; }
    
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
public class RestaurantRuntimeData
{
    public List<int> HaveArrivedCustomer;
    public List<int> WaitingCustomer;
    public List<CookResult> cookedMeal;
    public List<int> SoldMenuId;

}

[System.Serializable]
public class CookResult
{
    public int menuId;
    public float CompletePercent;
    public HashSet<cfg.food.flavorTag> Tags;
    public Dictionary<int, bool> QTEResult;//int = QTEId
}

