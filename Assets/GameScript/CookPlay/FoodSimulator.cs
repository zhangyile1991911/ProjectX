using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FoodSimulator : MonoBehaviour
{
    public PanSimulator panSimulator;
    public FriedFooData data;
    
    private Vector2 _previousCircleDirection;
    private Vector3 _direction;

    private float _velocity;

    private float _curHeatVal;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        Observable.EveryFixedUpdate().Subscribe(Simulator).AddTo(gameObject);
        Observable.EveryUpdate().Subscribe(Move).AddTo(gameObject);
        _curHeatVal = 0;
    }

    void Simulator(long param)
    {
        _velocity -= (panSimulator.friction+data.friction)*Time.fixedDeltaTime;
        _previousCircleDirection = transform.localPosition.normalized;
    }

    void Move(long param)
    {
        if (_velocity >= 0)
        {
            transform.localPosition += _direction * Time.deltaTime;
        }
    }
    
    public void AddVelocityAndDirection(float vel,Vector3 dir)
    {
        _direction = _direction * _velocity + vel * dir;
        _direction.Normalize();
        
        float dotval = Vector2.Dot(_direction, dir);
        _velocity += vel * dotval;
        _velocity = Mathf.Clamp(_velocity,0,panSimulator.maxVelocity);

        _direction *= _velocity;
    }

    public void Heat(float val)
    {
        _curHeatVal += val;
        _curHeatVal = Mathf.Clamp(_curHeatVal, 0, data.MaxHeatCapacity);
    }
}
