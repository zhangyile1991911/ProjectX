using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class BeeGameState
{
    protected AirplaneAppWidget control;

    public BeeGameState(AirplaneAppWidget col)
    {
        control = col;
    }
    
    public abstract void Enter();
    public abstract void EveryFrame();
    public abstract void Exit();
}


public class WaitStartState : BeeGameState
{
    public WaitStartState(AirplaneAppWidget col):base(col)
    {
    }
    
    public override void Enter()
    {
        control.ShowStartPage();
        control.BeeGirl.Hide();
        control.XBtn_Start.gameObject.SetActive(true);
        control.XBtn_touch.gameObject.SetActive(false);
    }

    public override void EveryFrame()
    {
        
    }

    public override void Exit()
    {
        control.HideStartPage();
        control.XBtn_Start.gameObject.SetActive(false);
    }
    
    
}

public class StageState : BeeGameState
{
    public StageState(AirplaneAppWidget col):base(col)
    {
        
    }
    
    public override void Enter()
    {
        control.Tran_Scroll.gameObject.SetActive(true);
        control.BeeGirl.Show();
        control.XBtn_touch.gameObject.SetActive(true);
    }

    public override void EveryFrame()
    {
        control.ScrollBG();
        control.UpdateBee();
        control.UpdateFlyingBullet();
        control.UpdateEnemy();
        if (control.Kill_Enemy_Num <= 0)
        {
            control.CurState = new BossState(control);
        }
    }

    public override void Exit()
    {
        
    }
    
}

public class BossState : BeeGameState
{
    public BossState(AirplaneAppWidget col):base(col)
    {
        
    }
    public override void Enter()
    {
        control.Boss.gameObject.SetActive(true);
        control.Boss.ConfigProperty(
            new Vector2(0,1000f),
            new Vector2(),
            control.Level);
    }

    public override void EveryFrame()
    {
        if (control.Boss.IsAlive)
        {
            control.UpdateBoss();     
        }
        else
        {
            control.CurState = new BossExit(control);
        }
    }

    public override void Exit()
    {
        
    }
}


public class BossExit : BeeGameState
{
    public BossExit(AirplaneAppWidget col):base(col)
    {
        
    }
    public override void Enter()
    {
        control.Boss.gameObject.SetActive(true);
    }

    public override void EveryFrame()
    {
        
    }

    public override void Exit()
    {
        
    }
}


