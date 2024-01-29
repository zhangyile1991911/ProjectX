using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;
using YooAsset;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class AirplaneAppWidget : BaseAppWidget
{
    private float maxY;
    private float maxX;
    
    private PlayerBee _airPlane;
    private BeeBoss _beeBoss;

    private ObjectPool<BeeBullet> enemyBulletPool;
    private ObjectPool<BeeBullet> beeBulletPool;
    private ObjectPool<AnimatorAutoRecycle> explosionPool;
    
    private ObjectPool<EnemyBee> enemyPool;
    private Dictionary<string,BeeBullet> activeBullets;
    private List<EnemyBee> activeEnemies;

    private List<RectTransform> enemySpawnPoint;

    
    private AirplaneDifficulty _difficulty;
    
    private int _curDifficultyIndex;
    private float cur_create_enemy_interval = 0f;

    private bool isGameOver;
    private CompositeDisposable handler;
    

    public ReactiveProperty<int> ScorePub;
    private RectTransform bg1;
    private RectTransform bg2;

    private BeeGameState _state;

    private Animation _appStartAnim;
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

    public PlayerBee BeeGirl => _airPlane;
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


    public List<Image> HPImages;
    private Sprite hpfullSprite;
    private Sprite hpEmptySprite;
    
    public AirplaneAppWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        WidgetType = AppType.AirPlane;
    }
    
    public override void OnCreate()
    {
        var go = uiTran.Find("Ins_BeeGirl");
        _airPlane = go.GetComponent<PlayerBee>();
        _airPlane.Init();
 

        go = uiTran.Find("Ins_BeeGirlBoss");
        _beeBoss = go.GetComponent<BeeBoss>();
        

        bg1 = Tran_Bg1.GetComponent<RectTransform>();
        bg2 = Tran_Bg2.GetComponent<RectTransform>();
        
        activeBullets = new Dictionary<string, BeeBullet>();
        activeEnemies = new(20);
        enemySpawnPoint = new List<RectTransform>(3);

        _appStartAnim = uiGo.GetComponent<Animation>();
        
        enemyBulletPool = new ObjectPool<BeeBullet>(onCreateEnemyBullet,
            onGetBullet,
            onReleaseBullet,
            onDestroyBullet,
            false,
            10,
            50);
        
        beeBulletPool = new ObjectPool<BeeBullet>(onCreateBeeBullet,
            onGetBeeBullet,
            onReleaseBeeBullet,
            onDestroyBeeBullet,
            false,
            10,
            50);

        enemyPool = new ObjectPool<EnemyBee>(onCreateEnemyBee,
            onGetEnemyBee,
            onReleaseEnemyBee,
            onDestroyEnemyBee,
            false,
            10,
            50);

        explosionPool = new ObjectPool<AnimatorAutoRecycle>(
            onCreateExplosion,
            onGetExplosion,
            onReleaseExplosion,
            onDestroyExplosion,
            false,
            2,
            5);
        
        
        enemySpawnPoint.Add(Tran_A.GetComponent<RectTransform>());
        enemySpawnPoint.Add(Tran_B.GetComponent<RectTransform>());
        enemySpawnPoint.Add(Tran_C.GetComponent<RectTransform>());
        isGameOver = false;

        // _curDifficultyIndex = 0;
        // _difficulty = new List<AirplaneDifficulty>(3);
        var easy = YooAssets.LoadAssetSync<AirplaneDifficulty>("Assets/GameRes/SOConfigs/Airplane/easy.asset");
        _difficulty = easy.AssetObject as AirplaneDifficulty;
        //
        // var medium = YooAssets.LoadAssetSync<AirplaneDifficulty>("Assets/GameRes/SOConfigs/Airplane/medium.asset");
        // _difficulty.Add(medium.AssetObject as AirplaneDifficulty);
        //
        // var high = YooAssets.LoadAssetSync<AirplaneDifficulty>("Assets/GameRes/SOConfigs/Airplane/high.asset");
        // _difficulty.Add(high.AssetObject as AirplaneDifficulty);

        hpfullSprite = YooAssets.LoadAssetSync<Sprite>("Assets/GameRes/Picture/UI/Phone/SuperBeeGirl/heart.png").AssetObject as Sprite;
        hpEmptySprite = YooAssets.LoadAssetSync<Sprite>("Assets/GameRes/Picture/UI/Phone/SuperBeeGirl/blackheart.png").AssetObject as Sprite;

        StartPageAnimation = Tran_StartPage.GetComponent<Animation>();
        
        CurState = new WaitStartState(this);
        Level = 1;
        Kill_Enemy_Num = Level * 5;

        ScorePub = new ReactiveProperty<int>(0);

        HPImages = new List<Image>(3);
        for (int i = 0; i < Tran_hpgroup.childCount; i++)
        {
            HPImages.Add(Tran_hpgroup.GetChild(i).GetComponent<Image>());
        }
    }

    public void NextLevel()
    {
        Level += 1;
        Kill_Enemy_Num = Level * 5;
    }

    public void ResetLevel()
    {
        Level = 1;
        Kill_Enemy_Num = Level * 1;
    }

    private int bulletNum = 0;
    private BeeBullet onCreateEnemyBullet()
    {
        var one = YooAssets.LoadAssetSync<BeeBullet>("Assets/GameRes/Prefabs/BeeGame/EnemyBullet.prefab");
        var go = one.AssetObject as BeeBullet;
        var bulletObj = Object.Instantiate(go,uiRectTran);
        bulletObj.name = "enemyBullet"+bulletNum;
        bulletNum++;
        // bulletObj.testname = "enemybullet" + (++aaa);
        return bulletObj.GetComponent<BeeBullet>();
    }

    private void onGetBullet(BeeBullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void onReleaseBullet(BeeBullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void onDestroyBullet(BeeBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        Object.Destroy(bullet.gameObject);
    }

 
    private BeeBullet onCreateBeeBullet()
    {
        var one = YooAssets.LoadAssetSync<BeeBullet>("Assets/GameRes/Prefabs/BeeGame/BeeBullet.prefab");
        var go = one.AssetObject as BeeBullet;
        var bulletObj = Object.Instantiate(go,uiRectTran);
        bulletObj.name = "playerBullet"+bulletNum;
        bulletNum++;
        return bulletObj.GetComponent<BeeBullet>();
    }

    private void onGetBeeBullet(BeeBullet bullet)
    {
        bullet.gameObject.SetActive(true);
        // activeBullects.Add(bullet);
    }

    private void onReleaseBeeBullet(BeeBullet bullet)
    {
        Debug.Log($"playerBullet release {bullet.name}");
        GlobalFunctions.PrintLimitedStackTrace(2);
        bullet.gameObject.SetActive(false);
        // activeBullects.Remove(bullet);
    }
    
    
    private void onDestroyBeeBullet(BeeBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        Object.Destroy(bullet.gameObject);
    }
    
    
    private AnimatorAutoRecycle onCreateExplosion()
    {
        var one = YooAssets.LoadAssetSync<GameObject>("Assets/GameRes/Prefabs/BeeGame/Explosion.prefab");
        var go = one.AssetObject as GameObject;
        var ar = Object.Instantiate(go,uiRectTran);
        return ar.GetComponent<AnimatorAutoRecycle>();
    }

    private void onGetExplosion(AnimatorAutoRecycle go)
    {
        go.gameObject.SetActive(true);
        go.StartAnimation("explosion",explosionPool);
        Debug.Log($"onGetExplosion {go.GetInstanceID()}");
    }

    private void onReleaseExplosion(AnimatorAutoRecycle go)
    {
        go.gameObject.SetActive(false);
        Debug.Log($"onReleaseExplosion {go.GetInstanceID()}");
    }

    private void onDestroyExplosion(AnimatorAutoRecycle go)
    {
        go.gameObject.SetActive(false);
        Object.Destroy(go);
    }
    
    
    private EnemyBee onCreateEnemyBee()
    {
        var path = (int)(Time.deltaTime * 10000f)%2 == 0 ? "Assets/GameRes/Prefabs/BeeGame/BeeGirlEnemy002.prefab" : "Assets/GameRes/Prefabs/BeeGame/BeeGirlEnemy001.prefab";
        var one = YooAssets.LoadAssetSync<EnemyBee>(path);
        var go = one.AssetObject as EnemyBee;
        var enemyAirplaneObj = Object.Instantiate(go,uiRectTran);    
        return enemyAirplaneObj;
    }

    private void onGetEnemyBee(EnemyBee airPlane)
    {
        airPlane.gameObject.SetActive(true);
        // activeEnemies.Add(airPlane);
    }

    private void onReleaseEnemyBee(EnemyBee airPlane)
    {
        airPlane.gameObject.SetActive(false);
        // activeEnemies.Remove(airPlane);
    }

    private void onDestroyEnemyBee(EnemyBee airPlane)
    {
        airPlane.gameObject.SetActive(false);
        Object.Destroy(airPlane.gameObject);
    }
    
    
    
    public override void OnDestroy()
    {
        enemySpawnPoint.Clear();
        RecycleAllBullet();
        
        foreach (var one in activeEnemies)
        {
            enemyPool.Release(one);
        }
        activeEnemies.Clear();
        
        enemyPool.Clear();
        enemyPool = null;
        
        beeBulletPool.Clear();
        beeBulletPool = null;
        
        enemyBulletPool.Clear();
        enemyBulletPool = null;
        
        
        
        // _difficulty.Clear();
        // _difficulty = null;
        
        ScorePub = null;
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
        XBtn_Restart.OnClick.Subscribe(RestartGame).AddTo(handler);

        ScorePub.Subscribe((param) =>
        {
            Txt_curscore.text = param.ToString();
        }).AddTo(handler);
        
        _airPlane.HPTopic.Subscribe((param) =>
        {
            int index = 0;
            for (;index < param;index++)
            {
                HPImages[index].sprite = hpfullSprite;
            }

            for (; index < 3; index++)
            {
                HPImages[index].sprite = hpEmptySprite;
            }
            
        }).AddTo(handler);
        
        // Debug.Log($"GetClipCount = {_appStartAnim.GetClipCount()} GetClip = {_appStartAnim.GetClip("AppOnLoad")}");
        _appStartAnim.Play("AppOnLoad");
    }

    public override async void OnHide()
    {
        handler.Dispose();
        handler.Clear();
        _appStartAnim.Play("AppOnHide");
        await UniTask.Delay(TimeSpan.FromMilliseconds(500f));
        base.OnHide();
    }

    private void onClickStart(PointerEventData param)
    {
        CurState = new StageState(this);
    }

    private bool isVisible(Vector2 pos)
    {
        return pos.y < maxY/1.5f && pos.y > -maxY/1.5f;
    }

    private readonly float bgMoveSpeed = 1228.8f/35f/60f;
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
        //检查主角是否死亡
        if (!_airPlane.IsAlive)
        {
            CurState = new BeeGameOver(this);
        }
        CurState.EveryFrame();
        if (!GlobalFunctions.IsDebugMode)
        {
            //增加时间
            Clocker.Instance.AddSecond(1);    
        }
    }


    public void UpdateBoss()
    {
        Boss.Move();
        if (Boss.Fire())
        {
            // Debug.Log("BeeBoss::Fire()");
            var one = enemyBulletPool.Get();
                
            one.Init(Boss.Controller.anchoredPosition + Boss.Shot.anchoredPosition,
                Boss.tag,
                new Vector2(Boss.Controller.anchoredPosition.x,-1f),
                Boss.FlySpeed*1.5f,
                recycleBeeBullet);
            one.tag = Boss.tag;
            activeBullets.Add(one.name,one);
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

            // var curDifficulty = _difficulty[_curDifficultyIndex];
            enemyAirPlane.ConfigProperty(recycleEnemyBee,spawnPosition,dest,
                _difficulty.enemy_hp,
                _difficulty.enemy_speed,
                _difficulty.enemy_shot_interval);
            // Debug.Log($"createEnemyAirplane spawnPosition = {spawnPosition} dest = {dest}");
            cur_create_enemy_interval = _difficulty.create_enemy_interval;
            
        }
        // int spawnIndex = Random.Range(0, enemySpawnPoint.Count);
        // var spawnPosition = enemySpawnPoint[spawnIndex].anchoredPosition;
        
        cur_create_enemy_interval -= Time.deltaTime;
    }

    private void updateEnemyBee()
    {
        for (int i = activeEnemies.Count - 1;i >= 0;i--)
        {
            var enemyBee = activeEnemies[i];
            if (enemyBee.Fire())
            {
                var one = enemyBulletPool.Get();
                activeBullets.Add(one.name,one);
                one.Init(enemyBee.Controller.anchoredPosition + enemyBee.Shot.anchoredPosition,
                    enemyBee.tag,
                    enemyBee.Direction,
                    enemyBee.FlySpeed*1.5f,
                    recycleBeeBullet);
                one.tag = enemyBee.tag;
                
            }
            
            if(enemyBee.Move())continue;
            // Debug.Log($"updateEnemyAirplane回收敌机 i = {i} activeEnemies.Count = {activeEnemies.Count}");
            enemyPool.Release(enemyBee);
            activeEnemies.RemoveAt(i);
        }
    }

    private void recycleBeeBullet(BeeBullet bb)
    {
        activeBullets.Remove(bb.name);
        if (bb.CompareTag("PlayerBee"))
        {
            beeBulletPool.Release(bb);   
        }
        else
        {
            enemyBulletPool.Release(bb);    
        }
    }
    
    private void recycleEnemyBee(EnemyBee airPlane)
    {
        var exp = explosionPool.Get();
        exp.GetComponent<RectTransform>().anchoredPosition = airPlane.CurPos;
        
        Kill_Enemy_Num--;
        Kill_Enemy_Num = Math.Max(0, Kill_Enemy_Num);
        
        enemyPool.Release(airPlane);
        activeEnemies.Remove(airPlane);
        // score += _difficulty[_curDifficultyIndex].enemy_score;
        ScorePub.Value += 1;
    }

    public void RecycleAllEnemy()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var one = activeEnemies[i];
            enemyPool.Release(one);
            activeEnemies.Remove(one);
        }
    }

    public void RecycleAllExplosion()
    {
        
    }
    
    public void UpdateBee()
    {
        _airPlane.Move();
        if (_airPlane.Fire())
        {
            var one = beeBulletPool.Get();
            // one.Init(_airPlane.AirPlaneRectTransform,
            //     _airPlane.tag,
            //     2.0f,
            //     beeBulletPool);
            one.Init(_airPlane.AirPlaneRectTransform.anchoredPosition,
                _airPlane.tag,
                new Vector2(0, 1),
                3.0f,
                recycleBeeBullet);
            one.tag = _airPlane.tag;
            if (activeBullets.ContainsKey(one.name))
            {
                Debug.LogError($"activeBullets = {one.name}");
            }
            else
            {
                activeBullets.Add(one.name,one);    
            }
            
        }
    }

    private List<BeeBullet> deleteBullet = new List<BeeBullet>();
    public void UpdateFlyingBullet()
    {
        deleteBullet.Clear();
        foreach (var one in activeBullets.Values)
        {
            one.Move();
            if (isVisible(one.CurPos))
            {
                // Debug.Log($"one.CurPos = {one.CurPos}");
                continue;
            }
            // Debug.Log("updateBullect 回收");
            deleteBullet.Add(one);
        }

        
        foreach (var one in deleteBullet)
        {
            recycleBeeBullet(one);
        }
        deleteBullet.Clear();
    }

    
    public void RecycleAllBullet()
    {
        foreach (var one in activeBullets.Values)
        {
            if (one.CompareTag("PlayerBee"))
            {
                beeBulletPool.Release(one);   
            }
            else
            {
                enemyBulletPool.Release(one);    
            }
        }
        activeBullets.Clear();
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
        
    }

    private void RestartGame(PointerEventData param)
    {
        _curDifficultyIndex = 0;
        ScorePub.Value = 0;
        ResetLevel();
        CurState = new StageState(this);
    }

    public void ShowResult()
    {
        Tran_Result.gameObject.SetActive(true);
        //Txt_Score.text = ZString.Format("分数:{0}", score);
    }

    public void HideResult()
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

    public void AddDeferTask(int wait_second,Action<long> f)
    {
        // Observable.Timer(System.TimeSpan.FromMilliseconds(milliseconds))
        //     .Subscribe(f)
        //     .AddTo(handler);
        var per_second_frame = 1f / Time.deltaTime;
        Observable.TimerFrame(wait_second*(int)per_second_frame)
            .Subscribe(f)
            .AddTo(handler);
    }
}