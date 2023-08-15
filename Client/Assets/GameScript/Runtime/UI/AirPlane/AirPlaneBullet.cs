using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AirPlaneBullet : MonoBehaviour
{
    public  RectTransform BRectTransform;
    private Vector2 Direction;
    private ObjectPool<AirPlaneBullet> BelongPool;
    private Vector2 Bound;
    private Vector2 newPos;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag(tag))
        {
            return;
        }
        Debug.Log("OnCollisionEnter2D 回收");
        BelongPool.Release(this);
    }

    public void Init(RectTransform launcher,ObjectPool<AirPlaneBullet> pool,Vector2 bound,bool up)
    {
        BelongPool = pool;
        Bound = bound;
        newPos = launcher.anchoredPosition;
        BRectTransform.anchoredPosition = launcher.anchoredPosition;
        tag = launcher.tag;

        Direction.x = launcher.anchoredPosition.x;
        Direction.y = up ? 3.0f : -3.0f;
    }
    
    public bool OnUpdate()
    {
        newPos.y += Direction.y;
        BRectTransform.anchoredPosition = newPos;
        return newPos.y < Bound.x && newPos.y > Bound.y;
    }
    
}
