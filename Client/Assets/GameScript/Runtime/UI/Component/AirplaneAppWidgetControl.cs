using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Text;
using DG.Tweening;
using UniRx;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using YooAsset;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class AirplaneAppWidget : BaseAppWidget
{
    private float maxY;
    private float maxX;
    
    private PlayerAirPlane _airPlane;
    private BeeBoss _beeBoss;

    private ObjectPool<AirPlaneBullet> enmeyBulletPool;
    private ObjectPool<AirPlaneBullet> beeBulletPool;
    
    private ObjectPool<EnemyAirPlane> enemyPool;
    private List<AirPlaneBullet> activeBullets;
    private List<EnemyAirPlane> activeEnemies;

    private List<RectTransform> enemySpawnPoint;

    private List<AirplaneDifficulty> _difficulty;

    private int _curDifficultyIndex;
    private float cur_create_enemy_interval = 0f;

    private bool isGameOver;
    private CompositeDisposable handler;
    private int score;

    private RectTransform bg1;
    private RectTransform bg2;

    private BeeGameState _state;
    public BeeGameState CurState
    {
        get => _state;
        set
        {
            if (_state != null)
            {
                _state.Exit();
            }
            _state = value;
            _state?.Enter();       
        }
    }

    public PlayerAirPlane BeeGirl => _airPlane;
    public BeeBoss Boss => _beeBoss;

    public Animation StartPageAnimation;

    public RectTransform BossRectTransform;
    public int Level
    {
        get;
        private set;
    }

    public int Kill_Enemy_Num
    {
        get;
        private set;
    }

    public AirplaneAppWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        WidgetType = AppType.AirPlane;
    }
    
    public override void OnCreate()
    {
        var go = uiTran.Find("Ins_BeeGirl");
        _airPlane = go.GetComponent<PlayerAirPlane>();
        _airPlane.Init();
        _airPlane.GameOver = GameOver;

        go = uiTran.Find("Ins_BeeGirlBoss");
        _beeBoss = go.GetComponent<BeeBoss>();
        

        bg1 = Tran_Bg1.GetComponent<RectTransform>();
        bg2 = Tran_Bg2.GetComponent<RectTransform>();
        
        activeBullets = new List<AirPlaneBullet>(30);
        activeEnemies = new(20);
        enemySpawnPoint = new List<RectTransform>(3);
        
        enmeyBulletPool = new ObjectPool<AirPlaneBullet>(onCreateBullect,
            onGetBullect,
            onReleaseBullect,
            onDestroyBullect,
            false,
            10,
            50);
        
        beeBulletPool = new ObjectPool<AirPlaneBullet>(onCreateBeeBullect,
            onGetBeeBullect,
            onReleaseBeeBullect,
            onDestroyBeeBullect,
            false,
            10,
            50);

        enemyPool = new ObjectPool<EnemyAirPlane>(onCreateEnemyBee,
            onGetEnemyBee,
            onReleaseEnemyBee,
            onDestroyEnemyBee,
            false,
            10,
            50);
        
        enemySpawnPoint.Add(Tran_A.GetComponent<RectTransform>());
        enemySpawnPoint.Add(Tran_B.GetComponent<RectTransform>());
        enemySpawnPoint.Add(Tran_C.GetComponent<RectTransform>());
        isGameOver = false;

        _curDifficultyIndex = 0;
        _difficulty = new List<AirplaneDifficulty>(3);
        var easy = YooAssets.LoadAssetSync<AirplaneDifficulty>("Assets/GameRes/SOConfigs/Airplane/easy.asset");
        _difficulty.Add(easy.AssetObject as AirplaneDifficulty);
        
        var medium = YooAssets.LoadAssetSync<AirplaneDifficulty>("Assets/GameRes/SOConfigs/Airplane/medium.asset");
        _difficulty.Add(medium.AssetObject as AirplaneDifficulty);
        
        var high = YooAssets.LoadAssetSync<AirplaneDifficulty>("Assets/GameRes/SOConfigs/Airplane/high.asset");
        _difficulty.Add(high.AssetObject as AirplaneDifficulty);

        StartPageAnimation = Tran_StartPage.GetComponent<Animation>();
        
        CurState = new WaitStartState(this);
        Level = 1;
        Kill_Enemy_Num = Level * 5;
    }

    public void NextLevel()
    {
        Level += 1;
        Kill_Enemy_Num = Level * 5;
    }

    public void ResetLevel()
    {
        Level = 0;
    }
    
    private AirPlaneBullet onCreateBullect()
    {
        var one = YooAssets.LoadAssetSync<AirPlaneBullet>("Assets/GameRes/Prefabs/BeeGame/EnemyBullet.prefab");
        var go = one.AssetObject as AirPlaneBullet;
        var bullectObj = Object.Instantiate(go,uiRectTran);
        return bullectObj.GetComponent<AirPlaneBullet>();
    }

    private void onGetBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(true);
        // activeBullects.Add(bullet);
    }

    private void onReleaseBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        // activeBullects.Remove(bullet);
    }

    private void onDestroyBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        Object.Destroy(bullet.gameObject);
    }
    
    
    private AirPlaneBullet onCreateBeeBullect()
    {
        var one = YooAssets.LoadAssetSync<AirPlaneBullet>("Assets/GameRes/Prefabs/BeeGame/BeeBullet.prefab");
        var go = one.AssetObject as AirPlaneBullet;
        var bullectObj = Object.Instantiate(go,uiRectTran);
        return bullectObj.GetComponent<AirPlaneBullet>();
    }

    private void onGetBeeBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(true);
        // activeBullects.Add(bullet);
    }

    private void onReleaseBeeBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        // activeBullects.Remove(bullet);
    }

    private void onDestroyBeeBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        Object.Destroy(bullet.gameObject);
    }
    
    
    private EnemyAirPlane onCreateEnemyBee()
    {
        var path = (int)(Time.deltaTime * 10000f)%2 == 0 ? "Assets/GameRes/Prefabs/BeeGame/BeeGirlEnemy002.prefab" : "Assets/GameRes/Prefabs/BeeGame/BeeGirlEnemy001.prefab";
        var one = YooAssets.LoadAssetSync<EnemyAirPlane>(path);
        var go = one.AssetObject as EnemyAirPlane;
        var enemyAirplaneObj = Object.Instantiate(go,uiRectTran);    
        return enemyAirplaneObj;
    }

    private void onGetEnemyBee(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(true);
        // activeEnemies.Add(airPlane);
    }

    private void onReleaseEnemyBee(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(false);
        // activeEnemies.Remove(airPlane);
    }

    private void onDestroyEnemyBee(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(false);
        Object.Destroy(airPlane.gameObject);
    }
    
    
    
    public override void OnDestroy()
    {
        enemySpawnPoint.Clear();
        
        foreach (var one in activeBullets)
        {
            if (one.CompareTag("Player"))
            {
                beeBulletPool.Release(one);   
            }
            else
            {
                enmeyBulletPool.Release(one);    
            }
        }
        activeBullets.Clear();
        foreach (var one in activeEnemies)
        {
            enemyPool.Release(one);
        }
        activeEnemies.Clear();
        
        _difficulty.Clear();
        _difficulty = null;
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        var btnRect = XBtn_touch.GetComponent<RectTransform>();
        // Debug.Log($"sizeDelta = {btnRect.rect}");
        // Debug.Log($"sizeDelta = {btnRect.rect.height}");
        maxY = btnRect.rect.height;
        maxX = btnRect.rect.width;
        
        handler = new CompositeDisposable(5);
        
        XBtn_Start.OnClick.Subscribe(onClickStart).AddTo(handler);
        
        XBtn_touch.OnClick.Subscribe(handletouch).AddTo(handler);
        // Btn_Restart.OnClick.Subscribe(RestartGame).AddTo(handler);

        // Sequence bg1move = DOTween.Sequence();
        // bg1move.Append(bg1.DOAnchorPosY(-1288.8f, 10f).SetEase(Ease.Linear));
        // bg1move.Append(bg1.DOAnchorPosY(1288.8f, 0.01f));
        // bg1move.Append(bg1.DOAnchorPosY(0f, 10f).SetEase(Ease.Linear));
        // bg1move.SetLoops(-1);
        //
        // Sequence bg2move = DOTween.Sequence();
        // bg2move.Append(bg2.DOAnchorPosY(0f, 10f).SetEase(Ease.Linear));
        // bg2move.Append(bg2.DOAnchorPosY(-1288.8f, 10f).SetEase(Ease.Linear));
        // bg2move.Append(bg2.DOAnchorPosY(1288.8f, 0.01f));
        // bg2move.SetLoops(-1);
        
        // XBtn_Start.OnClick.Subscribe(onClickStart).AddTo(handler);
        if (isGameOver)
        {
            ShowResult();
        }
        else
        {
            HideResult();
        }
    }

    public override void OnHide()
    {
        base.OnHide();
        handler.Dispose();
        handler.Clear();
    }

    private void onClickStart(PointerEventData param)
    {
        CurState = new StageState(this);
    }

    private bool isVisible(Vector2 pos)
    {
        return pos.y < maxY/1.5f && pos.y > -maxY/1.5f;
    }

    private readonly float bgMoveSpeed = 1228.8f/15f/60f;
    public void ScrollBG()
    {
        var p1 = bg1.anchoredPosition; 
        var tmp = p1.y - bgMoveSpeed;
        p1.y = tmp <= -1228.8f ? 1228.8f : tmp;
        
        bg1.anchoredPosition = p1;
        
        var p2 = bg2.anchoredPosition;
        tmp = p2.y - bgMoveSpeed;
        p2.y = tmp <= -1228.8f ? 1228.8f : tmp;
        
        bg2.anchoredPosition = p2;
    }
    
    public override void OnUpdate()
    {
        CurState.EveryFrame();
        
        // if (isGameOver) return;
        //
        // movebg();
        //
        // // updateBullect();
        // // updateAirPlane();
        // // updateEnemy();
        // //增加时间
        // Clocker.Instance.AddSecond(1);
        // if (score < 15)
        // {
        //     _curDifficultyIndex = 0;
        // }
        // else if(score < 10+2*10)
        // {
        //     _curDifficultyIndex = 1;
        // }
        // else
        // {
        //     _curDifficultyIndex = 2;
        // }
        // Debug.Log($"bullets.CountAll = {bulletPool.CountAll} bullets.CountActive = {bulletPool.CountActive} bullets.CountInactive = {bulletPool.CountInactive}");
        // Debug.Log($"enemyPool.CountAll = {enemyPool.CountAll} enemyPool.CountActive = {enemyPool.CountActive} enemyPool.CountInactive = {enemyPool.CountInactive}");
    }


    public void UpdateBoss()
    {
        if (!Boss.Move())
        {
            
        }

        if (Boss.Fire())
        {
            
        }
    }
    public void UpdateEnemy()
    {
        createEnemyBee();
        updateEnemyBee();
    }

    private void createEnemyBee()
    {
        if (activeEnemies.Count > 5) return;
        if (cur_create_enemy_interval <= 0)
        {
            var spawnPosition = new Vector2();
            spawnPosition.y = maxY;
            spawnPosition.x = Random.Range(maxX/2f,-maxX/2f) ;
       
            var enemyAirPlane = enemyPool.Get();
            activeEnemies.Add(enemyAirPlane);
            //目的地
            var dest = new Vector2();
            dest.y = -maxY*0.8f;
            dest.x = Random.Range(maxX/2f,-maxX/2f);

            var curDifficulty = _difficulty[_curDifficultyIndex];
            enemyAirPlane.ConfigProperty(recycleEnemyAirplane,spawnPosition,dest,
                curDifficulty.enemy_hp,
                curDifficulty.enemy_speed,
                curDifficulty.enemy_shot_interval);
            // Debug.Log($"createEnemyAirplane spawnPosition = {spawnPosition} dest = {dest}");
            cur_create_enemy_interval = curDifficulty.create_enemy_interval;
            
        }
        // int spawnIndex = Random.Range(0, enemySpawnPoint.Count);
        // var spawnPosition = enemySpawnPoint[spawnIndex].anchoredPosition;
        
        cur_create_enemy_interval -= Time.deltaTime;
    }

    private void updateEnemyBee()
    {
        for (int i = activeEnemies.Count - 1;i >= 0;i--)
        {
            var enemyAirPlane = activeEnemies[i];
            if (enemyAirPlane.Fire())
            {
                var one = enmeyBulletPool.Get();
                
                one.Init(enemyAirPlane.Controller.anchoredPosition + enemyAirPlane.Shot.anchoredPosition,
                    enemyAirPlane.tag,
                    enemyAirPlane.Direction,
                    enemyAirPlane.FlySpeed*1.5f);
                one.tag = enemyAirPlane.tag;
                activeBullets.Add(one);
            }
            
            if(enemyAirPlane.Move())continue;
            // Debug.Log($"updateEnemyAirplane回收敌机 i = {i} activeEnemies.Count = {activeEnemies.Count}");
            enemyPool.Release(enemyAirPlane);
            activeEnemies.RemoveAt(i);
        }
    }

    private void recycleEnemyAirplane(EnemyAirPlane airPlane)
    {
        Kill_Enemy_Num--;
        Kill_Enemy_Num = Math.Max(0, Kill_Enemy_Num);
        
        enemyPool.Release(airPlane);
        activeEnemies.Remove(airPlane);
        score += _difficulty[_curDifficultyIndex].enemy_score;
    }
    
    public void UpdateBee()
    {
        _airPlane.Move();
        if (_airPlane.Fire())
        {
            var one = beeBulletPool.Get();
            one.Init(_airPlane.AirPlaneRectTransform,
                4.0f);
            one.tag = _airPlane.tag;
            activeBullets.Add(one);
        }
    }

    public void UpdateFlyingBullet()
    {
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            var one = activeBullets[i];
            one.Move();
            if (isVisible(one.CurPos))
            {
                // Debug.Log($"one.CurPos = {one.CurPos}");
                continue;
            }
            // Debug.Log("updateBullect 回收");
            activeBullets.RemoveAt(i);
            if (one.CompareTag("Player"))
            {
                beeBulletPool.Release(one);   
            }
            else
            {
                enmeyBulletPool.Release(one);    
            }
        }
    }

    
    
    // private void movebg()
    // {
    //     var pos = bgA.anchoredPosition;
    //     pos.y += scrollSpeed;
    //     if (pos.y >= maxY)
    //     {
    //         pos.y = -maxY;
    //     }
    //     bgA.anchoredPosition = pos;
    //     
    //     pos = bgB.anchoredPosition;
    //     pos.y += scrollSpeed;
    //     if (pos.y >= maxY)
    //     {
    //         pos.y = -maxY;
    //     }
    //     bgB.anchoredPosition = pos;
    // }

    private void handletouch(PointerEventData param)
    {
        var wp = param.pointerPressRaycast.worldPosition;
        // Debug.Log($"handletouch world position {wp}");
        var lp = uiRectTran.InverseTransformPoint(wp);
        // Debug.Log($"handletouch local position {lp}");
        _airPlane.SetDestination(lp);

        // _airPlane.dist = lp;
    }

    private void GameOver()
    {
        isGameOver = true;
        
        _airPlane.Reset();
        foreach (var one in activeEnemies)
        {
            enemyPool.Release(one);
        }
        activeEnemies.Clear();

        foreach (var one in activeBullets)
        {
            if (one.CompareTag("Player"))
            {
                beeBulletPool.Release(one);
            }
            else
            {
                enmeyBulletPool.Release(one);    
            }
            
        }
        activeBullets.Clear();
        
        ShowResult();
    }

    private void RestartGame(PointerEventData param)
    {
        isGameOver = false;
        
        _curDifficultyIndex = 0;
        
        score = 0;

        HideResult();
    }

    private void ShowResult()
    {
        Tran_Result.gameObject.SetActive(true);
        Txt_Score.text = ZString.Format("分数:{0}", score);
    }

    private void HideResult()
    {
        Tran_Result.gameObject.SetActive(false);
    }

    public void ShowStartPage()
    {
        Tran_StartPage.gameObject.SetActive(true);
        StartPageAnimation.enabled = true;
        StartPageAnimation.Play();
    }

    public void HideStartPage()
    {
        Tran_StartPage.gameObject.SetActive(false);
        StartPageAnimation.enabled = false;
    }
}