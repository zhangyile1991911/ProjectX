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
