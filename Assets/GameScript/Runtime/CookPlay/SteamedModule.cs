using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using Cysharp.Text;
using UniRx;
using UniRx.Triggers;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = UnityEngine.Random;

public class SteamedModule : MonoBehaviour
{
    public Transform FoodRoot;
    public Transform SteamShelf;
    public Transform GameOverTxt;
    private List<SteamedFoodController> _foods;
    private CompositeDisposable _handle;
    private List<List<SteamedFoodController>> _quadtree;
    private Recipe _curRecipe;
    private bool _start;

    private Subject<bool> _completeTopic;

    private SteamedFoodController _curDragFood;
    private Vector3 _curOriginPos;

    private CircleCollider2D _boundary;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init()
    {
        _quadtree ??= new List<List<SteamedFoodController>>();
        _quadtree.Add(new List<SteamedFoodController>());
        _quadtree.Add(new List<SteamedFoodController>());
        _quadtree.Add(new List<SteamedFoodController>());
        _quadtree.Add(new List<SteamedFoodController>());
        _start = false;
        _handle ??= new CompositeDisposable();

        _boundary = SteamShelf.GetComponent<CircleCollider2D>();
    }

    public void SetRecipe(SteamedRecipe recipe)
    {
        _curRecipe = recipe;
        foreach (var one in recipe.Sets)
        {
            for (int i = 0; i < one.Key; i++)
            {
                var go = Instantiate(one.Value, FoodRoot);
                var controller = go.GetComponent<SteamedFoodController>();
                AddSteamedFood(controller);
            }
        }
    }

    private void AddSteamedFood(SteamedFoodController food)
    {
        // var position = food.transform.position;
        // position = Random.insideUnitCircle*3f;
        food.Init();

        food.OnClick.Where(_ => _start && null == _curDragFood).Subscribe(click).AddTo(_handle);
        food.OnDrag.Where(_ => _start && null != _curDragFood).Subscribe(move).AddTo(_handle);
        food.OnPut.Where(_ => _start && null != _curDragFood).Subscribe(put).AddTo(_handle);

        var position = Random.insideUnitCircle * 3f;
        food.transform.position = position;

        _foods ??= new List<SteamedFoodController>(20);
        _foods.Add(food);
        food.name = ZString.Format("{0}", _foods.Count);
        InsertQuardTree(food);
    }

    #region 四叉树

    private Vector2 leftTop = new();
    private Vector2 leftBottom = new();
    private Vector2 rightTop = new();
    private Vector2 rightBottom = new();

    private void DetermineQuadTree(SteamedFoodController controller)
    {
        controller.QuadTree?.Clear();
        if (controller.ColType == SteamedFoodController.ColliderType.Square ||
            controller.ColType == SteamedFoodController.ColliderType.Circle)
        {
            //查询四个点位置
            var localPosition = controller.transform.localPosition;
            var bounds = controller.Bounds;
            leftTop.x = localPosition.x - bounds.size.x / 2f;
            leftTop.y = localPosition.y + bounds.size.y / 2f;
            var index = InWhichTreeIndex(leftTop);
            if (index >= 0)
            {
                controller.QuadTree.Add(index);
            }

            leftBottom.x = localPosition.x - bounds.size.x / 2f;
            leftBottom.y = localPosition.y - bounds.size.y / 2f;
            index = InWhichTreeIndex(leftBottom);
            if (index >= 0)
            {
                controller.QuadTree.Add(index);
            }

            rightTop.x = localPosition.x + bounds.size.x / 2f;
            rightTop.y = localPosition.y + bounds.size.y / 2f;
            index = InWhichTreeIndex(rightTop);
            if (index >= 0)
            {
                controller.QuadTree.Add(index);
            }

            rightBottom.x = localPosition.x - bounds.size.x / 2f;
            rightBottom.y = localPosition.y - bounds.size.y / 2f;
            index = InWhichTreeIndex(rightBottom);
            if (index >= 0)
            {
                controller.QuadTree.Add(index);
            }
        }

        if (SteamedFoodController.ColliderType.Polygon == controller.ColType)
        {
            foreach (var one in controller.PolygonCollider2D.points)
            {
                var transform1 = controller.transform;
                leftTop.x = transform1.localPosition.x + one.x;
                leftTop.y = transform1.localPosition.y + one.y;
                var index = InWhichTreeIndex(leftTop);
                if(index > 0) controller.QuadTree.Add(index);
            }
        }

        foreach (var one in controller.QuadTree)
        {
            Debug.Log($"判断当前食物{controller.name} 在 {one}象限");
        }
    }

    private void InsertQuardTree(SteamedFoodController controller)
    {
        DetermineQuadTree(controller);
        Debug.Log($"{controller.name} 在{controller.QuadTree.Count}个象限中");
        foreach (var one in controller.QuadTree)
        {
            _quadtree[one].Add(controller);
        }
    }

    private void CheckOverlap(SteamedFoodController food)
    {
        foreach (var treeIndex in food.QuadTree)
        {
            var tree = _quadtree[treeIndex];
            for (int i = 0; i < tree.Count; i++)
            {
                var staticFood = tree[i];
                var overlap = staticFood.Bounds.Intersects(food.Bounds);
                staticFood.SetOverlap(overlap || staticFood.Overlap);
                food.SetOverlap(overlap);
            }
        }
    }

    public int InWhichTreeIndex(Vector3 localPos)
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

    private void CheckTreeOverlap(List<SteamedFoodController> tree)
    {
        for (int x = 0; x < tree.Count; x++)
        {
            var anchorObj = tree[x];
            for (int y = 0; y < tree.Count; y++)
            {
                var checkObj = tree[y];
                if (anchorObj.gameObject == checkObj.gameObject)
                {
                    continue;
                }

                var overlap = anchorObj.Bounds.Intersects(checkObj.Bounds);
                // if (overlap)
                // {
                //     Debug.Log($"{anchorObj.name} overlap {checkObj.name}");
                // }
                anchorObj.SetOverlap(overlap || anchorObj.Overlap);
                checkObj.SetOverlap(overlap || checkObj.Overlap);
            }
        }
    }

    #endregion

    public void StartGame()
    {
        _curDragFood = null;
        _start = true;

        GameOverTxt.gameObject.SetActive(false);
        foreach (var one in _foods)
        {
            one.Begin(_handle);
        }

        this.UpdateAsObservable()
            .Where(_ => _start)
            .Subscribe(CheckAllFoodOverlap)
            .AddTo(_handle);

        var timer = Observable
            .Timer(TimeSpan.FromSeconds(_curRecipe.duration))
            .Select(_ => false);
        this.UpdateAsObservable().Subscribe(CheckGameOver).AddTo(_handle);
        _completeTopic = new Subject<bool>();
        Observable.Amb(timer, _completeTopic).Subscribe(GameOver).AddTo(_handle);
    }

    public void GameOver(bool success)
    {
        Debug.Log($"!!!!!Game Over!!!!!");
        _handle?.Clear();
        _curDragFood = null;
        GameOverTxt.gameObject.SetActive(true);
    }

    private void CheckAllFoodOverlap(Unit param)
    {
        foreach (var one in _foods)
        {
            one.ResetOverlap();
        }

        foreach (var tree in _quadtree)
        {
            CheckTreeOverlap(tree);
        }
    }

    private void CheckGameOver(Unit param)
    {
        if (_curDragFood != null) return;

        var allDepart = true;
        foreach (var one in _foods)
        {
            if (one.Overlap)
            {
                allDepart = false;
                break;
            }

            var distance = 0f;
            // var maxDistance = _boundary.radius*SteamShelf.transform.localScale.x;
            var isIn = CheckFoodInBoundary(one);
            if (!isIn)
            {
                allDepart = false;
                break;
            }
            // Debug.Log($"maxDistance = {maxDistance}");
            // switch (one.ColType)
            // {
            //     case SteamedFoodController.ColliderType.Circle:
            //          distance = Vector2.Distance(one.transform.position, SteamShelf.transform.position);
            //         if (distance > maxDistance - one.CircleCollider2D.radius * 2f)
            //         {
            //             Debug.Log($"{one.name}不在范围内");
            //             allDepart = false;
            //         }
            //         break;
            //     case SteamedFoodController.ColliderType.Polygon:
            //         var worldPoint = one.transform.position;
            //         foreach (var point in one.PolygonCollider2D.points)
            //         {
            //             worldPoint.x += point.x;
            //             worldPoint.y += point.y;
            //             distance = Vector2.Distance(worldPoint, SteamShelf.transform.position);
            //             if (distance > maxDistance)
            //             {
            //                 Debug.Log($"{one.name}不在范围内");
            //                 allDepart = false;
            //                 break;
            //             }
            //         }
            //         break;
            //     case SteamedFoodController.ColliderType.Square:
            //         // Debug.Log($"name = {one.name} min = {one.Collider2D.bounds.min} max = {one.Collider2D.bounds.max}");
            //         distance = Vector2.Distance(one.Collider2D.bounds.min, SteamShelf.transform.position);
            //         bool leftBottom = distance < maxDistance;
            //         distance = Vector2.Distance(one.Collider2D.bounds.max, SteamShelf.transform.position);
            //         bool rightTop = distance < maxDistance;
            //         if (!leftBottom || !rightTop)
            //         {
            //             Debug.Log($"{one.name}不在范围内");
            //             allDepart = false;
            //         }
            //         break;
            // }
        }

        if (allDepart)
        {
            _completeTopic.OnNext(true);
            _completeTopic.OnCompleted();
        }
    }

    private bool CheckFoodInBoundary(SteamedFoodController p)
    {
        var distance = 0f;
        var maxDistance = _boundary.radius * SteamShelf.transform.localScale.x;
        switch (p.ColType)
        {
            case SteamedFoodController.ColliderType.Circle:
                distance = Vector2.Distance(p.transform.position, SteamShelf.transform.position);
                if (distance > maxDistance - p.CircleCollider2D.radius * 2f)
                {
                    Debug.Log($"{p.name}不在范围内");
                    return false;
                }

                break;
            case SteamedFoodController.ColliderType.Polygon:
                var worldPoint = p.transform.position;
                foreach (var point in p.PolygonCollider2D.points)
                {
                    worldPoint.x += point.x;
                    worldPoint.y += point.y;
                    distance = Vector2.Distance(worldPoint, SteamShelf.transform.position);
                    if (distance > maxDistance)
                    {
                        Debug.Log($"{p.name}不在范围内");
                        return false;
                    }
                }

                break;
            case SteamedFoodController.ColliderType.Square:
                // Debug.Log($"name = {one.name} min = {one.Collider2D.bounds.min} max = {one.Collider2D.bounds.max}");
                distance = Vector2.Distance(p.Collider2D.bounds.min, SteamShelf.transform.position);
                bool leftBottom = distance < maxDistance;
                distance = Vector2.Distance(p.Collider2D.bounds.max, SteamShelf.transform.position);
                bool rightTop = distance < maxDistance;
                if (!leftBottom || !rightTop)
                {
                    Debug.Log($"{p.name}不在范围内");
                    return false;
                }

                break;
        }

        return true;
    }

    void click(SteamedFoodController p)
    {
        Debug.Log($"click {p.name}");
        p.ResetOverlap();
        foreach (var treeIndex in p.QuadTree)
        {
            var tree = _quadtree[treeIndex];
            tree.Remove(p);
        }

        _curDragFood = p;
        _curOriginPos = p.transform.localPosition;
    }

    void move(SteamedFoodController controller)
    {
        if (_curDragFood.gameObject != controller.gameObject) return;
        Debug.Log($"move {controller.name}");
        //是否在范围内
        var inBoundary = CheckFoodInBoundary(controller);
        if (inBoundary)
        {
            controller.SetOverlap(false);
            //先确定在哪几个象限里
            DetermineQuadTree(controller);
            //遍历是否叠加
            CheckOverlap(controller);
        }
        else
        {
            controller.SetOverlap(true);
        }
    }

    void put(SteamedFoodController p)
    {
        Debug.Log($"put {p.name}");
        var completeIn = CheckFoodInBoundary(p);
        if (!completeIn)
        {
            _curDragFood.transform.localPosition = _curOriginPos;
        }

        InsertQuardTree(p);
        // var inBoundary = _boundary.bounds.Intersects(p.Bounds);
        // if (inBoundary)
        // {
        //     InsertQuardTree(p);
        // }
        // else
        // {
        //     p.SetOverlap(true);
        // }
        _curDragFood = null;
    }
}
