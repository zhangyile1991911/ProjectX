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


    // Start is called before the first frame update
    void Start()
    {
        
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
        
        _dragableFood.OnClick.Subscribe(_=>
        {
            _onClick.OnNext(this);
            _spriteRenderer.sortingOrder = 6;
        }).AddTo(gameObject);
        _dragableFood.OnDrag.Subscribe(_=>
        {
            _onDrag.OnNext(this);
        }).AddTo(gameObject);
        _dragableFood.OnPut.Subscribe(_=>
        {
            _spriteRenderer.sortingOrder = 5;
            _onPut.OnNext(this);
        }).AddTo(gameObject);
        // _collider2D.bounds.Intersects();
        // var circle = GetComponent<CircleCollider2D>();
        // circle.bounds.Intersects();
    }

    public void ChangeColor(Color color)
    {
        _spriteRenderer.color = color;
    }
}
