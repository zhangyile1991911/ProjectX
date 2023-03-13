using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UniRx;
using UniRx.Triggers;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = UnityEngine.Random;

public class SteamedModule : MonoBehaviour
{
    public Transform SteamShelf;
    private List<SteamedFoodController> _foods;
    private CompositeDisposable _handle;
    private List<List<SteamedFoodController>> _quadtree;
    private Recipe _curRecipe;
    private bool _start;

    private Subject<bool> _completeTopic;

    private SteamedFoodController _curDragFood;
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
    }

    public void SetRecipe(SteamedRecipe recipe)
    {
        _curRecipe = recipe;
        foreach (var one in recipe.Sets)
        {
            for (int i = 0; i < one.Key; i++)
            {
                var go = Instantiate(one.Value,SteamShelf);
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
        
        food.OnClick.Where(_=>_start && null ==_curDragFood).Subscribe(click).AddTo(_handle);
        food.OnDrag.Where(_=>_start && _curDragFood != null).Subscribe(move).AddTo(_handle);
        food.OnPut.Where(_=>_start &&  _curDragFood != null).Subscribe(put).AddTo(_handle);
        
        var position = Random.insideUnitCircle*3f;
        food.transform.position = position;

        _foods ??= new List<SteamedFoodController>(20);
        _foods.Add(food);
        food.name = ZString.Format("{0}", _foods.Count);
        InsertQuardTree(food);
        // var tree = InWhichTree(position);
        // if (tree==null)
        // {
        //     Debug.LogError($"{position} InWhichTree return null");
        // }
        // else
        // {
        //     // Debug.Log($"{food.name} local position = {position} 加入第{tree.Count}象限");
        //     food.name = ZString.Format("position={0}",position);
        //     tree.Add(food);    
        // }
    }

    #region 四叉树

    private void DetermineQuadTree(SteamedFoodController controller)
    {
        controller.QuadTree?.Clear();
        if (controller.ColType == SteamedFoodController.ColliderType.Square || controller.ColType == SteamedFoodController.ColliderType.Circle)
        {//查询四个点位置
            var localPosition = controller.transform.localPosition;
            var bounds = controller.Bounds;
            Vector2 leftTop = new Vector2(
                localPosition.x - bounds.size.x/2f,
                localPosition.y + bounds.size.y/2f);
            var index = InWhichTreeIndex(leftTop);
            controller.QuadTree.Add(index);

            Vector2 leftBottom = new Vector2(
                localPosition.x - bounds.size.x/2f,
                localPosition.y - bounds.size.y/2f);
            index = InWhichTreeIndex(leftBottom);
            controller.QuadTree.Add(index);

            Vector2 rightTop = new Vector2(
                localPosition.x + bounds.size.x/2f,
                localPosition.y + bounds.size.y/2f);
            index = InWhichTreeIndex(rightTop);
            controller.QuadTree.Add(index);

            Vector2 rightBottom = new Vector2(
                localPosition.x - bounds.size.x/2f,
                localPosition.y - bounds.size.y/2f);
            index = InWhichTreeIndex(rightBottom);
            controller.QuadTree.Add(index);
        }

        if (SteamedFoodController.ColliderType.Variable == controller.ColType)
        {
            foreach (var one in controller.EdgeCollider2D.points)
            {
                var transform1 = controller.transform;
                var index = InWhichTreeIndex(new Vector2(
                    transform1.localPosition.x+one.x,
                    transform1.localPosition.y+one.y)
                );
                controller.QuadTree.Add(index);
            }
        }
        Debug.Log($"判断当前食物{controller.name} 在 {controller.QuadTree.Count}个象限");
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
    
    private List<SteamedFoodController> CheckOverlap(SteamedFoodController food)
    {
        List<SteamedFoodController> controllers = new List<SteamedFoodController>();
        foreach (var treeIndex in food.QuadTree)
        {
            var tree = _quadtree[treeIndex];
            for (int i = 0; i < tree.Count; i++)
            {
                var staticFood = tree[i];
                // if(staticFood.gameObject == food.gameObject)continue;

                var overlap = staticFood.Bounds.Intersects(food.Bounds);
                if (overlap)controllers.Add(staticFood);
            }    
        }
        return controllers;
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
                if (overlap)
                {
                    Debug.Log($"{anchorObj.name} overlap {checkObj.name}");
                }
                anchorObj.SetOverlap(overlap||anchorObj.Overlap);
                checkObj.SetOverlap(overlap||checkObj.Overlap);
            }
        }
    }
    #endregion

    public void StartGame()
    {
        _curDragFood = null;
        _start = true;
        foreach (var tree in _quadtree)
        {
            CheckTreeOverlap(tree);
        }

        foreach (var one in _foods)
        {
            one.Begin(_handle);
        }
        
        var timer = Observable
            .Timer(TimeSpan.FromSeconds(_curRecipe.duration))
            .Select(_=>false);
        this.UpdateAsObservable().Subscribe(CheckGameOver).AddTo(_handle);
        _completeTopic = new Subject<bool>();
        Observable.Amb(timer,_completeTopic).Subscribe(GameOver).AddTo(_handle);
    }

    public void GameOver(bool success)
    {
        _handle?.Clear();
        _curDragFood = null;
    }
    
    private void CheckGameOver(Unit param)
    {
        var allDepart = true;
        for (int i = 0; i < _foods.Count; i++)
        {
            if (_foods[i].Overlap)
            {
                allDepart = false;
                break;
            }
        }

        if (allDepart)
        {
            _completeTopic.OnNext(true);
            _completeTopic.OnCompleted();
        }
    }
    
    void click(SteamedFoodController p)
    {
        Debug.Log($"click {p.name}");
        foreach (var treeIndex in p.QuadTree)
        {
            var tree = _quadtree[treeIndex];
            tree.Remove(p);
            CheckTreeOverlap(tree);
        }
        _curDragFood = p;
        // var tree = InWhichTree(p.transform.localPosition);
        // if (tree != null)
        // {
        //     tree.Remove(p);
        //     CheckTreeOverlap(tree);    
        // }
    }

    void move(SteamedFoodController controller)
    {
        if (_curDragFood.gameObject != controller.gameObject) return;
        Debug.Log($"click {controller.name}");
        //先确定在哪几个象限里
        DetermineQuadTree(controller);
        //遍历是否叠加
        CheckOverlap(controller);
        // var overlapObj = CheckOverlap(controller);
        // if (overlapObj != null)
        // {
        //     controller.SetOverlap(true);
        //     overlapObj.SetOverlap(true);
        // }
    }

    void put(SteamedFoodController p)
    {
        Debug.Log($"click {p.name}");
        
        InsertQuardTree(p);
        _curDragFood = null;
    }

}
