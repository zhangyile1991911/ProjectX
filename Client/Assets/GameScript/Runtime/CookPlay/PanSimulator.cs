using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PanSimulator : MonoBehaviour
{
    // public Slider heatSlider;
    public Transform panHandle;

    public Transform animNode;

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

        _foodList ??= new List<FoodSimulator>();

        _quadtree = new List<List<FoodSimulator>>
        {
            new List<FoodSimulator>(),
            new List<FoodSimulator>(),
            new List<FoodSimulator>(),
            new List<FoodSimulator>()
        };
    }

    private Vector2 leftTop = new Vector2();
    private Vector2 leftBottom = new Vector2();
    private Vector2 rightTop = new Vector2();
    private Vector2 rightBottom = new Vector2();

    public void AddFood(FoodSimulator food)
    {
        if (food == null) return;
        food.panSimulator = this;
        _foodList.Add(food);
    }

    public void RemoveAllFood()
    {
        foreach (var one in _foodList)
        {
            one.panSimulator = null;
            Destroy(one.gameObject);
        }

        _foodList.Clear();
    }

    private int InWhichTreeIndex(Vector3 localPos)
    {
        if (localPos is { x: > 0, y: > 0 })
        {
            return 0;
        }

        if (localPos is { x: < 0, y: > 0 })
        {
            return 1;
        }

        if (localPos is { x: < 0, y: < 0 })
        {
            return 2;
        }

        if (localPos is { x: > 0, y: < 0 })
        {
            return 3;
        }

        return -1;
    }

    public void GameStart(CompositeDisposable handler)
    {
        this.UpdateAsObservable().Subscribe(UpdatePan).AddTo(handler);
        foreach (var one in _foodList)
        {
            one.Begin(handler);
        }
    }

    private Vector3 ScreenToWorld(Vector3 mousePos, Transform targetTransform)
    {
        var cameraTransform = _mainCamera.transform;
        Vector3 dir = targetTransform.position - cameraTransform.position;
        Vector3 normalDir = Vector3.Project(dir, cameraTransform.forward);
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, normalDir.magnitude));
        return worldPos;
    }

    // private Ray gizomsRay;
    // private void OnDrawGizmos()
    // {
    //     // Debug.Log($"origin = ${gizomsRay.origin} direction = ${gizomsRay.direction}");
    //     // Gizmos.DrawRay(gizomsRay.origin,gizomsRay.direction);    
    //     Debug.DrawLine(gizomsRay.origin,gizomsRay.origin+gizomsRay.direction*100f,Color.red);
    // }
    
    private void DragHandle()
    {
        if (Input.GetMouseButtonDown(0) && !_isDrag)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            // gizomsRay = ray;
            RaycastHit hit;
            // RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if(Physics.Raycast(ray, out hit, 100f))
            // if (hit && hit.collider.transform == panHandle)
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

    private void UpdatePan(Unit param)
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

    private void ClearQuadtree()
    {
        foreach (var one in _quadtree)
        {
            one.Clear();
        }
    }

    private void AddQuadtree(FoodSimulator one_food)
    {
        var localPosition = one_food.transform.localPosition;
        var bounds = one_food.Bounds;

        leftTop.x = localPosition.x - bounds.size.x / 2f;
        leftTop.y = localPosition.y + bounds.size.y / 2f;
        var index = InWhichTreeIndex(leftTop);
        if (index >= 0)
        {
            one_food.QuadTreeIndex.Add(index);
            _quadtree[index].Add(one_food);
        }

        leftBottom.x = localPosition.x - bounds.size.x / 2f;
        leftBottom.y = localPosition.y - bounds.size.y / 2f;
        index = InWhichTreeIndex(leftBottom);
        if (index >= 0)
        {
            one_food.QuadTreeIndex.Add(index);
            _quadtree[index].Add(one_food);
        }

        rightTop.x = localPosition.x + bounds.size.x / 2f;
        rightTop.y = localPosition.y + bounds.size.y / 2f;
        index = InWhichTreeIndex(rightTop);
        if (index >= 0)
        {
            one_food.QuadTreeIndex.Add(index);
            _quadtree[index].Add(one_food);
        }


        rightBottom.x = localPosition.x - bounds.size.x / 2f;
        rightBottom.y = localPosition.y - bounds.size.y / 2f;
        index = InWhichTreeIndex(rightBottom);
        if (index >= 0)
        {
            one_food.QuadTreeIndex.Add(index);
            _quadtree[index].Add(one_food);
        }
    }

    private void QuadCollision()
    {
        int count = 0;
        foreach (var food in _foodList)
        {
            foreach (var treeIndex in food.QuadTreeIndex)
            {
                var curTree = _quadtree[treeIndex];
                foreach (var treeFood in curTree)
                {
                    if (treeFood.transform == food.transform) continue;
                    
                    float distance = Vector2.Distance(food.transform.localPosition, treeFood.transform.localPosition);
                    if (distance > food.Radius * 1.5f) continue;

                    Vector3 reverseDirection =
                        (food.transform.localPosition - treeFood.transform.localPosition).normalized;
                    if (reverseDirection.magnitude <= 0.01f)
                    {
                        reverseDirection.x = Random.Range(-1f, 1f);
                        reverseDirection.y = Random.Range(-1f, 1f);
                    }

                    float newpower = (1 + (1 - distance / 0.2f)) * treeFood.data.bounce * 0.5f;
                    food.AddVelocityAndDirection(newpower, reverseDirection);
                    count++;
                }
            }
        }
        
        if (count > 0)
        {
            Debug.Log($"QuadCollision一帧循环了{count}次数");
        }
    }
}