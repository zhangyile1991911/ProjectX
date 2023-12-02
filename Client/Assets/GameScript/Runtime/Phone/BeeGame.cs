using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Cysharp.Threading.Tasks.Triggers;
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
        control.Tran_UI.gameObject.SetActive(true);
    }

    public override void EveryFrame()
    {
        control.ScrollBG();
        control.UpdateBee();
        control.UpdateFlyingBullet();
        control.UpdateEnemy();
        if (control.Kill_Enemy_Num <= 0)
        {
            control.CurState = new BossStateEnter(control);
        }
    }

    public override void Exit()
    {

    }
}

public class BossStateEnter : BeeGameState
{
    public BossStateEnter(AirplaneAppWidget col):base(col)
    {
        
    }
    
    public override void Enter()
    {
        control.Boss.Show();
        control.Boss.ConfigProperty(
            new Vector2(0,1000f),
            new Vector2(0,260f),
            control.Level);
        
        control.BeeGirl.SetDestination(new Vector2(0,-300f));
        control.XBtn_touch.gameObject.SetActive(false);
        control.RecycleAllBullet();
        control.RecycleAllEnemy();
        
        control.Boss.ShowDialogue("哦~~~嚯嚯嚯嚯");
        
    }
    
    public override void EveryFrame()
    {
        bool boss = control.Boss.Move();
        bool girl = control.BeeGirl.Move();
        if (!boss && !girl)
        {
            control.CurState = new BossState(control);
        }
    }

    public override void Exit()
    {
        control.AddDeferTask(2, (p) =>
        {
            control.Boss.HideDialogue();
        });
    }
}

public class BossState : BeeGameState
{
    private float wait_time;
    public BossState(AirplaneAppWidget col):base(col)
    {
        
    }
    public override void Enter()
    {
        control.XBtn_touch.gameObject.SetActive(true);
        control.Boss.Show();
    }

    public override void EveryFrame()
    {
        if (!control.Boss.IsAlive)
        {
            control.ScorePub.Value += 5;
            control.CurState = new BossBeforeExit(control);
        }
        
        control.ScrollBG();
        control.UpdateBee();
        control.UpdateFlyingBullet();
        // control.UpdateEnemy();
        control.UpdateBoss();
        wait_time -= Time.deltaTime;
        if (wait_time <= 0f)
        {
            wait_time = Random.Range(2f, 3f);
            float x = Random.Range(500f/2f,-500f/2f);
            float y = Random.Range(0,500f);
            control.Boss.SetDestination(new Vector2(x,y));
        }
    }

    public override void Exit()
    {
        control.RecycleAllBullet();
        control.RecycleAllEnemy();
        control.XBtn_touch.gameObject.SetActive(true);
    }
}


public class BossBeforeExit : BeeGameState
{
    public BossBeforeExit(AirplaneAppWidget col):base(col)
    {
        
    }
    public override void Enter()
    {
        control.Boss.Show();
        control.XBtn_touch.gameObject.SetActive(false);
        
        control.Boss.SetDestination(new Vector2(0,260f));
        control.BeeGirl.SetDestination(new Vector2(0,-300f));
        control.Boss.ShowDialogue("我会回来的!哦~嚯");
    }

    public override void EveryFrame()
    {
        if (!control.Boss.Move() && !control.BeeGirl.Move())
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
        control.Boss.Show();
        control.XBtn_touch.gameObject.SetActive(false);
        
        control.Boss.SetDestination(new Vector2(0,1000f));
        control.BeeGirl.SetDestination(new Vector2(0,-300f));
    }

    public override void EveryFrame()
    {
        if (!control.Boss.Move())
        {
            control.CurState = new StageState(control);
        }
    }

    public override void Exit()
    {
        control.Boss.Hide();
        control.Boss.HideDialogue();
        control.NextLevel();
    }
}


public class BeeGameOver : BeeGameState
{
    public BeeGameOver(AirplaneAppWidget col):base(col)
    {
        
    }
    public override void Enter()
    {
        control.RecycleAllBullet();
        control.RecycleAllEnemy();
        control.XBtn_touch.gameObject.SetActive(false);
        control.Tran_Scroll.gameObject.SetActive(false);
        control.BeeGirl.Hide();
        control.Boss.Hide();
        
        control.ShowResult();
        control.BeeGirl.Reset();
        control.Boss.Reset();
        
        control.Tran_UI.gameObject.SetActive(false);
    }

    public override void EveryFrame()
    {
       
    }

    public override void Exit()
    {
        control.HideResult();
    }
}
