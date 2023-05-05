using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class RestaurantEnter : MonoBehaviour
{
    public Transform standGroup;
    public Transform spawnGroup;
    
    private UIManager _uiManager;

    private CharacterMgr _characterMgr;
    // Start is called before the first frame update
    private List<Transform> _seatPoints;
    private List<Transform> _spawnPoints;

    private List<int> _emptyPoints;
    private List<Character> _characters;
    void Start()
    {
        _uiManager = UniModule.GetModule<UIManager>();
        _uiManager.OpenUI(UIEnum.RestaurantWindow, null, null);

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
        
        var _clocker = UniModule.GetModule<Clocker>();
        _clocker.Topic.Subscribe(TimeGoesOn).AddTo(gameObject);

        _characterMgr = UniModule.GetModule<CharacterMgr>();
        
        _characters = new List<Character>(4);
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

    public Vector3 RandSpawnPoint()
    {
        int index = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[index].position;
    }

    private async void TimeGoesOn(DateTime dateTime)
    { //时间流逝
        if (dateTime.Hour == 17)
        {
            if (dateTime.Minute >= 25 && dateTime.Minute < 26)
            {
                var DeliveryMan = await _characterMgr.CreateCharacter("DeliveryMan");
                DeliveryMan.SeatIndex = RandSeatIndex();
                var seatPoint = TakeSeatPoint(DeliveryMan.SeatIndex);
                _characters.Add(DeliveryMan);
                var enterBh = new CharacterEnterSceneBehaviour(RandSpawnPoint(),seatPoint);
                enterBh.Enter(DeliveryMan);    
            }
            
            if (dateTime.Minute >= 45 && dateTime.Minute < 46)
            {
                var DeliveryMan = await _characterMgr.CreateCharacter("FishMan");
                DeliveryMan.SeatIndex = RandSeatIndex();
                var seatPoint = TakeSeatPoint(DeliveryMan.SeatIndex);
                _characters.Add(DeliveryMan);
                var enterBh = new CharacterEnterSceneBehaviour(RandSpawnPoint(),seatPoint);
                enterBh.Enter(DeliveryMan);    
            }
        }
    }
}
