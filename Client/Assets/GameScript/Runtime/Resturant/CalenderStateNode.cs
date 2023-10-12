using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalenderStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
    }

    public void OnEnter(object param = null)
    {
        UIManager.Instance.OpenUI(UIEnum.CalenderWindow, (uiBase) =>
        {
            var calender = uiBase as CalenderWindow;
            calender.ContinueCB = () => { _machine.ChangeState<WaitStateNode>(); };
        }, null);
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _machine.ChangeState<WaitStateNode>();
        }
    }

    public void OnExit()
    {
        UIManager.Instance.CloseUI(UIEnum.CalenderWindow);
    }
}
