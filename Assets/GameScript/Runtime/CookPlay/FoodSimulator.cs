using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class FoodSimulator : MonoBehaviour
{
    [HideInInspector]
    public PanSimulator panSimulator;
    public FriedFooData data;
    
    private Vector2 _previousCircleDirection;
    private Vector3 _direction;

    private float _velocity;
    
    // private float _curHeatVal;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Begin(CompositeDisposable handler)
    {
        this.FixedUpdateAsObservable().Subscribe(Simulator).AddTo(handler);
        this.UpdateAsObservable().Subscribe(Move).AddTo(handler);
    }

    private void Init()
    {
      
    }

    private void Simulator(Unit param)
    {
        _velocity -= (panSimulator.friction+data.friction)*Time.fixedDeltaTime;
        _previousCircleDirection = transform.localPosition.normalized;
    }

    private void Move(Unit param)
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
}
