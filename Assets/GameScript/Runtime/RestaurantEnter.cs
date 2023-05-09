using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UniRx;
using UnityEngine;
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

    // private async void TimeGoesOn(DateTime dateTime)
    // { //时间流逝
    //     if (dateTime.Hour == 17)
    //     {
    //         if (dateTime.Minute >= 25 && dateTime.Minute < 26)
    //         {
    //             var DeliveryMan = await _characterMgr.CreateCharacter("DeliveryMan");
    //             DeliveryMan.SeatIndex = RandSeatIndex();
    //             var seatPoint = TakeSeatPoint(DeliveryMan.SeatIndex);
    //             _characters.Add(DeliveryMan);
    //             var enterBh = new CharacterEnterSceneBehaviour(RandSpawnPoint(),seatPoint);
    //             enterBh.Enter(DeliveryMan);    
    //         }
    //         
    //         if (dateTime.Minute >= 45 && dateTime.Minute < 46)
    //         {
    //             var DeliveryMan = await _characterMgr.CreateCharacter("FishMan");
    //             DeliveryMan.SeatIndex = RandSeatIndex();
    //             var seatPoint = TakeSeatPoint(DeliveryMan.SeatIndex);
    //             _characters.Add(DeliveryMan);
    //             var enterBh = new CharacterEnterSceneBehaviour(RandSpawnPoint(),seatPoint);
    //             enterBh.Enter(DeliveryMan);    
    //         }
    //     }
    // }

    // private void fiveSecondLoop()
    // {
    //     for (int i = 0; i < _characters.Count; i++)
    //     {
    //         var ch = _characters[i];
    //         var chatId = ch.HaveChatId();
    //         if (chatId > 0)
    //         {
    //             _restaurantWindow.GenerateChatBubble(chatId,ch,onClickBubble);
    //         }
    //     }
    // }

    // private void onClickBubble(ChatBubble bubble)
    // {
    //     //1 判断是否已经看过了
    //     //2 切换模式
    //     var cur = _stateMachine.CurrentNode as IRestaurantStateNode;
    //     cur?.OnClickBubble(bubble);
    // }
}
