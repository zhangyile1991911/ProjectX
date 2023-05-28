using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Yarn.Unity;
using Random = UnityEngine.Random;

public class RestaurantEnter : MonoBehaviour
{
    public Transform standGroup;
    public Transform spawnGroup;
    
    // private CharacterMgr _characterMgr;
    // Start is called before the first frame update
    private List<Transform> _seatPoints;
    private List<Transform> _spawnPoints;

    private List<int> _emptyPoints;

    public IEnumerable<Character> CharacterEnumerable => _characters.Values;
    private Dictionary<int,Character> _characters;

    // private RestaurantWindow _restaurantWindow;
    // private IDisposable _fiveSecondTimer;
    private StateMachine _stateMachine;
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
        
        // var _clocker = UniModule.GetModule<Clocker>();
        // _clocker.Topic.Subscribe(TimeGoesOn).AddTo(gameObject);

        _characters = new Dictionary<int, Character>();

        // _fiveSecondTimer = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(fiveSecondLoop).AddTo(this);

        _stateMachine = new StateMachine(this);
        _stateMachine.AddNode<WaitStateNode>();
        _stateMachine.AddNode<DialogueStateNode>();
        _stateMachine.AddNode<PrepareStateNode>();
        
        _stateMachine.Run<WaitStateNode>();
        
    }


    private void Update()
    {
        _stateMachine?.Update();
    }

    public void CharacterTakeRandomSeat(Character character)
    {
        character.SeatIndex = RandSeatIndex();
        _characters.Add(character.CharacterId,character);
        
        
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

    public bool HaveEmptySeat()
    {
        return _emptyPoints.Count > 0;
    }

    public Vector3 RandSpawnPoint()
    {
        int index = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[index].position;
    }

    public void FocusOnCharacter(Character c)
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
            one.CurBehaviour = new CharacterOnFocus();
        }
    }

    #region 自定义Command
    //加载Actor是异步的 可能图片还没加载完 执行了后面的命令
    //所以加一个计数器 等到人物都加载完后 在执行后面的演出命令
    //暂时还没想到更好的解决方式
    // private static uint LoadActorCounter = 0;
    // [YarnCommand("LoadActor")]
    // public static async void LoadActor(string nodeName,string characterName,bool show)
    // {
    //     var trans = Instance.findNode(nodeName);
    //     if (trans == null)
    //     {
    //         Debug.LogError($"找不到{nodeName}节点");
    //         return;
    //     }
    //
    //     LoadActorCounter++;
    //     var spName = string.Concat("Assets/Art/Character/Picture/", characterName,".png");
    //     var characterSp = await Addressables.LoadAssetAsync<Sprite>(spName).ToUniTask();
    //     LoadActorCounter--;
    //     
    //     var spriteRenderer = trans.GetComponent<SpriteRenderer>();
    //     spriteRenderer.sprite = characterSp;
    //     trans.gameObject.SetActive(show);
    //     // Debug.Log($"LoadActor {show}");
    // }

    // [YarnCommand("ActorFadeIn")]
    // public static async void ActorFadeIn(string actorName,float x,float y,float duration)
    // {
    //     await UniTask.WaitUntil(()=>LoadActorCounter == 0);
    //     var actor = Instance.findNode(actorName);
    //     actor.transform.position = new Vector3(x, y,0);
    //     var spriteRenderer = actor.GetComponent<SpriteRenderer>();
    //     spriteRenderer.color = new Color(1,1,1,0);
    //     spriteRenderer.DOFade(1, duration);
    //     actor.gameObject.SetActive(true);
    //     // Debug.Log($"ActorFadeIn");
    // }
    
    // [YarnCommand("CharacterFadeOut")]
    // public static void ActorFadeOut(string actorName,float x,float y,float duration)
    // {
    //     var actor = Instance.findNode(actorName);
    //     actor.transform.position = new Vector3(x, y,0);
    //     var spriteRenderer = actor.GetComponent<SpriteRenderer>();
    //     spriteRenderer.DOFade(0, duration);
    // }
    // [YarnCommand("OrderMeal")]
    // public void OrderMeal(string menuName)
    // {
    //     
    // }
    #endregion
    
}