using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class PanSimulator : MonoBehaviour
{
    // public Slider heatSlider;
    public Transform panHandle;
    // public Transform firePoint;
    // [InspectorName("距离热度比例")]
    // public AnimationCurve heatCurve;
    public float velocity => _velocity;
    public float friction = 1.0f;
    public float edgeBounce = 50.0f;
    public float maxVelocity = 10f;
    
    private Camera _mainCamera;
    private Vector3 _panMoveDir;
    private Vector3 _previousPosition;
    private Vector3 _offset;
    private float _velocity;
    private float _curHeatSpeed;
    private bool _isDrag = false;
    private List<List<FoodSimulator>> _quadtree;
    private List<FoodSimulator> _foodList;
    // Start is called before the first frame update
    void Start()
    {
        // Init();
    }

    public void Init()
    {
        _mainCamera = Camera.main;
        
        _previousPosition = transform.position;
        
        _foodList = new List<FoodSimulator>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var one = transform.GetChild(i).GetComponent<FoodSimulator>(); ;
            if (one == null) continue;
            one.panSimulator = this;
            _foodList.Add(one);
        }

        _quadtree = new List<List<FoodSimulator>>
        {
            new List<FoodSimulator>(),
            new List<FoodSimulator>(),
            new List<FoodSimulator>(),
            new List<FoodSimulator>()
        };
        // Observable.EveryFixedUpdate().Subscribe(UpdatePan);
        // this.FixedUpdateAsObservable().Subscribe(UpdatePan);
        // Observable.EveryUpdate().Subscribe(HeatFood);
    }

    Vector3 ScreenToWorld(Vector3 mousePos, Transform targetTransform)
    {
        var cameraTransform = _mainCamera.transform;
        Vector3 dir = targetTransform.position - cameraTransform.position;
        Vector3 normalDir = Vector3.Project(dir, cameraTransform.forward);
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, normalDir.magnitude));
        return worldPos;
    }

    void DragHandle()
    {
        if (Input.GetMouseButtonDown(0) && !_isDrag)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit && hit.collider.transform == panHandle)
            {
                Vector3 pos = ScreenToWorld(Input.mousePosition, transform);
                _offset = transform.position - pos;
                _isDrag = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isDrag = false;
        }
        if (Input.GetMouseButton(0) && _isDrag)
        {
            Vector3 pos = ScreenToWorld(Input.mousePosition, transform);
            transform.position = pos + _offset;
        }
    }
    
    public void UpdatePan(Unit param)
    {
        DragHandle();

        ClearQuadtree();

        var position = transform.position;
        float distance = Vector2.Distance(position, _previousPosition);

        // _velocity = distance / Time.fixedDeltaTime;
        _velocity = distance;
        _velocity = Mathf.Clamp(_velocity, 0f, maxVelocity);

        _panMoveDir = (position - _previousPosition).normalized;

        _previousPosition = position;

        foreach (var one in _foodList)
        {
            distance = Vector2.Distance(Vector2.zero, one.transform.localPosition);
            if (distance >= 0.4f)
            {
                one.AddVelocityAndDirection(edgeBounce, one.transform.localPosition.normalized * -1f);
            }
            else
            {
                one.AddVelocityAndDirection(_velocity, _panMoveDir);
            }
            AddQuadtree(one);
        }
        QuadCollision();
    }
    
    void ClearQuadtree()
    {
        foreach (var one in _quadtree)
        {
            one.Clear();
        }
    }
    
    void AddQuadtree(FoodSimulator one_food)
    {
        if (one_food.transform.localPosition is { x: > 0, y: > 0 })
        {
            _quadtree[0].Add(one_food);
        }
        else if (one_food.transform.localPosition is { x: < 0, y: > 0 })
        {
            _quadtree[1].Add(one_food);
        }
        else if (one_food.transform.localPosition is { x: < 0, y: < 0 })
        {
            _quadtree[2].Add(one_food);
        }
        else
        {
            _quadtree[3].Add(one_food);
        }
    }

    void QuadCollision()
    {
        int count = 0;
        for (int  x = 0;x < _quadtree.Count;x++)
        {
            for(int y = 0;y < _quadtree[x].Count;y++)
            {
                FoodSimulator onefood = _quadtree[x][y];
                for(int z = 0;z < _quadtree[x].Count;z++)
                {
                    FoodSimulator otherFood = _quadtree[x][z];
                    if (onefood == otherFood) continue;

                    float distance = Vector2.Distance(otherFood.transform.localPosition, onefood.transform.localPosition);
                    if (distance > 0.12f) continue;
                    
                    Vector3 reverseDirection = (onefood.transform.localPosition - otherFood.transform.localPosition).normalized;
                    if(reverseDirection.magnitude <= 0.01f)
                    {
                        reverseDirection.x = Random.Range(-1f,1f);
                        reverseDirection.y = Random.Range(-1f, 1f);
                    }
                    float newpower = (1 + (1 - distance / 0.2f)) * otherFood.data.bounce*0.5f;
                    onefood.AddVelocityAndDirection(newpower, reverseDirection);
                    count++;
                }
            }
        }

        if (count > 0)
        {
            Debug.Log($"QuadCollision一帧循环了{count}次数");    
        }
        
    }

    // public void HeatFood(float heatSpeed)
    // {
    //     foreach (var oneFood in _foodList)
    //     {
    //         oneFood.Heat(heatSpeed);
    //     }
    // }

}
