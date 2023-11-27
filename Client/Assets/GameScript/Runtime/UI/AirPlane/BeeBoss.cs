using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeBoss : MonoBehaviour
{
    public RectTransform Controller;
    public RectTransform Shot;
    
    private Vector2 newPos;
    private Vector2 destinationPos;
    
    public float FlySpeed => flySpeed;
    private float flySpeed;
    
    private float shot_invertal;
    private float cur_shot_invertval;

    public bool IsAlive => healthPoint > 0;
    public Vector2 Direction => direction;
    private int healthPoint;
    private Vector2 direction;
    
    public void ConfigProperty(Vector2 startPos,Vector2 destPos,int hp,float speed = 0.7f,float shotInterval = 2.5f)
    {
        newPos = startPos;
        destinationPos = destPos;
        
        Controller.anchoredPosition = startPos;
        flySpeed = speed;
        cur_shot_invertval = shot_invertal = shotInterval;
        
        direction = (destPos - Controller.anchoredPosition).normalized;
        healthPoint = hp;
    }
    
    public bool Move()
    {
        var dist = Vector2.Distance(Controller.anchoredPosition, destinationPos);
        if (dist <= 0.5f)
        {
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
}
