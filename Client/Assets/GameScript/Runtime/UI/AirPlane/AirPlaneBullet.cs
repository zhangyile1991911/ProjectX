using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AirPlaneBullet : MonoBehaviour
{
    public  RectTransform BRectTransform;
    public Vector2 CurPos => BRectTransform.anchoredPosition;
    private Vector2 Direction;
    // private ObjectPool<AirPlaneBullet> BelongPool;
    
    private Vector2 newPos;
    private float speed;
    
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     // Debug.Log("AirPlaneBullet::OnTriggerEnter2D");
    //     if (other.CompareTag(tag))
    //     {
    //         return;
    //     }
    //
    //     if (other.name.Equals(name))
    //     {
    //         return;
    //     }
    //     // Debug.Log($"Bullet other.name = {other.transform.name}");
    //     // Debug.Log("OnTriggerEnter2D 回收");
    //     BelongPool.Release(this);
    // }

    public void Init(RectTransform launcher,float speed)
    {
        // BelongPool = pool;
        newPos = launcher.anchoredPosition;
        BRectTransform.anchoredPosition = launcher.anchoredPosition;
        tag = launcher.tag;

        Direction.x = launcher.anchoredPosition.x;
        Direction.y = speed;
    }

    public void Init(RectTransform launcher,Vector2 dir,float sped)
    {
        // BelongPool = pool;
        newPos = launcher.anchoredPosition;
        BRectTransform.anchoredPosition = launcher.anchoredPosition;
        tag = launcher.tag;
        Direction = dir;
        speed = sped;
    }
    
    public void Move()
    {
        newPos.y += Direction.y+speed*Time.deltaTime;
        BRectTransform.anchoredPosition = newPos;
    }
    
}
