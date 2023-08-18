using System;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyAirPlane : MonoBehaviour
{
    public RectTransform Controller;
    public Vector2 CurPos => newPos;
    public Vector2 Direction => direction;
    private int healthPoint;
    private Vector2 direction;
    private float flySpeed;

    private Vector2 newPos;
    private Vector2 destinationPos;
    
    private float shot_invertal;
    private float cur_shot_invertval;

    private Action<EnemyAirPlane> onRelease;
    public void ConfigProperty(Action<EnemyAirPlane> release,Vector2 startPos,Vector2 destPos,int hp,float speed = 0.7f,float shotInterval = 2.5f)
    {
        newPos = startPos;
        destinationPos = destPos;
        
        Controller.anchoredPosition = startPos;
        flySpeed = speed;
        cur_shot_invertval = shot_invertal = shotInterval;
        
        direction = (destPos - Controller.anchoredPosition).normalized;
        healthPoint = hp;

        onRelease = release;
    }
    public bool Move()
    {
        var dist = Vector2.Distance(Controller.anchoredPosition, destinationPos);
        // Debug.Log($"dist = {dist}");
        if (dist <= 0.5f)
        {
            // Debug.Log($"dist returndist returndist returndist returndist returndist return");
            return false;
        }
        newPos.x += direction.x * flySpeed;
        newPos.y += direction.y * flySpeed;
        Controller.anchoredPosition = newPos;
        return true;
    }

    public bool Fire()
    {
        if (cur_shot_invertval <= 0)
        {
            cur_shot_invertval = shot_invertal;
            return true;
        }

        cur_shot_invertval -= Time.deltaTime;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tag))
        {
            return;
        }
        // Debug.Log($"EnemyAirPlane::OnTriggerEnter2D other.tag = {other.tag}");

        healthPoint -= 1;
        if (healthPoint <= 0)
        {
            onRelease.Invoke(this);
        }
    }
}
