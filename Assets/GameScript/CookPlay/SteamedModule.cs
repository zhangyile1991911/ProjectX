using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SteamedModule : MonoBehaviour
{
    public List<SteamedFoodController> foods;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var one in foods)
        {
            one.Init();
            one.OnClick.Subscribe(click).AddTo(gameObject);
            one.OnDrag.Subscribe(move).AddTo(gameObject);
            one.OnPut.Subscribe(put).AddTo(gameObject);
            
            one.transform.position = Random.insideUnitCircle*3f;
        }
    }

    void click(SteamedFoodController p)
    {
        
    }

    void move(SteamedFoodController controller)
    {
        // Debug.Log($"move {controller.name}");
        bool intersects = false;
        for (int y = 0; y < foods.Count; y++)
        {
            var staticFood = foods[y];
            if(staticFood.gameObject == controller.gameObject) continue;
            Debug.Log($"staticFood = {staticFood.name}");
            intersects = staticFood.Bounds.Intersects(controller.Bounds);
            // Debug.Log($"p.Bounds = {p.Bounds} other.Bounds = {other.Bounds}");
            if (intersects)
            {
                Debug.Log($"{controller.name} 与 {staticFood.name} 发生碰撞");
                break;
            }
        }
        Debug.Log($"=============================================");
        controller.ChangeColor(intersects?Color.red: Color.white);
    }

    void put(SteamedFoodController p)
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
