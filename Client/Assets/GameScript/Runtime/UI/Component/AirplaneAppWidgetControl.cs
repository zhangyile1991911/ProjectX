using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using YooAsset;


/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class AirplaneAppWidget : BaseAppWidget
{
    private RectTransform bgA;
    private RectTransform bgB;
    private float maxY;
    private PlayerAirPlane _airPlane;

    private ObjectPool<AirPlaneBullet> bulletPool;
    private ObjectPool<EnemyAirPlane> enemyPool;
    private List<AirPlaneBullet> activeBullects;

    private List<EnemyAirPlane> activeEnemies;
    // private ObjectPool<> enemies;
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

        activeBullects = new List<AirPlaneBullet>(20);
        activeEnemies = new(20);
        
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
        activeBullects.Add(bullet);
    }

    private void onReleaseBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        activeBullects.Remove(bullet);
    }

    private void onDestroyBullect(AirPlaneBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        Object.Destroy(bullet.gameObject);
    }
    
    
    private EnemyAirPlane onCreateEnemyAirPlane()
    {
        var one = YooAssets.LoadAssetSync<EnemyAirPlane>("Assets/GameRes/Prefabs/AirPlane/EnemyAirPlane.prefab");
        var go = one.AssetObject as AirPlaneBullet;
        var enemyAirplaneObj = Object.Instantiate(go,uiRectTran);
        return enemyAirplaneObj.GetComponent<EnemyAirPlane>();
    }

    private void onGetEnemyAirPlane(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(true);
        activeEnemies.Add(airPlane);
    }

    private void onReleaseEnemyAirPlane(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(false);
        airPlane.Reset();
        activeEnemies.Remove(airPlane);
    }

    private void onDestroyEnemyAirPlane(EnemyAirPlane airPlane)
    {
        airPlane.gameObject.SetActive(false);
        Object.Destroy(airPlane.gameObject);
    }
    
    
    
    public override void OnDestroy()
    {
        
    }

    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        var btnRect = Btn_touch.GetComponent<RectTransform>();
        // Debug.Log($"sizeDelta = {btnRect.rect}");
        // Debug.Log($"sizeDelta = {btnRect.rect.height}");
        maxY = btnRect.rect.height;
        Btn_touch.OnClick.Subscribe(handletouch);
    }

    public override void OnHide()
    {
        base.OnHide();
        
    }

    private float scrollSpeed = 0.2f;
    public override void OnUpdate()
    {
        movebg();
        updateBullect();
        updateAirPlane();
        Debug.Log($"bullets.CountAll = {bulletPool.CountAll}\nbullets.CountActive = {bulletPool.CountActive}\nbullets.CountInactive = {bulletPool.CountInactive}\n");
    }

    private void updateEnemy()
    {
        createEnemy();
        for (int i = activeEnemies.Count - 1;i >= 0;i--)
        {
            var one = activeEnemies[i];
            
        }
    }

    private void createEnemy()
    {
        
    }
    
    private void updateAirPlane()
    {
        _airPlane.Move();
        if (_airPlane.Fire())
        {
            var one = bulletPool.Get();
            one.Init(_airPlane.AirPlaneRectTransform,
                bulletPool,
                new Vector2(maxY/2f,-maxY/2f),
                true);
            one.tag = _airPlane.tag;
        }
    }

    private void updateBullect()
    {
        for (int i = activeBullects.Count - 1; i >= 0; i--)
        {
            var one = activeBullects[i];
            if (one.OnUpdate())
            {
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
}