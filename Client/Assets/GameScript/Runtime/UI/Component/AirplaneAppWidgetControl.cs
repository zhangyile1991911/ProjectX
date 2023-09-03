using System;
using System.Collections.Generic;
using Cysharp.Text;
using UniRx;
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
    // class AirPlaneConfig
    // {
    //     public float create_enemy_interval = 2.0f;
    //     public float enemy_speed = 0.7f;
    //     public float enemy_shot_interval = 2.5f;
    //     public int enemy_score = 1;
    //     public int enemy_hp = 1;
    // }
    
    private RectTransform bgA;
    private RectTransform bgB;
    private float maxY;
    private float maxX;
    private PlayerAirPlane _airPlane;

    private ObjectPool<AirPlaneBullet> bulletPool;
    private ObjectPool<EnemyAirPlane> enemyPool;
    private List<AirPlaneBullet> activeBullects;
    private List<EnemyAirPlane> activeEnemies;

    private List<RectTransform> enemySpawnPoint;

    private List<AirplaneDifficulty> _difficulty;

    private int _curDifficultyIndex;
    private float cur_create_enemy_interval = 5.0f;

    private bool isGameOver;
    private CompositeDisposable handler;
    private int score;
    
    public AirplaneAppWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        WidgetType = AppType.AirPlane;
    }
    
    public override void OnCreate()
    {
        bgA = Img_bg1.GetComponent<RectTransform>();
        bgB = Img_bg2.GetComponent<RectTransform>();
        
        Debug.Log($"bgA.anchorMax = {bgA.anchorMax}");
        Debug.Log($"bgB.anchorMax = {bgB.anchorMax}");

        var go = uiTran.Find("PlayerAirPlane");
        _airPlane = go.GetComponent<PlayerAirPlane>();
        _airPlane.GameOver = GameOver;
        
        activeBullects = new List<AirPlaneBullet>(20);
        activeEnemies = new(20);
        enemySpawnPoint = new List<RectTransform>(3);
        
        bulletPool = new ObjectPool<AirPlaneBullet>(onCreateBullect,
            onGetBullect,
            onReleaseBullect,
            onDestroyBullect,
            false,
            10,
            50);

        enemyPool = new ObjectPool<EnemyAirPlane>(onCreateEnemyAirPlane,
            onGetEnemyAirPlane,
            onReleaseEnemyAirPlane,
            onDestroyEnemyAirPlane,
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
    }

    private AirPlaneBullet onCreateBullect()
    {
        var one = YooAssets.LoadAssetSync<AirPlaneBullet>("Assets/GameRes/Prefabs/AirPlane/AirPlaneBullet.prefab");
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
    
    
    private EnemyAirPlane onCreateEnemyAirPlane()
    {
        var one = YooAssets.LoadAssetSync<EnemyAirPlane>("Assets/GameRes/Prefabs/AirPlane/EnemyAirPlane.prefab");
        var go = one.AssetObject as EnemyAirPlane;
        var enemyAirplaneObj = Object.Instantiate(go,uiRectTran);
        return enemyAirplaneObj;
    }

    private void onGetEnemyAirPlane(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(true);
        // activeEnemies.Add(airPlane);
    }

    private void onReleaseEnemyAirPlane(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(false);
        // activeEnemies.Remove(airPlane);
    }

    private void onDestroyEnemyAirPlane(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(false);
        Object.Destroy(airPlane.gameObject);
    }
    
    
    
    public override void OnDestroy()
    {
        enemySpawnPoint.Clear();
        
        foreach (var one in activeBullects)
        {
            bulletPool.Release(one);
        }
        activeBullects.Clear();
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
        var btnRect = Btn_touch.GetComponent<RectTransform>();
        // Debug.Log($"sizeDelta = {btnRect.rect}");
        // Debug.Log($"sizeDelta = {btnRect.rect.height}");
        maxY = btnRect.rect.height;
        maxX = btnRect.rect.width;
        
        handler = new CompositeDisposable(5);
        Btn_touch.OnClick.Subscribe(handletouch).AddTo(handler);
        Btn_Restart.OnClick.Subscribe(RestartGame).AddTo(handler);
        
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

    private float scrollSpeed = 0.2f;

    private bool isVisible(Vector2 pos)
    {
        return pos.y < maxY/1.5f && pos.y > -maxY/1.5f;
    }
    
    public override void OnUpdate()
    {
        if (isGameOver) return;
        movebg();
        updateBullect();
        updateAirPlane();
        updateEnemy();
        //增加时间
        Clocker.Instance.AddSecond(1);
        if (score < 15)
        {
            _curDifficultyIndex = 0;
        }
        else if(score < 10+2*10)
        {
            _curDifficultyIndex = 1;
        }
        else
        {
            _curDifficultyIndex = 2;
        }
        // Debug.Log($"bullets.CountAll = {bulletPool.CountAll} bullets.CountActive = {bulletPool.CountActive} bullets.CountInactive = {bulletPool.CountInactive}");
        // Debug.Log($"enemyPool.CountAll = {enemyPool.CountAll} enemyPool.CountActive = {enemyPool.CountActive} enemyPool.CountInactive = {enemyPool.CountInactive}");
    }

    private void updateEnemy()
    {
        createEnemyAirplane();
        updateEnemyAirplane();
    }

    private void createEnemyAirplane()
    {
        if (activeEnemies.Count > 2) return;
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

    private void updateEnemyAirplane()
    {
        for (int i = activeEnemies.Count - 1;i >= 0;i--)
        {
            var enemyAirPlane = activeEnemies[i];
            if (enemyAirPlane.Fire())
            {
                var one = bulletPool.Get();
                one.Init(enemyAirPlane.Controller,
                    enemyAirPlane.Direction,
                    enemyAirPlane.FlySpeed*1.5f);
                one.tag = enemyAirPlane.tag;
                activeBullects.Add(one);
            }
            
            if(enemyAirPlane.Move())continue;
            // Debug.Log($"updateEnemyAirplane回收敌机 i = {i} activeEnemies.Count = {activeEnemies.Count}");
            enemyPool.Release(enemyAirPlane);
            activeEnemies.RemoveAt(i);
        }
    }

    private void recycleEnemyAirplane(EnemyAirPlane airPlane)
    {
        enemyPool.Release(airPlane);
        activeEnemies.Remove(airPlane);
        score += _difficulty[_curDifficultyIndex].enemy_score;
    }
    
    private void updateAirPlane()
    {
        _airPlane.Move();
        if (_airPlane.Fire())
        {
            var one = bulletPool.Get();
            one.Init(_airPlane.AirPlaneRectTransform,
                4.0f);
            one.tag = _airPlane.tag;
            activeBullects.Add(one);
        }
    }

    private void updateBullect()
    {
        for (int i = activeBullects.Count - 1; i >= 0; i--)
        {
            var one = activeBullects[i];
            one.Move();
            if (isVisible(one.CurPos))
            {
                // Debug.Log($"one.CurPos = {one.CurPos}");
                continue;
            }
            // Debug.Log("updateBullect 回收");
            activeBullects.RemoveAt(i);
            bulletPool.Release(one);
        }
    }

    
    
    private void movebg()
    {
        var pos = bgA.anchoredPosition;
        pos.y += scrollSpeed;
        if (pos.y >= maxY)
        {
            pos.y = -maxY;
        }
        bgA.anchoredPosition = pos;
        
        pos = bgB.anchoredPosition;
        pos.y += scrollSpeed;
        if (pos.y >= maxY)
        {
            pos.y = -maxY;
        }
        bgB.anchoredPosition = pos;
    }

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

        foreach (var one in activeBullects)
        {
            bulletPool.Release(one);
        }
        activeBullects.Clear();
        
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
}