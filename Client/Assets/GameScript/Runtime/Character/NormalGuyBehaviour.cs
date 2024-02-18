
using cfg.character;
using DG.Tweening;
using UnityEngine;

public class NormalGuyEnterScene : CharacterBehaviour
{
    public behaviour BehaviourID => behaviour.Enter;
    private RestaurantRoleBase _restaurantCharacter;

    public behaviour PreBehaviourID => behaviour.Enter;
    
    private Vector3 destPoint;
    public NormalGuyEnterScene(Vector3 dest)
    {
        destPoint = dest;
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
            
            _restaurantCharacter.CurBehaviour = new NormalGuyOrderMeal();
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



public class NormalGuyOrderMeal : CharacterBehaviour
{
    public behaviour BehaviourID => behaviour.OrderMeal;
    private long _preDateTime;
    private RestaurantCharacter _restaurantCharacter;
    public behaviour PreBehaviourID => behaviour.OrderMeal;


    public NormalGuyOrderMeal()
    {

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
            Debug.Log($"{_restaurantCharacter.CharacterName} 点单超过上限");
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
        // if (_restaurantCharacter.GenerateOrder())
        //     _restaurantCharacter.CurBehaviour = new NormalGuyWaitOrder();
    }
}


public class NormalGuyWaitOrder : CharacterBehaviour
{
    public behaviour BehaviourID => behaviour.WaitOrder;
    private RestaurantNPC _restaurantCharacter;
    public behaviour PreBehaviourID => behaviour.WaitOrder;
    private long timestamp;
    private int attenuation;
    public NormalGuyWaitOrder()
    {
        
    }
    
    public void Enter(RestaurantRoleBase restaurantCharacter)
    {
        _restaurantCharacter = restaurantCharacter as RestaurantNPC;
        _restaurantCharacter.PlayAnimation(BehaviourID);
        timestamp = Clocker.Instance.NowDateTime.Timestamp;
        _restaurantCharacter.ResetPatient();
        _restaurantCharacter.ShowOrderBoard();
        attenuation = DataProviderModule.Instance.AttenuatePatientValue();
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 进入等待上菜状态-----");
    }

    public void Exit()
    {
        Debug.Log($"-----{_restaurantCharacter.CharacterName} 退出等待上菜状态-----");
        _restaurantCharacter.HideOrderBoard();
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
            Debug.Log($"_restaurantCharacter {_restaurantCharacter.CharacterName} 耐心耗尽退出等待");
            _restaurantCharacter.CurBehaviour = new CharacterLeave();
        }
    }
}