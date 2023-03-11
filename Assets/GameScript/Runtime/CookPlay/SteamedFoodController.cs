using System;
using UniRx;
using UnityEngine;

public class SteamedFoodController : MonoBehaviour
{
    public enum ColliderType
    {
        Circle,Square,Variable
    }

    public ColliderType ColType;
    public IObservable<SteamedFoodController> OnClick => _onClick;
    public IObservable<SteamedFoodController> OnDrag => _onDrag;
    public IObservable<SteamedFoodController> OnPut => _onPut;

    private readonly Subject<SteamedFoodController> _onClick = new();
    private readonly Subject<SteamedFoodController> _onDrag = new();
    private readonly Subject<SteamedFoodController> _onPut = new();
    
    private CircleCollider2D _circleCollider2D;
    private Collider2D _collider2D;
    private PolygonCollider2D _edgeCollider2D;
    
    private DragableFood _dragableFood;
    private SpriteRenderer _spriteRenderer;

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

            return _edgeCollider2D.bounds;
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

        if (ColType == ColliderType.Variable)
        {
            _edgeCollider2D = GetComponent<PolygonCollider2D>();
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _dragableFood = GetComponent<DragableFood>();
        _dragableFood.Init();
    }

    public void Begin(CompositeDisposable handler)
    {
        _dragableFood.Begin(handler);
        _dragableFood.OnClick.Subscribe(_=>
        {
            _onClick.OnNext(this);
            _spriteRenderer.sortingOrder = 6;
        }).AddTo(handler);
        _dragableFood.OnDrag.Subscribe(_=>
        {
            _onDrag.OnNext(this);
        }).AddTo(handler);
        _dragableFood.OnPut.Subscribe(_=>
        {
            _spriteRenderer.sortingOrder = 5;
            _onPut.OnNext(this);
        }).AddTo(handler);
    }

    public void SetOverlap(bool overlap)
    {
        _isOverlap = overlap;
        _spriteRenderer.color = overlap ? Color.white : Color.red;
    }
}
