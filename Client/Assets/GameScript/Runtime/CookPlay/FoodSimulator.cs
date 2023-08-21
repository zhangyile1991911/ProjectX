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
    
    // private Vector2 _previousCircleDirection;
    private Vector3 _direction;

    private float _velocity;
    
    private HashSet<int> _quadTreeIndex = new();
    public HashSet<int> QuadTreeIndex => _quadTreeIndex ??= new HashSet<int>();

    private Bounds _bounds;
    private CircleCollider2D _circleCollider;
    public float Radius => transform.localScale.x * _circleCollider.radius;
    public Bounds Bounds => _bounds;
    public int CollisionLayer = -1;
    // private float _curHeatVal;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Begin(CompositeDisposable handler)
    {
        // this.FixedUpdateAsObservable().Subscribe(Simulator).AddTo(handler);
        this.UpdateAsObservable().Subscribe(Move).AddTo(handler);
    }

    private void Init()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _bounds = _circleCollider.bounds;
    }

    public void Simulator(Unit param)
    {
        _velocity -= (panSimulator.friction+data.friction)*Time.fixedDeltaTime;
        // _previousCircleDirection = transform.localPosition.normalized;
    }

    private void Move(Unit param)
    {
        if (_velocity >= 0)
        {
            // Debug.Log($"{name} before Move transform.localPosition = {transform.localPosition}");
            transform.localPosition += _direction * Time.deltaTime;
            // Debug.Log($"{name} after Move transform.localPosition = {transform.localPosition}");
        }
    }
    
    public void AddVelocityAndDirection(float vel,Vector3 dir)
    {
        _direction = _direction * _velocity + vel * dir;
        _direction.Normalize();
        
        float dotval = Vector2.Dot(_direction, dir);
        _velocity += vel * dotval;
        _velocity = Mathf.Clamp(_velocity,0,panSimulator.maxVelocity);
        // Debug.Log($"{name} _velocity = {_velocity}");
        _direction *= _velocity;
    }
}
