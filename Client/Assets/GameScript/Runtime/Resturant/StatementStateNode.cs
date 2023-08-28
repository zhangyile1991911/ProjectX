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
        var openData = new FlowControlWindowData();
        openData.StateMachine = _machine;
        UIManager.Instance.OpenUI(UIEnum.RestaurantStatementWindow, null, openData);
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
        UIManager.Instance.CloseUI(UIEnum.RestaurantStatementWindow);
        
        //清理角色占座
        var characters = CharacterMgr.Instance.Characters;
        foreach (var chara in characters)
        {
            _restaurant.CharacterReturnSeat(chara.SeatOccupy);
        }
        //清理角色资源
        CharacterMgr.Instance.ClearAllCharacter();
        //清理下单条
        UIManager.Instance.DestroyUI(UIEnum.OrderQueueWindow);
        
        //计算收入
        calculate();
        UserInfoModule.Instance.ClearRestaurantData();
        //日期往前走
        Clocker.Instance.MoveToNextDay();
        _restaurant.CloseRestaurant();
    }

    private void calculate()
    {//结算
        var soldMenuId = UserInfoModule.Instance.GetRestaurantSoldMenuId();
        int total = 0;
        foreach (var id in soldMenuId)
        {
            var itemTb = DataProviderModule.Instance.GetItemBaseInfo(id);
            total += itemTb.Sell;
        }

        UserInfoModule.Instance.AddMoney(total);
        UserInfoModule.Instance.ClearRestaurantData();
    }
}
