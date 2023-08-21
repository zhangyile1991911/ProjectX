using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;

public interface CharacterBehaviour
{
    public RestaurantRoleBase Char { get; }

    public void Enter(RestaurantRoleBase restaurantCharacter);

    public void Update();

    public void Exit();
}


public class CharacterEnterScene : CharacterBehaviour
{
    public RestaurantRoleBase Char => _restaurantCharacter;
    private RestaurantRoleBase _restaurantCharacter;
    
    private Vector3 destPoint;
    // private Vector3 spawnPoint;
    public CharacterEnterScene(Vector3 dest)
    {
        destPoint = dest;
        // spawnPoint = spawn;
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        _restaurantCharacter.Sprite.color = Color.clear;
        _restaurantCharacter.transform.position = destPoint;

        _restaurantCharacter.Sprite.DOColor(Color.white, 3.5f).OnComplete(() =>
        {
            _restaurantCharacter.AddAppearCount();
            _restaurantCharacter.CurBehaviour = new CharacterMakeBubble();
        });
        // var dest1 = new Vector3(destPoint.x, 0, _restaurantCharacter.transform.position.z);
        // var dest2 = new Vector3(destPoint.x, 0, destPoint.z);
        // var seq = DOTween.Sequence();
        // seq.Append(_restaurantCharacter.transform.DOMove(dest1, 7f));
        // seq.Append(_restaurantCharacter.transform.DOMove(dest2, 3f));
        // seq.OnComplete(() =>
        // {
        //     _restaurantCharacter.AddAppearCount();
        //     _restaurantCharacter.CurBehaviour = new CharacterMakeBubble();
        // });

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
    public RestaurantRoleBase Char => _restaurantCharacter;
    private RestaurantCharacter _restaurantCharacter;

    private DateTime preDateTime;
    private IDisposable _disposable;
    public CharacterMakeBubble()
    {
        
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter as RestaurantCharacter;
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
        if (_restaurantCharacter.IsTimeToLeave())
        {
            _restaurantCharacter.CurBehaviour = new CharacterLeave();
        }
        else
        {
            var minutes = (dateTime - preDateTime).TotalMinutes;
            if (minutes < 3)
            {
                return;
                // var eventModule = UniModule.GetModule<EventModule>();
                // eventModule.CharBubbleTopic.OnNext(_restaurantCharacter);
            }
            preDateTime = dateTime;
            _restaurantCharacter.GenerateChatId();
        }
    }
    
}

public class CharacterOnFocus : CharacterBehaviour
{
    public RestaurantRoleBase Char => _restaurantCharacter;
    private RestaurantRoleBase _restaurantCharacter;

    public CharacterOnFocus()
    {
        
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
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
    public RestaurantRoleBase Char => _restaurantCharacter;
    private RestaurantRoleBase _restaurantCharacter;

    public CharacterMute()
    {
        
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
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

public class CharacterLeave : CharacterBehaviour
{
    public RestaurantRoleBase Char => _restaurantCharacter;
    private RestaurantRoleBase _restaurantCharacter;
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        _restaurantCharacter.Sprite
        .DOFade(0, 1.5f)
        .OnComplete(() =>
        {
            _restaurantCharacter.CurBehaviour = null;
            EventModule.Instance.CharacterLeaveTopic.OnNext(_restaurantCharacter);
            CharacterMgr.Instance.RemoveCharacter(_restaurantCharacter);
        });
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}

public class FollowCharacter : CharacterBehaviour
{
    public RestaurantRoleBase Char => _restaurantCharacter;
    private RestaurantRoleBase _restaurantCharacter;

    private RestaurantRoleBase _destCharacter;
    private Vector3 _seatPosition;
    private Tweener moveTweener;
    private Vector3 preDestionation;
    public FollowCharacter(RestaurantRoleBase dest,Vector3 seatPosition)
    {
        _destCharacter = dest;
        _seatPosition = seatPosition;
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        preDestionation = _restaurantCharacter.transform.position = _destCharacter.transform.position;
        
        moveTweener = _restaurantCharacter.transform.DOMove(_destCharacter.transform.position, 0.5f).SetAutoKill(false);
    }

    public void Update()
    {
        // if ((Type)_destCharacter.CurBehaviour != typeof(CharacterEnterScene))
        // {
        //     _restaurantCharacter.CurBehaviour = null;
        //     _restaurantCharacter.transform.position = _seatPosition;
        // }
        if (_destCharacter.CurBehaviour.GetType() == typeof(CharacterMakeBubble))
        {
            moveTweener.Kill();
            moveTweener = null;
            _restaurantCharacter.transform.position = _seatPosition;
            _restaurantCharacter.CurBehaviour = null;
        }
        else
        {
            moveTweener.ChangeEndValue(_destCharacter.transform.position,true).Restart();
            preDestionation = _destCharacter.transform.position;
        }
    }

    public void Exit()
    {
        
    }
}