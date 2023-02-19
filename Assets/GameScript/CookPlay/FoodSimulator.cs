using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FoodSimulator : MonoBehaviour
{
    public PanSimulator panSimulator;
    public float friction;
    public float bounce;
    private Vector2 _previousCircleDirection;
    private Vector3 _direction;

    private float _velocity;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        Observable.EveryFixedUpdate().Subscribe(Simulator);
        Observable.EveryUpdate().Subscribe(Move);
    }

    void Simulator(long param)
    {
        _velocity -= (panSimulator.friction+friction)*Time.fixedDeltaTime;
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
}
