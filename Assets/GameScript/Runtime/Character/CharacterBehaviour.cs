using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;

public interface CharacterBehaviour
{
    public Character Char { get; }

    public void Enter(Character character);

    public void Update();

    public void Exit();
}


public class CharacterEnterScene : CharacterBehaviour
{
    public Character Char => _character;
    private Character _character;
    
    private Vector3 destPoint;
    private Vector3 spawnPoint;
    public CharacterEnterScene(Vector3 spawn,Vector3 dest)
    {
        destPoint = dest;
        spawnPoint = spawn;
    }
    
    public void Enter(Character character)
    {
        _character = character;
        _character.transform.position = spawnPoint;

        var dest1 = new Vector3(destPoint.x, 0, _character.transform.position.z);
        var dest2 = new Vector3(destPoint.x, 0, destPoint.z);
        var seq = DOTween.Sequence();
        seq.Append(_character.transform.DOMove(dest1, 7f));
        seq.Append(_character.transform.DOMove(dest2, 3f));
        seq.OnComplete(() =>
        {
            _character.CurBehaviour = new CharacterMakeBubble();
        });
        // seq.Complete();
        // _character.transform.DOMove(dest1, 10f);
        // _character.MoveTo(dest1,10);
    }

    public void Exit()
    {
        _character = null;
    }

    public void Update()
    {
        
    }
}

public class CharacterMakeBubble : CharacterBehaviour
{
    public Character Char => _character;
    private Character _character;

    private DateTime preDateTime;
    private IDisposable _disposable;
    public CharacterMakeBubble()
    {
        
    }
    
    public void Enter(Character character)
    {
        _character = character;
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
        if (minutes >= 5)
        {
            var eventModule = UniModule.GetModule<EventModule>();
            eventModule.CharBubbleTopic.OnNext(_character);
            preDateTime = dateTime;
        }
    }
}