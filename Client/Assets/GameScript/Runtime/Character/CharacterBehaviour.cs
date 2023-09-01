using System;
using System.Collections;
using System.Collections.Generic;
using cfg.character;
using cfg.common;
using UnityEngine;
using DG.Tweening;
using UniRx;
using Random = UnityEngine.Random;

public interface CharacterBehaviour
{
    // public RestaurantRoleBase Char { get; }
    public behaviour BehaviourID
    {
        get;
    }

    public void Enter(RestaurantRoleBase restaurantCharacter);

    public void Update();

    public void Exit();
}




// public class CharacterMakeBubble : CharacterBehaviour
// {
//     public RestaurantRoleBase Char => _restaurantCharacter;
//     private RestaurantCharacter _restaurantCharacter;
//
//     private GameDateTime preDateTime;
//     private IDisposable _disposable;
//     public CharacterMakeBubble()
//     {
//         
//     }
//     
//     public void Enter(RestaurantRoleBase restaurantCharacter)
//     {
//         _restaurantCharacter = restaurantCharacter as RestaurantCharacter;
//         var clocker = UniModule.GetModule<Clocker>();
//         _disposable = clocker.Topic.Subscribe(think);
//         preDateTime = clocker.NowDateTime;
//     }
//
//     public void Exit()
//     {
//         _disposable?.Dispose();
//     }
//
//     public void Update()
//     {
//         
//     }
//
//     private void think(GameDateTime dateTime)
//     {
//         if (_restaurantCharacter.IsTimeToLeave())
//         {
//             _restaurantCharacter.CurBehaviour = new CharacterLeave();
//         }
//         else
//         {
//             var minutes = (dateTime - preDateTime).TotalMinutes;
//             if (minutes < 3)
//             {
//                 return;
//                 // var eventModule = UniModule.GetModule<EventModule>();
//                 // eventModule.CharBubbleTopic.OnNext(_restaurantCharacter);
//             }
//             preDateTime = dateTime;
//             _restaurantCharacter.GenerateChatId();
//         }
//     }
//     
// }

// public class CharacterOnFocus : CharacterBehaviour
// {
//     public RestaurantRoleBase Char => _restaurantCharacter;
//     private RestaurantRoleBase _restaurantCharacter;
//
//     public CharacterOnFocus()
//     {
//         
//     }
//     
//     public void Enter(RestaurantRoleBase restaurantCharacter)
//     {
//         _restaurantCharacter = restaurantCharacter;
//         _restaurantCharacter.ToLight();
//     }
//
//     public void Exit()
//     {
//         _restaurantCharacter = null;
//     }
//
//     public void Update()
//     {
//         
//     }
// }
// public class CharacterMute : CharacterBehaviour
// {
//     public RestaurantRoleBase Char => _restaurantCharacter;
//     private RestaurantRoleBase _restaurantCharacter;
//
//     public CharacterMute()
//     {
//         
//     }
//     
//     public void Enter(RestaurantRoleBase restaurantCharacter)
//     {
//         _restaurantCharacter = restaurantCharacter;
//         _restaurantCharacter.ToDark();
//     }
//
//     public void Exit()
//     {
//         _restaurantCharacter.ToLight();
//         _restaurantCharacter = null;
//     }
//
//     public void Update()
//     {
//         
//     }
// }

// public class FollowCharacter : CharacterBehaviour
// {
//     public RestaurantRoleBase Char => _restaurantCharacter;
//     private RestaurantRoleBase _restaurantCharacter;
//
//     private RestaurantRoleBase _destCharacter;
//     private Vector3 _seatPosition;
//     private Tweener moveTweener;
//     private Vector3 preDestionation;
//     public FollowCharacter(RestaurantRoleBase dest,Vector3 seatPosition)
//     {
//         _destCharacter = dest;
//         _seatPosition = seatPosition;
//     }
//     
//     public void Enter(RestaurantRoleBase restaurantCharacter)
//     {
//         _restaurantCharacter = restaurantCharacter;
//         preDestionation = _restaurantCharacter.transform.position = _destCharacter.transform.position;
//         
//         moveTweener = _restaurantCharacter.transform.DOMove(_destCharacter.transform.position, 0.5f).SetAutoKill(false);
//     }
//
//     public void Update()
//     {
//         // if ((Type)_destCharacter.CurBehaviour != typeof(CharacterEnterScene))
//         // {
//         //     _restaurantCharacter.CurBehaviour = null;
//         //     _restaurantCharacter.transform.position = _seatPosition;
//         // }
//         if (_destCharacter.CurBehaviour.GetType() == typeof(CharacterMakeBubble))
//         {
//             moveTweener.Kill();
//             moveTweener = null;
//             _restaurantCharacter.transform.position = _seatPosition;
//             _restaurantCharacter.CurBehaviour = null;
//         }
//         else
//         {
//             moveTweener.ChangeEndValue(_destCharacter.transform.position,true).Restart();
//             preDestionation = _destCharacter.transform.position;
//         }
//     }
//
//     public void Exit()
//     {
//         
//     }
// }



public class CharacterEnterScene : CharacterBehaviour
{
    // public RestaurantRoleBase Char => _restaurantCharacter;
    public behaviour BehaviourID => behaviour.Enter;
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
        Debug.Log($"-----{restaurantCharacter.CharacterName} 进入饭店状态-----");
        _restaurantCharacter = restaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        _restaurantCharacter.Sprite.color = Color.clear;
        _restaurantCharacter.transform.position = destPoint;
        
        _restaurantCharacter.Sprite.DOColor(Color.white, 3.5f).OnComplete(() =>
        {
            _restaurantCharacter.AddAppearCount();
            _restaurantCharacter.CurBehaviour = new CharacterTalk();
        });
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出饭店状态-----");
        _restaurantCharacter = null;
    }

    public void Update()
    {
        
    }
}


public class CharacterTalk : CharacterBehaviour
{
    // public RestaurantRoleBase Char => _restaurantCharacter;
    public behaviour BehaviourID => behaviour.Talk;
    private RestaurantCharacter _restaurantCharacter;

    private long _preDateTime;
    public CharacterTalk()
    {
        
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter as RestaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        _preDateTime = Clocker.Instance.NowDateTime.Timestamp;
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入讲话状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出讲话状态-----");
    }

    public void Update()
    {
        var minutes = (Clocker.Instance.NowDateTime.Timestamp - _preDateTime)/60L;
        if (minutes < 2)
        {
            return;
        }
        _preDateTime = Clocker.Instance.NowDateTime.Timestamp;
        var success = _restaurantCharacter.GenerateMain();
        if (!success) success = _restaurantCharacter.GenerateTalkId();
        if (success) _restaurantCharacter.CurBehaviour = new CharacterWaitReply();
    }
}

public class CharacterWaitReply : CharacterBehaviour
{
    // public RestaurantRoleBase Char => _restaurantCharacter;
    public behaviour BehaviourID => behaviour.WaitReply;
    private RestaurantCharacter _restaurantCharacter;
    private long timestamp;
    private int attenuation;
    public CharacterWaitReply()
    {
        
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter as RestaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        timestamp = Clocker.Instance.NowDateTime.Timestamp;
        _restaurantCharacter.ResetPatient();
        attenuation = DataProviderModule.Instance.AttenuatePatientValue();
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入等待回复状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出等待回复状态-----");
    }

    public void Update()
    {
        var nowts = Clocker.Instance.NowDateTime.Timestamp;
        var diffMinute = (nowts - timestamp) / 60L;
        if (diffMinute > 0)
        {
            _restaurantCharacter.AttenuatePatient(attenuation*(int)diffMinute);
            timestamp = nowts;
        }
        if (_restaurantCharacter.PatientValue <= 0)
        {//如果耐心值耗完,就离开
            Debug.Log("客人等待回复 耐心耗尽");
            _restaurantCharacter.CurBehaviour = new CharacterLeave();
        }
    }
}

public class CharacterWaitOrder : CharacterBehaviour
{
    // public RestaurantRoleBase Char => _restaurantCharacter;
    public behaviour BehaviourID => behaviour.WaitOrder;
    private RestaurantCharacter _restaurantCharacter;
    private long timestamp;
    private int attenuation;
    public CharacterWaitOrder()
    {
        
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter as RestaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        timestamp = Clocker.Instance.NowDateTime.Timestamp;
        _restaurantCharacter.ResetPatient();
        attenuation = DataProviderModule.Instance.AttenuatePatientValue();
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入等待上菜状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出等待上菜状态-----");
    }

    public void Update()
    {
        var nowts = Clocker.Instance.NowDateTime.Timestamp;
        var diffMinute = (nowts - timestamp) / 60L;
        if (diffMinute > 0)
        {
            _restaurantCharacter.AttenuatePatient(attenuation*(int)diffMinute);
            timestamp = nowts;
        }
        //todo 在等待过程中可以产生一些吐槽
        if (_restaurantCharacter.PatientValue <= 0)
        {//如果耐心值耗完,就离开
            Debug.Log($"_restaurantCharacter {_restaurantCharacter.CharacterName} 耐心耗尽退出等待");
            _restaurantCharacter.CurBehaviour = new CharacterLeave();
        }
    }
}

public enum CharacterOrderType
{
    Meal,Drink,All
}

public class CharacterOrder : CharacterBehaviour
{
    public behaviour BehaviourID => behaviour.Order;
    public CharacterOrder()
    {
        
    }

    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }
}


public class CharacterOrderMeal : CharacterBehaviour
{
    // public RestaurantRoleBase Char { get; }
    public behaviour BehaviourID => behaviour.OrderMeal;
    private long _preDateTime;
    private RestaurantCharacter _restaurantCharacter;
    // private CharacterOrderType _orderType;
    public CharacterOrderMeal()
    {
        // _orderType = orderType;
    }

    public void Enter(RestaurantRoleBase restaurantRoleBase)
    {
        _restaurantCharacter = restaurantRoleBase as RestaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        _preDateTime = Clocker.Instance.NowDateTime.Timestamp;
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入下单料理状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出下单料理状态-----");
    }

    public void Update()
    {
        think();
    }
    private void think()
    {
        if (!_restaurantCharacter.CanOrder())
        {
            Debug.Log("点单超过上限");
            _restaurantCharacter.CurBehaviour = new CharacterLeave();
            return;
        }
        var minutes = (Clocker.Instance.NowDateTime.Timestamp - _preDateTime)/60L;
        if (minutes < 2)
        {
            return;
        }
        _preDateTime = Clocker.Instance.NowDateTime.Timestamp;
        _restaurantCharacter.GenerateOrder();
    }
}

public class CharacterOrderDrink : CharacterBehaviour
{
    // public RestaurantRoleBase Char { get; }
    public behaviour BehaviourID => behaviour.OrderDrink;
    private long _preDateTime;
    private RestaurantCharacter _restaurantCharacter;
    // private CharacterOrderType _orderType;
    public CharacterOrderDrink()
    {
        // _orderType = orderType;
    }

    public void Enter(RestaurantRoleBase restaurantRoleBase)
    {
        _restaurantCharacter = restaurantRoleBase as RestaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        _preDateTime = Clocker.Instance.NowDateTime.Timestamp;
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入下单饮料状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出下单饮料状态-----");
    }

    public void Update()
    {
        think();
    }
    private void think()
    {
        if (!_restaurantCharacter.CanOrder())
        {
            Debug.Log("点单超过上限");
            _restaurantCharacter.CurBehaviour = new CharacterLeave();
            return;
        }
        var minutes = (Clocker.Instance.NowDateTime.Timestamp - _preDateTime)/60L;
        if (minutes < 2)
        {
            return;
        }
        _preDateTime = Clocker.Instance.NowDateTime.Timestamp;
        _restaurantCharacter.GenerateOrder();
    }
}



public class CharacterEating : CharacterBehaviour
{
    // public RestaurantRoleBase Char { get; }
    public behaviour BehaviourID => behaviour.Eating;
    private RestaurantCharacter _restaurantCharacter;
    private long start_eating_timestamp;
    private long start_chow_timestamp;
    private IDisposable eatingTimer;
    public CharacterEating()
    {
        
    }

    public void Enter(RestaurantRoleBase restaurantRoleBase)
    {
        _restaurantCharacter = restaurantRoleBase as RestaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        start_chow_timestamp = start_eating_timestamp = Clocker.Instance.NowDateTime.Timestamp;
        eatingTimer = Clocker.Instance.Topic.Subscribe(Eating);
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入吃饭状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出吃饭状态-----");
        eatingTimer.Dispose();
        eatingTimer = null;
    }

    public void Update()
    {
        
    }

    private void Eating(GameDateTime nowts)
    {
        var eating_duration = (nowts.Timestamp - start_eating_timestamp) / 60L;
        if (eating_duration > 10)
        {
            _restaurantCharacter.CurBehaviour = new CharacterComment();
        }
        eating_duration = (nowts.Timestamp - start_chow_timestamp) / 60L;
        if (eating_duration % 2 == 0)
        {
            _restaurantCharacter.PlayAnimation(BehaviourID);
            start_chow_timestamp = nowts.Timestamp;
        }
    }
}


public class CharacterComment : CharacterBehaviour
{
    public behaviour BehaviourID => behaviour.Comment;
    private RestaurantCharacter _restaurantCharacter;
    public CharacterComment()
    {
    }

    public void Enter(RestaurantRoleBase restaurantRoleBase)
    {
        _restaurantCharacter = restaurantRoleBase as RestaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        var info = new CharacterSaidInfo
        {
            CharacterId = _restaurantCharacter.CharacterId,
        };
        switch (_restaurantCharacter.CurOrderInfo.OrderType)
        {
            case bubbleType.Omakase:
                info.ChatId = _restaurantCharacter.OMAKASEComment.Id;
                break;
            case bubbleType.HybridOrder:
                info.ChatId = _restaurantCharacter.HybridOrderComment.Id;
                break;
            case bubbleType.SpecifiedOrder:
                info.ChatId = _restaurantCharacter.SpecifiedOrderComment.Id;
                break;
        }
        EventModule.Instance.CharBubbleTopic.OnNext(info);
        Debug.Log($"-----{_restaurantCharacter.CharacterName} {info.ChatId} 进入评论状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出评论状态-----");
        _restaurantCharacter.ClearReceiveFood();
    }

    public void Update()
    {
        
        
    }
}

public class CharacterThinking : CharacterBehaviour
{
    // public RestaurantRoleBase Char { get; }
    public behaviour BehaviourID => behaviour.Thinking;
    private RestaurantCharacter _restaurantCharacter;
    public CharacterThinking()
    {
        
    }

    public void Enter(RestaurantRoleBase restaurantRoleBase)
    {
        _restaurantCharacter = restaurantRoleBase  as RestaurantCharacter;
        
        _restaurantCharacter.PlayAnimation(BehaviourID);
        
        //随机组
        int id = _restaurantCharacter.TBBaseInfo.BehaviourGroup;
        var tb = DataProviderModule.Instance.GetBehaviourGroup(id);
        int total_weight = 0;
        foreach (var one in tb.Group)
        {
            total_weight += one.Weight;
        }

        int val = Random.Range(0, total_weight);
        total_weight = 0;
        behaviour next_behaviour = behaviour.Leave;
        for (int i = 0; i < tb.Group.Count; i++)
        {
            if (val >= total_weight && val <= total_weight + tb.Group[i].Weight)
            {
                next_behaviour = tb.Group[i].Behaviour;
                break;
            }

            total_weight += tb.Group[i].Weight;
        }
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入思考状态 下一个状态 {next_behaviour}-----");
        
        switch (next_behaviour)
        {
            case behaviour.Leave:
                _restaurantCharacter.CurBehaviour = new CharacterLeave();
                break;
            case behaviour.Talk:
                _restaurantCharacter.CurBehaviour = new CharacterTalk();
                break;
            case behaviour.OrderDrink:
                _restaurantCharacter.CurBehaviour = new CharacterOrderDrink(); 
                break;
            case behaviour.OrderMeal:
                _restaurantCharacter.CurBehaviour = new CharacterOrderMeal();
                break;
        }
        
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出思考状态-----");
    }

    public void Update()
    {
        
    }
}


public class CharacterLeave : CharacterBehaviour
{
    // public RestaurantRoleBase Char => _restaurantCharacter;
    public behaviour BehaviourID => behaviour.Leave;
    private RestaurantRoleBase _restaurantCharacter;
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        _restaurantCharacter.Sprite
            .DOFade(0, 1.5f)
            .OnComplete(() =>
            {
                _restaurantCharacter.CurBehaviour = null;
                EventModule.Instance.CharacterLeaveTopic.OnNext(_restaurantCharacter);
                CharacterMgr.Instance.RemoveCharacter(_restaurantCharacter);
            });
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入 离开状态-----");
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出 离开状态-----");
    }
}



