using System;
using System.Collections.Generic;
using cfg.character;
using cfg.food;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using YooAsset;
using Random = UnityEngine.Random;

public class RestaurantEnter : MonoBehaviour
{
    public enum RestaurantCamera
    {
        RestaurantMain,
        Kitchen,
        Cook
    }
    public Transform standGroup;
    public Transform spawnGroup;
    public Transform orderBoardGroup;
    public Transform CookNode;
    public Transform Curtain;
    public Transform kitchenGroup;
    public Transform cookToolPos;
    public Transform Backet;
    
    public Transform PeopleGroup;
    public Transform[] PeopleSpawns;

    public Transform[] ShopKeeperStand;

    public Flavor[] Flavors;
    
    public CinemachineVirtualCamera RestaurantMainCamera;
    public CinemachineVirtualCamera KitchenCamera;
    public CinemachineVirtualCamera CookCamera;

    public Camera MainCamera;

    private CinemachineBrain _mainCameraBrain;
    // private CharacterMgr _characterMgr;
    // Start is called before the first frame update
    private List<Transform> _seatPoints;
    private List<Transform> _spawnPoints;
    private List<Transform> _orderBoardPoints;

    private List<int> _seatStatus;

    // public IEnumerable<RestaurantCharacter> CharacterEnumerable => _characters.Values;
    private Dictionary<int,RestaurantRoleBase> _waitingCharacters;

    // private RestaurantWindow _restaurantWindow;
    // private IDisposable _fiveSecondTimer;
    private StateMachine _stateMachine;

    private List<AssetOperationHandle> _cacheHandles;
    private Dictionary<cookTools, GameObject> _cookPlayDict;

    private ObjectPool<WalkingPeople> _peoplePool;
    private List<WalkingPeople> _activedPeople;
    
    private List<OutlineControl> _foodGameObjects;
    private List<Vector3> _foodPositions = new List<Vector3>()
    {
        new (-3,3,0),new (0,2.67f,0),new (3.27f,2.67f,0),
        new (-3.05f,-1.65f,0),new (0.17f,-1.42f,0),new (3.23f,-1.97f,0)
    };

    private int _foodPositionIndex = 0;
    void Start()
    {
        MainCamera = Camera.main;
        _mainCameraBrain = MainCamera.GetComponent<CinemachineBrain>();
        _mainCameraBrain.m_CameraCutEvent.AddListener((b) =>
        {
            Debug.Log($"RestaurantEnter IsLiveInBlend {b.IsLiveInBlend(KitchenCamera)} {b.IsLive(CookCamera)}");
        });
        _seatPoints = new List<Transform>(standGroup.childCount);
        // _emptyPoints = new List<int>(4);
        for (int i = 0; i < standGroup.childCount; i++)
        {
            _seatPoints.Add(standGroup.GetChild(i));
        }

        _orderBoardPoints = new List<Transform>(orderBoardGroup.childCount);
        for (int i = 0; i < orderBoardGroup.childCount; i++)
        {
            _orderBoardPoints.Add(orderBoardGroup.GetChild(i));
        }
        
        _spawnPoints = new List<Transform>(4);
        for (int i = 0; i < spawnGroup.childCount; i++)
        {
            _spawnPoints.Add(spawnGroup.GetChild(i));
        }
        
        _waitingCharacters = new Dictionary<int, RestaurantRoleBase>();

        _cookPlayDict = new Dictionary<cookTools, GameObject>();
        
        _cacheHandles = new List<AssetOperationHandle>(10);
        
        _foodGameObjects = new List<OutlineControl>(10);
        // _fiveSecondTimer = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(fiveSecondLoop).AddTo(this);

        _stateMachine = new StateMachine(this);
        _stateMachine.AddNode<CalenderStateNode>();
        _stateMachine.AddNode<WaitStateNode>();
        _stateMachine.AddNode<DialogueStateNode>();
        _stateMachine.AddNode<PrepareStateNode>();
        _stateMachine.AddNode<ProduceStateNode>();
        _stateMachine.AddNode<StatementStateNode>();
        
        //_stateMachine.Run<WaitStateNode>();
        _stateMachine.Run<CalenderStateNode>();

        EventModule.Instance.CharacterLeaveSub.Subscribe(CharacterLeave).AddTo(this);
        _seatStatus = new List<int>(_seatPoints.Count);
        for (int i = 0; i < _seatPoints.Count; i++)
        {
            _seatStatus.Add(0);
        }

        // loadShopkeeper();
        
        //加载当前已经在店里的客人
        // loadWaitingCharacter();
        GlobalFunctions.InitRecipe(UserInfoModule.Instance.OwnMenus);
        
        initPeople();

        // TestRecipeTree();
    }

    // private void TestRecipeTree()
    // {
    //     GlobalFunctions.InitRecipe(UserInfoModule.Instance.OwnMenus);
    //     foreach (var one in DataProviderModule.Instance.DataBase.TbMenuInfo.DataList)
    //     {
    //         GlobalFunctions.InsertRecipe(one.Id);
    //     }
    //     Debug.Log("food 1006 1013");
    //     var result = GlobalFunctions.SearchRecipe(new List<int>() {1013,1006});
    //     foreach (var recipeId in result)
    //     {
    //         Debug.Log(recipeId);
    //     }
    //     
    //     Debug.Log("food 1003");
    //     result = GlobalFunctions.SearchRecipe(new List<int>() {1003});
    //     foreach (var recipeId in result)
    //     {
    //         Debug.Log(recipeId);
    //     }
    //     
    //     Debug.Log("food 1002");
    //     result = GlobalFunctions.SearchRecipe(new List<int>() {1002});
    //     foreach (var recipeId in result)
    //     {
    //         Debug.Log(recipeId);
    //     }
    //     
    //     Debug.Log("food 1021");
    //     result = GlobalFunctions.SearchRecipe(new List<int>() {1021});
    //     foreach (var recipeId in result)
    //     {
    //         Debug.Log(recipeId);
    //     }
    //     
    //     Debug.Log("food 1022,1008,1026");
    //     result = GlobalFunctions.SearchRecipe(new List<int>() {1008,1022,1026});
    //     foreach (var recipeId in result)
    //     {
    //         Debug.Log(recipeId);
    //     }
    //     
    //     
    // }

    // private async void loadShopkeeper()
    // {
    //     //创建老板
    //     var characterObj = await CharacterMgr.Instance.CreateCharacter(10004);
    //     characterObj.gameObject.SetActive(false);
    // }
    
    // private async void loadWaitingCharacter()
    // {
    //     foreach (var cid in UserInfoModule.Instance.RestaurantWaitingCharacter)
    //     {
    //         var characterBase = await CharacterMgr.Instance.CreateCharacter(cid);
    //         var chara = characterBase as RestaurantCharacter;
    //         // var chara = handler as RestaurantRoleBase;
    //         var meal = UserInfoModule.Instance.GetCookedMealByCharacterId(chara.CharacterId);
    //         if (meal != null)
    //         {
    //             chara.ReceiveFood(meal,false);
    //         }
    //     
    //         var order = UserInfoModule.Instance.GetNPCOrder(cid);
    //         if (order != null)
    //         {
    //             chara.CurOrderInfo = order;
    //         }
    //         for (int i = 0; i < _seatPoints.Count; i++)
    //         {
    //                 var seatWorldPosition = CharacterTakeSeatPoint(chara.CharacterId, chara.SeatOccupy);
    //                 chara.transform.position = seatWorldPosition;
    //                 switch (chara.BehaviourID)
    //                 {
    //                     case behaviour.Leave:
    //                         chara.CurBehaviour = new CharacterLeave();
    //                         break;
    //                     case behaviour.Talk:
    //                         chara.CurBehaviour = new CharacterTalk();
    //                         break;
    //                     case behaviour.WaitReply:
    //                         chara.CurBehaviour = new CharacterTalk();
    //                         break;
    //                     case behaviour.WaitOrder:
    //                         chara.CurBehaviour = new CharacterWaitOrder();
    //                         break;
    //                     case behaviour.Eating:
    //                         chara.CurBehaviour = new CharacterEating();
    //                         break;
    //                     case behaviour.OrderDrink:
    //                         chara.CurBehaviour = new CharacterOrderDrink();
    //                         break;
    //                     case behaviour.OrderMeal:
    //                         chara.CurBehaviour = new CharacterOrderMeal();
    //                         break;
    //                     case behaviour.Thinking:
    //                         chara.CurBehaviour = new CharacterThinking();
    //                         break;
    //                     case behaviour.Comment:
    //                         chara.CurBehaviour = new CharacterComment();
    //                         break;
    //                     default:
    //                         Debug.Log($"waiting {chara.CharacterName} behaviour = {chara.BehaviourID} id = {(int)chara.BehaviourID}");
    //                         break;
    //                 }
    //                 break;
    //         }
    //     }
    // }
    
    private void Update()
    {
        _stateMachine?.Update();
    }

    public Vector3 CharacterTakeSeat(RestaurantRoleBase restaurantCharacter)
    {
        var seatIndex = FindEmptySeatIndex();
        restaurantCharacter.SeatOccupy = seatIndex;
        
        //todo 测试
        if (_waitingCharacters.ContainsKey(restaurantCharacter.CharacterId))
        {
            Debug.LogError($"duplication Id {restaurantCharacter.CharacterId}");
        }
        _waitingCharacters.Add(restaurantCharacter.CharacterId,restaurantCharacter);
        _seatStatus[seatIndex] = restaurantCharacter.CharacterId;
        return _seatPoints[seatIndex].position;
    }

    public bool ExistWaitingCharacter(int characterId)
    {
        return _waitingCharacters.ContainsKey(characterId);
    }
    
    public int FindEmptySeatIndex()
    {
        for (int i = 0; i < _seatStatus.Count; i++)
        {
            if (_seatStatus[i] == 0)
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsEmptySeat(int seatIndex)
    {
        if (seatIndex >= _seatStatus.Count)
            throw new Exception($"IsEmptySeat = {seatIndex}");
        return _seatStatus[seatIndex] == 0;
    }

    public Vector3 CharacterTakeSeatPoint(int characterId,int sindex)
    {
        if (IsEmptySeat(sindex))
        {
            _seatStatus[sindex] = characterId;
            return _seatPoints[sindex].position;    
        }
        Debug.LogError($"TakeSeatPoint error enter characterId = {characterId} sindex = {sindex} occupy character {_seatStatus[sindex]}");
        return Vector3.zero;
    }

    public void CharacterReturnSeat(int characterSeat)
    {
        // for (int i = 0; i < _seatStatus.Count; i++)
        // {
        //     int tmp = 1 << i;
        //     if ((characterSeat | tmp) == 1)
        //     {
        //         _seatStatus[i]= 0;        
        //     }
        // }
        _seatStatus[characterSeat] = 0;
        
    }

    public Vector3 SeatOrderBoardPositionInWorld(int seatIndex)
    {
        return _orderBoardPoints[seatIndex].position;
    }

    private void CharacterLeave(RestaurantRoleBase character)
    {
        CharacterReturnSeat(character.SeatOccupy);
        _waitingCharacters.Remove(character.CharacterId);
    }

    public bool HaveEmptySeat()
    {
        foreach (var one in _seatStatus)
        {
            if (one == 0) return true;
        }

        return false;
    }

    public int EmptySeatNum()
    {
        var counter = 0;
        foreach (var one in _seatStatus)
        {
            if(one == 0)counter++;
        }

        return counter;
    }

    public bool HaveDoubleSeat()
    {
        int counter = 0;
        foreach (var one in _seatStatus)
        {
            if (one == 0)
            {
                counter++;
            }
            else
            {
                counter = 0;
            }

            if (counter == 2)
                return true;
        }

        return false;
    }

    public Vector3 RandSpawnPoint()
    {
        int index = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[index].position;
    }

    public void FocusOnCharacter(RestaurantRoleBase c)
    {
        foreach (var one in _waitingCharacters.Values)
        {
            if (one == c)
            {
                one.ToLight();
            }
            else
            {
                one.ToDark();
            }
        }
    }
    
    public void NoFocusOnCharacter()
    {
        foreach (var one in _waitingCharacters.Values)
        {
            // one.CurBehaviour = new CharacterMute();
            one.ToLight();
        }
    }

    private async UniTask<GameObject> LoadCookGamePrefab(cookTools tools)
    {
        string prefabPath = DataProviderModule.Instance.GetCookPrefabInfo(tools).CookPrefab;
        if (_cookPlayDict.ContainsKey(tools))
        {
            return _cookPlayDict[tools];
        }
        //加载资源初始化
        var handle = YooAssets.LoadAssetAsync<GameObject>(prefabPath);
        _cacheHandles.Add(handle);
        await handle.ToUniTask();
        var prefab = handle.AssetObject as GameObject;
        var go = Instantiate(prefab, CookNode);
        go.transform.position = cookToolPos.position;
        
        _cookPlayDict.Add(tools,go);
        return go;
    }

    private void UnloadCookGamePrefab(cookTools tools)
    {
        if (!_cookPlayDict.ContainsKey(tools))
        {
            return;
        }
        //todo 卸载玩法场景
        _cookPlayDict.Remove(tools);
    }

    public void CloseRestaurant()
    {
        for (int i = 0;i < _seatPoints.Count;i++)
        {
            _seatStatus[i] = 0;
        }
        _waitingCharacters.Clear();
    }
    
    public async UniTask<GameObject> ShowCookGamePrefab(cookTools tools)
    {
        if (_cookPlayDict.TryGetValue(tools, out var prefab))
        {
            prefab.gameObject.SetActive(true);
            return prefab;
        }

        prefab = await LoadCookGamePrefab(tools);
        return prefab;
    }

    public void HideCookGamePrefab(cookTools tools)
    {
        if (_cookPlayDict.ContainsKey(tools))
        {
            var prefab = _cookPlayDict[tools];
            prefab.gameObject.SetActive(false);
        }
    }

    public void CutCamera(RestaurantCamera cameraType)
    {
        switch (cameraType)
        {
            case RestaurantCamera.Kitchen:
                RestaurantMainCamera.Priority = 0;
                CookCamera.Priority = 0;
                KitchenCamera.Priority = 999;
                UIManager.Instance.SyncUICameraPosition(KitchenCamera.transform.position,
                    true,
                    KitchenCamera.m_Lens.OrthographicSize,
                    KitchenCamera.m_Lens.NearClipPlane,
                    KitchenCamera.m_Lens.FarClipPlane);
                break;
            case RestaurantCamera.RestaurantMain:
                RestaurantMainCamera.Priority = 999;
                KitchenCamera.Priority = 0;
                CookCamera.Priority = 0;
                UIManager.Instance.SyncUICameraPosition(RestaurantMainCamera.transform.position,
                    false,
                    KitchenCamera.m_Lens.FieldOfView,
                    KitchenCamera.m_Lens.NearClipPlane,
                    KitchenCamera.m_Lens.FarClipPlane);
                break;
            case RestaurantCamera.Cook:
                RestaurantMainCamera.Priority = 0;
                KitchenCamera.Priority = 0;
                CookCamera.Priority = 999;
                UIManager.Instance.SyncUICameraPosition(KitchenCamera.transform.position,
                    true,
                    KitchenCamera.m_Lens.OrthographicSize,
                    KitchenCamera.m_Lens.NearClipPlane,
                    KitchenCamera.m_Lens.FarClipPlane);
                break;
        }

        // UIManager.Instance.UICamera.transform.position = RestaurantMainCamera.transform.position;
    }

    public void ShowCurtain()
    {
        Curtain.gameObject.SetActive(true);
    }

    public void HideCurtain()
    {
        Curtain.gameObject.SetActive(false);
    }

    public void GeneratePeople()
    {
        var one = _peoplePool.Get();
        int val = Random.Range(0, 10);
        var peopleId = Random.Range(1,9);
        if (val < 5)
        {
            one.Config(peopleId,PeopleSpawns[0].position,PeopleSpawns[1].position);
        }
        else
        {
            one.Config(peopleId,PeopleSpawns[1].position,PeopleSpawns[0].position);    
        }
        // Debug.Log($"_peoplePool.CountAll = {_peoplePool.CountAll} _peoplePool.CountActive = {_peoplePool.CountActive} _peoplePool.CountInactive = {_peoplePool.CountInactive} ");
    }

    public void FrozenAllPeople()
    {
        if (_activedPeople == null) return;
        foreach (var one in _activedPeople)
        {
            one.PauseWalk();
        }
    }

    public void ResumeAllPeople()
    {
        if (_activedPeople == null) return;
        foreach (var one in _activedPeople)
        {
            one.ResumeWalk();
        }
    }

    public void RecycleAllPeople()
    {
        if (_activedPeople == null) return;
        for( int i = _activedPeople.Count - 1;i >=0;i-- )
        {
            var one = _activedPeople[i];
            one.PauseWalk();
            _peoplePool.Release(one);
        }
        
    }
    
    private void initPeople()
    {
        _peoplePool =
            new ObjectPool<WalkingPeople>(onCreatePeople, 
                onGetPeople,
                onReleasePeople, 
                onDestroyPeople, 
                false,
                10, 
                30);
        _activedPeople = new List<WalkingPeople>(10);
    }

    private WalkingPeople onCreatePeople()
    {
        var loadHandler = YooAssets.LoadAssetSync<GameObject>(
            "Assets/GameRes/Prefabs/NPC/Crowd/WalkingPeople.prefab");
        var peopleObj = Instantiate(loadHandler.AssetObject,PeopleGroup) as GameObject;
        var result = peopleObj.GetComponent<WalkingPeople>();
        result.BelongPool = _peoplePool;
        return result;
    }
    
    private void onGetPeople(WalkingPeople people)
    {
        people.gameObject.SetActive(true);
        _activedPeople.Add(people);
    }

    private void onReleasePeople(WalkingPeople people)
    {
        people.gameObject.SetActive(false);
        _activedPeople.Remove(people);
    }

    private void onDestroyPeople(WalkingPeople people)
    {
        people.Release();
        Destroy(people.gameObject);          
    }
    
    
    public void ShowAndAdjustShopKeeperStand(int seatIndex)
    {
        var shopKeeper = CharacterMgr.Instance.GetCharacterById(10004);
        var skTrans = shopKeeper.transform;
        switch (seatIndex)
        {
            case 0:
            case 1:
                skTrans.position = ShopKeeperStand[0].position;
                skTrans.localScale = Vector3.one;
                break;
            case 2:
            case 3:
                skTrans.position = ShopKeeperStand[1].position;
                skTrans.localScale = new Vector3(-1,1,1);
                break;
        }
        shopKeeper.gameObject.SetActive(true);
    }

    public void HideShopKeeper()
    {
        var shopKeeper = CharacterMgr.Instance.GetCharacterById(10004);
        shopKeeper.gameObject.SetActive(false);
    }

    public void ShowKitchen()
    {
        kitchenGroup.gameObject.SetActive(true);
    }

    public void HideKitchen()
    {
        kitchenGroup.gameObject.SetActive(false);
    }

    public int CheckPickFood(Transform tran,out OutlineControl outlineControl)
    {
        outlineControl = null;
        for (int index = 0;index < _foodGameObjects.Count;index++)
        {
            var one = _foodGameObjects[index];
            if (one.transform == tran)
            {
                outlineControl = one;
                return index;
            }
        }

        return -1;
    }

    public async void AddFoodObject(int foodId)
    {
        if (_foodGameObjects.Count >= 10)
        {
            var data = new TipCommonData();
            data.tipstr = "超过上限";
            UIManager.Instance.CreateTip<TipCommon>(data).Forget();
            return;   
        }
        
        var itemTb = DataProviderModule.Instance.GetItemBaseInfo(foodId);
        if (itemTb == null)
        {
            Debug.Log($"foodId = {foodId} itemTb == null");
            return;
        }
        Debug.Log($"foodId = {foodId}");
        var res = YooAssets.LoadAssetAsync<GameObject>(itemTb.SceneResPath);
        await res.ToUniTask();
        var go = res.AssetObject as GameObject;
        var obj = GameObject.Instantiate(go,Backet);
        var oc = obj.GetComponent<OutlineControl>();
        oc.UserDataId = foodId;

        if (_foodPositionIndex >= _foodPositions.Count)
        {
            _foodPositionIndex = 0;
        }
        var pos = _foodPositions[_foodPositionIndex++];
        obj.transform.localPosition = pos;
        
        _foodGameObjects.Add(oc);
    }

    public void FoodDoDisappear(float duration)
    {
        for(int index = 0;index < _foodGameObjects.Count;index++)
        {
            _foodGameObjects[index].Disappear(duration);
        }
    }
    
    public void RemoveFoodObject(GameObject go)
    {
        for(int index = 0;index < _foodGameObjects.Count;index++)
        {
            if (go == _foodGameObjects[index].gameObject)
            {
                Destroy(go);
                _foodGameObjects.RemoveAt(index);
                break;
            }
        }
    }

    public void GetAllFoodObjectId(ref List<int> foodIds)
    {
        foodIds.Clear();
        foreach (var one in _foodGameObjects)
        {
            foodIds.Add(one.UserDataId);
        }
    }
    
    public void ClearAllFoodObject()
    {
        foreach (var one in _foodGameObjects)
        {
            Destroy(one);
        }
        _foodGameObjects.Clear();
    }
    
    public void ResetFoodOutline()
    {
        foreach (var one in _foodGameObjects)
        {
            one.HideOutline();
        }
    }
    
    
}
