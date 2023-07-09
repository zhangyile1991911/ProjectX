using System.Collections;
using System.Collections.Generic;
using SQLite;

[Table("ItemTableData")]
public class ItemTableData
{
    [PrimaryKey]
    [Column("Id")]
    public int Id { get; set; }
    [Column("Num")]
    public int Num { get; set; }
}

