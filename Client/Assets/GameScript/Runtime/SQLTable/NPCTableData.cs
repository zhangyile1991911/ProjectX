using SQLite;

[Table("NPCTableData")]
public class NPCTableData
{
    [PrimaryKey]
    [Column("Id")]
    public int Id { get; set; }
    [Column("FriendlyValue")]
    public int FriendlyValue { get; set; }
    [Column("AppearCount")]
    public int AppearCount { get; set; }//出现次数
    [Column("PartnerId")]
    public int PartnerId { get; set; }
    [Column("SeatIndex")]
    public int SeatIndex { get; set; }
    [Column("TodayOrderCount")]
    public int TodayOrderCount { get; set; }//今天已经点单的次数
    [Column("AccumulateFriendAtWeek")]
    public int AccumulateFriendAtWeek { get; set; }//本周已获得好感度

    [Column("Behaviour")]
    public int Behaviour { get; set; }

    [Column("Patient")]
    public int patient { get; set; }//耐心值
    
}

[Table("NPCOrderTable")]
public class NPCOrderTable
{
    [PrimaryKey]
    [Column("CharacterId")]
    public int CharacterId { get; set; }
    
    [Column("MenuId")]
    public int MenuId { get; set; }

    [Column("OrderType")]
    public int OrderType { get; set; }

    [Column("Flavor")]
    public string Flavor { get; set; }
}