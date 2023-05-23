using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private UIManager _uiManager;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
    }

    public void OnEnter(object param)
    {
        _uiManager = UniModule.GetModule<UIManager>();
        _uiManager.OpenUI(UIEnum.KitchenWindow, (uiBase)=>{}, null);
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        _uiManager.CloseUI(UIEnum.KitchenWindow);
    }
}
