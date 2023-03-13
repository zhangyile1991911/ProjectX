using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class SteamedFoodController : MonoBehaviour
{
    public enum ColliderType
    {
        Circle,Square,Polygon
    }

    public ColliderType ColType;
    public IObservable<SteamedFoodController> OnClick => _onClick;
    public IObservable<SteamedFoodController> OnDrag => _onDrag;
    public IObservable<SteamedFoodController> OnPut => _onPut;

    private readonly Subject<SteamedFoodController> _onClick = new();
    private readonly Subject<SteamedFoodController> _onDrag = new();
    private readonly Subject<SteamedFoodController> _onPut = new();
    
    private CircleCollider2D _circleCollider2D;
    public CircleCollider2D CircleCollider2D => _circleCollider2D;
    
    private Collider2D _collider2D;
    public Collider2D Collider2D => _collider2D;
    private PolygonCollider2D _polygonCollider2D;
    public PolygonCollider2D PolygonCollider2D => _polygonCollider2D;
    
    private DragableFood _dragableFood;
    private SpriteRenderer _spriteRenderer;

    private HashSet<int> _quadTree = new();

    public HashSet<int> QuadTree
    {
        get
        {
            if (_quadTree == null)
            {
                _quadTree = new HashSet<int>();
            }

            return _quadTree;
        }
    }
    

    public bool Overlap => _isOverlap;
    private bool _isOverlap;
    public Bounds Bounds
    {
        get
        {
            if (ColType == ColliderType.Circle)
            {
                return _circleCollider2D.bounds;
            }
            if (ColType == ColliderType.Square)
            {
                return _collider2D.bounds;
            }

            return _polygonCollider2D.bounds;
        }
    }
    

    public void Init()
    {
        Debug.Log($"{name} Init() ColType = {ColType}");
        if (ColType == ColliderType.Square)
        {
            _collider2D = GetComponent<Collider2D>();
        }

        if (ColType == ColliderType.Circle)
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();
        }

        if (ColType == ColliderType.Polygon)
        {
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _dragableFood = GetComponent<DragableFood>();
        _dragableFood.Init();
    }

    public void Begin(CompositeDisposable handler)
    {
        _dragableFood.Begin(handler);
        _dragableFood.Click += (_=>
        {
            _onClick.OnNext(this);
            _spriteRenderer.sortingOrder = 6;
        });
        _dragableFood.Drag += (_=>
        {
            _onDrag.OnNext(this);
        });
        _dragableFood.Put += (_=>
        {
            _spriteRenderer.sortingOrder = 5;
            _onPut.OnNext(this);
        });
    }

    public void SetOverlap(bool overlap)
    {
        _isOverlap = overlap;
        _spriteRenderer.color = overlap ? Color.red:Color.white;
    }

    public void ResetOverlap()
    {
        _isOverlap = false;
    }
}
