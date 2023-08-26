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
}
