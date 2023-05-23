using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserInfoModule : IModule
{
    public List<int> OwnFoods => _ownFoods.Keys.ToList();
    private Dictionary<int, int> _ownFoods;
    public void OnCreate(object createParam)
    {
        //todo 改成读database数据
        _ownFoods = new Dictionary<int, int>();
        _ownFoods.Add(20001,10);
        _ownFoods.Add(20002,10);
        _ownFoods.Add(20003,10);
        _ownFoods.Add(20004,10);
        _ownFoods.Add(20005,10);
        _ownFoods.Add(20006,10);
        _ownFoods.Add(20007,10);
        _ownFoods.Add(20008,10);
        _ownFoods.Add(20009,10);
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    public int FoodStackNum(int foodId)
    {
        if (_ownFoods.ContainsKey(foodId))
        {
            return _ownFoods[foodId];
        }

        return 0;
    }
}
