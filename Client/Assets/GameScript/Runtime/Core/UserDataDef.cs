using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataDef
{
}

public class ItemDataDef
{
    public int Id { get; set; }
    public int Num{ get; set; }
}
// public class BagDataDef
// {
//     public Dictionary<int, ItemDataDef> Items { get; set; }
// }

public class NPCDataDef
{
    public int Id { get; set; }
    public int FriendlyValue { get; set; }
    public HashSet<int> talkedId { get; set; }
}
