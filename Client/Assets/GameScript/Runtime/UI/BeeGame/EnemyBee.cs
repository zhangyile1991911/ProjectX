using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBee : MonoBehaviour
{
    public RectTransform Controller;
    public RectTransform Shot;
    public Vector2 CurPos => newPos;
    public Vector2 Direction => direction;
    private int healthPoint;
    private Vector2 direction;
    public float FlySpeed => flySpeed;
    private float flySpeed;

    private Vector2 newPos;
    private Vector2 destinationPos;
    
    private float shot_invertal;
    private float cur_shot_invertval;
    private Image _avatar;
    private Action<EnemyBee> onRelease;
    public void ConfigProperty(Action<EnemyBee> release,Vector2 startPos,Vector2 destPos,int hp,float speed = 0.7f,float shotInterval = 2.5f)
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

    private readonly Color transparent = new Color(255,255,255,0);
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log($"EnemyBee::OnTriggerEnter2D other.tag = {other.tag}");
        
        if (other.CompareTag(tag))
        {
            return;
        }
        
        healthPoint -= 1;
        if (healthPoint <= 0)
        {
            onRelease.Invoke(this);
        }
        else
        {
            _avatar.DOColor(transparent, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     Debug.Log("EnemyAirPlane OnCollisionEnter2D");
    // }

}
