using System;
using System.Collections;
using System.Collections.Generic;
using cfg.food;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Yarn.Unity;
using YooAsset;
using Random = UnityEngine.Random;

public class RestaurantEnter : MonoBehaviour
{
    public enum RestaurantCamera
    {
        RestaurantMain,
        Kitchen
    }
    public Transform standGroup;
    public Transform spawnGroup;
    public Transform CookNode;

    public CinemachineVirtualCamera RestaurantMainCamera;
    public CinemachineVirtualCamera KitchenCamera;
    
    // private CharacterMgr _characterMgr;
    // Start is called before the first frame update
    private List<Transform> _seatPoints;
    private List<Transform> _spawnPoints;

    private List<int> _emptyPoints;

    // public IEnumerable<RestaurantCharacter> CharacterEnumerable => _characters.Values;
    private Dictionary<int,RestaurantCharacter> _characters;

    // private RestaurantWindow _restaurantWindow;
    // private IDisposable _fiveSecondTimer;
    private StateMachine _stateMachine;

    private List<AssetOperationHandle> _cacheHandles;
    private Dictionary<cookTools, GameObject> _cookPlayDict;
    void Start()
    {
        _seatPoints = new List<Transform>(4);
        _emptyPoints = new List<int>(4);
        for (int i = 0; i < standGroup.childCount; i++)
        {
            _seatPoints.Add(standGroup.GetChild(i));
            _emptyPoints.Add(i);
        }
        
        _spawnPoints = new List<Transform>(4);
        for (int i = 0; i < spawnGroup.childCount; i++)
        {
            _spawnPoints.Add(spawnGroup.GetChild(i));
        }
        
        _characters = new Dictionary<int, RestaurantCharacter>();

        _cookPlayDict = new Dictionary<cookTools, GameObject>();
        
        _cacheHandles = new List<AssetOperationHandle>(10);
        // _fiveSecondTimer = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(fiveSecondLoop).AddTo(this);

        _stateMachine = new StateMachine(this);
        _stateMachine.AddNode<WaitStateNode>();
        _stateMachine.AddNode<DialogueStateNode>();
        _stateMachine.AddNode<PrepareStateNode>();
        _stateMachine.AddNode<ProduceStateNode>();
        _stateMachine.AddNode<StatementStateNode>();
        
        _stateMachine.Run<WaitStateNode>();

        EventModule.Instance.CharacterLeaveSub.Subscribe(CharacterLeave).AddTo(this);
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    public void CharacterTakeRandomSeat(RestaurantCharacter restaurantCharacter)
    {
        restaurantCharacter.SeatIndex = RandSeatIndex();
        _characters.Add(restaurantCharacter.CharacterId,restaurantCharacter);
        
        
    }

    public bool ExistCharacter(int characterId)
    {
        return _characters.ContainsKey(characterId);
    }
    
    public int RandSeatIndex()
    {
        int index = Random.Range(0, _emptyPoints.Count);
        return index;
    }

    public Vector3 TakeSeatPoint(int sindex)
    {
        int tmp = _emptyPoints[sindex];
        var v = _seatPoints[tmp];
        _emptyPoints.RemoveAt(sindex);
        return v.position;
    }

    public void ReturnSeat(int index)
    {
        _emptyPoints.Add(index);
    }

    public void CharacterLeave(RestaurantCharacter character)
    {
        ReturnSeat(character.SeatIndex);
        _characters.Remove(character.CharacterId);
    }

    public bool HaveEmptySeat()
    {
        return _emptyPoints.Count > 0;
    }

    public Vector3 RandSpawnPoint()
    {
        int index = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[index].position;
    }

    public void FocusOnCharacter(RestaurantCharacter c)
    {
        foreach (var one in _characters.Values)
        {
            if (one == c)
            {
                one.CurBehaviour = new CharacterOnFocus();
            }
            else
            {
                one.CurBehaviour = new CharacterMute();    
            }
        }
    }

    public void NoFocusOnCharacter()
    {
        foreach (var one in _characters.Values)
        {
            one.CurBehaviour = new CharacterMakeBubble();
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
        _emptyPoints.Clear();
        for (int i = 0;i < _seatPoints.Count;i++)
        {
            _emptyPoints.Add(i);
        }
        _characters.Clear();
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
                KitchenCamera.Priority = 999;
                break;
            case RestaurantCamera.RestaurantMain:
                RestaurantMainCamera.Priority = 999;
                KitchenCamera.Priority = 0;
                break;
        }

        UIManager.Instance.UICamera.transform.position = RestaurantMainCamera.transform.position;
    }

}
