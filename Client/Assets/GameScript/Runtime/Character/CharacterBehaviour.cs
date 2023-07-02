using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;

public interface CharacterBehaviour
{
    public RestaurantCharacter Char { get; }

    public void Enter(RestaurantCharacter restaurantCharacter);

    public void Update();

    public void Exit();
}


public class CharacterEnterScene : CharacterBehaviour
{
    public RestaurantCharacter Char => _restaurantCharacter;
    private RestaurantCharacter _restaurantCharacter;
    
    private Vector3 destPoint;
    private Vector3 spawnPoint;
    public CharacterEnterScene(Vector3 spawn,Vector3 dest)
    {
        destPoint = dest;
        spawnPoint = spawn;
    }
    
    public void Enter(RestaurantCharacter restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        _restaurantCharacter.transform.position = spawnPoint;

        var dest1 = new Vector3(destPoint.x, 0, _restaurantCharacter.transform.position.z);
        var dest2 = new Vector3(destPoint.x, 0, destPoint.z);
        var seq = DOTween.Sequence();
        seq.Append(_restaurantCharacter.transform.DOMove(dest1, 7f));
        seq.Append(_restaurantCharacter.transform.DOMove(dest2, 3f));
        seq.OnComplete(() =>
        {
            _restaurantCharacter.CurBehaviour = new CharacterMakeBubble();
        });
        // seq.Complete();
        // _character.transform.DOMove(dest1, 10f);
        // _character.MoveTo(dest1,10);
    }

    public void Exit()
    {
        _restaurantCharacter = null;
    }

    public void Update()
    {
        
    }
}

public class CharacterMakeBubble : CharacterBehaviour
{
    public RestaurantCharacter Char => _restaurantCharacter;
    private RestaurantCharacter _restaurantCharacter;

    private DateTime preDateTime;
    private IDisposable _disposable;
    public CharacterMakeBubble()
    {
        
    }
    
    public void Enter(RestaurantCharacter restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        var clocker = UniModule.GetModule<Clocker>();
        _disposable = clocker.Topic.Subscribe(think);
        preDateTime = clocker.NowDateTime;
    }

    public void Exit()
    {
        _disposable?.Dispose();
    }

    public void Update()
    {
        
    }

    private void think(DateTime dateTime)
    {
        var minutes = (dateTime - preDateTime).TotalMinutes;
        if (minutes >= 3)
        {
            var eventModule = UniModule.GetModule<EventModule>();
            eventModule.CharBubbleTopic.OnNext(_restaurantCharacter);
            preDateTime = dateTime;
        }
    }
}

public class CharacterOnFocus : CharacterBehaviour
{
    public RestaurantCharacter Char => _restaurantCharacter;
    private RestaurantCharacter _restaurantCharacter;

    public CharacterOnFocus()
    {
        
    }
    
    public void Enter(RestaurantCharacter restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        _restaurantCharacter.ToLight();
    }

    public void Exit()
    {
        _restaurantCharacter = null;
    }

    public void Update()
    {
        
    }
}
public class CharacterMute : CharacterBehaviour
{
    public RestaurantCharacter Char => _restaurantCharacter;
    private RestaurantCharacter _restaurantCharacter;

    public CharacterMute()
    {
        
    }
    
    public void Enter(RestaurantCharacter restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        _restaurantCharacter.ToDark();
    }

    public void Exit()
    {
        _restaurantCharacter.ToLight();
        _restaurantCharacter = null;
    }

    public void Update()
    {
        
    }
}