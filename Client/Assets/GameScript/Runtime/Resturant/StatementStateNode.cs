using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatementStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
    }

    public void OnEnter(object param)
    {
        UIManager.Instance.OpenUI(UIEnum.RestaurantStatementWindow, null, null);
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        UIManager.Instance.CloseUI(UIEnum.RestaurantStatementWindow);
    }
}
