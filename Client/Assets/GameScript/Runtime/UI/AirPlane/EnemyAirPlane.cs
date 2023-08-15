using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAirPlane : MonoBehaviour
{
    public RectTransform AirPlaneRectTransform => controller;
    private RectTransform controller;
    
    private int hp;
    private Vector2 direction;
    private float speed;

    private Vector2 newPos;
    private Vector2 destPos;
    
    private float shot_invertal;
    
    public void Reset()
    {
        newPos = Vector3.zero;
        destPos = Vector3.zero;
        speed = 0.5f;
        shot_invertal = 0.5f;
    }
    public void Move()
    {
        var dist = Vector2.Distance(controller.anchoredPosition, destPos);
        // Debug.Log($"dist = {dist}");
        if (dist <= 0.5f)
        {
            // Debug.Log($"dist returndist returndist returndist returndist returndist return");
            return;
        }
        newPos.x += direction.x * speed;
        newPos.y += direction.y * speed;
        controller.anchoredPosition = newPos;
    }

    public void SetDestination(Vector2 dest)
    {
        destPos = dest;
        direction = (dest - controller.anchoredPosition).normalized;
    }

    public bool Fire()
    {
        if (shot_invertal <= 0)
        {
            shot_invertal = 0.3f;
            return true;
        }

        shot_invertal -= Time.deltaTime;
        return false;
    }
}
