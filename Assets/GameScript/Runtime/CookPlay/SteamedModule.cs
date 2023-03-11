using System;
using System.Collections;
using System.Collections.Generic;
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
        var position = food.transform.position;
        position = Random.insideUnitCircle*3f;
        food.Init();
        
        food.OnClick.Where(_=>_start).Subscribe(click).AddTo(_handle);
        food.OnDrag.Where(_=>_start).Subscribe(move).AddTo(_handle);
        food.OnPut.Where(_=>_start).Subscribe(put).AddTo(_handle);
        
        position = Random.insideUnitCircle*3f;
        food.transform.position = position;

        _foods ??= new List<SteamedFoodController>(20);
        _foods.Add(food);
        var tree = InWhichTree(position);
        if (tree==null)
        {
            Debug.LogError($"{position} InWhichTree return null");
        }
        else
        {
            // Debug.Log($"{food.name} local position = {position} 加入第{tree.Count}象限");
            tree.Add(food);    
        }
    }

    public List<SteamedFoodController> InWhichTree(Vector3 localPos)
    {
        if (localPos is { x: > 0, y: > 0 })
        {
            Debug.Log($"local position = {localPos} 加入第1象限");
            return _quadtree[0];
        }
        if (localPos is { x: < 0, y: > 0 })
        {
            Debug.Log($"local position = {localPos} 加入第2象限");
            return _quadtree[1];
        }
        if (localPos is { x: < 0, y: < 0 })
        {
            Debug.Log($"local position = {localPos} 加入第3象限");
            return _quadtree[2];
        }
        if (localPos is { x: > 0, y: < 0 })
        {
            Debug.Log($"local position = {localPos} 加入第4象限");
            return _quadtree[3];
        }
        return null;
    }
    public void StartGame()
    {
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
    }

    private SteamedFoodController CheckOverlap(SteamedFoodController food)
    {
        var tree = InWhichTree(food.transform.localPosition);
        for (int i = 0; i < tree.Count; i++)
        {
            var staticFood = tree[i];
            if(staticFood.gameObject == food.gameObject)continue;

            var overlap = staticFood.Bounds.Intersects(food.Bounds);
            if (overlap) return staticFood;
        }
        return null;
    }

    private void CheckTreeOverlap(List<SteamedFoodController> tree)
    {
        for (int x = 0; x < tree.Count; x++)
        {
            for (int y = 0; y < tree.Count; y++)
            {
                if (tree[x].gameObject == tree[y].gameObject)
                {
                    continue;
                }

                var overlap = tree[x].Bounds.Intersects(tree[y].Bounds);
                tree[x].SetOverlap(overlap);
                tree[y].SetOverlap(overlap);
            }
        }
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
        var tree = InWhichTree(p.transform.localPosition);
        tree.Remove(p);
        CheckTreeOverlap(tree);
    }

    void move(SteamedFoodController controller)
    {
        // Debug.Log($"move {controller.name}");
        // bool overlap = false;
        // for (int y = 0; y < _foods.Count; y++)
        // {
        //     var staticFood = _foods[y];
        //     if(staticFood.gameObject == controller.gameObject) continue;
        //     Debug.Log($"staticFood = {staticFood.name}");
        //     overlap = staticFood.Bounds.Intersects(controller.Bounds);
        //     // Debug.Log($"p.Bounds = {p.Bounds} other.Bounds = {other.Bounds}");
        //     if (overlap)
        //     {
        //         Debug.Log($"{controller.name} 与 {staticFood.name} 发生碰撞");
        //         break;
        //     }
        // }
        // Debug.Log($"=============================================");
        // controller.SetOverlap(overlap);

        var tree = InWhichTree(controller.transform.localPosition);
        foreach (var one in tree)
        {
            var overlap = one.Bounds.Intersects(controller.Bounds);
            controller.SetOverlap(overlap);
            if (one.Overlap)
            {//本来就和其他重叠
                continue;
            }
            one.SetOverlap(overlap);
        }
    }

    void put(SteamedFoodController p)
    {
        var tree = InWhichTree(p.transform.localPosition);
        if(tree == null)
            Debug.LogError($"local position {p.transform.localPosition} in tree");
        
        tree.Add(p);
        CheckTreeOverlap(tree);
    }

}
