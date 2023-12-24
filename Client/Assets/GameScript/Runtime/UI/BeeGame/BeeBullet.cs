using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BeeBullet : MonoBehaviour
{
    private  RectTransform BRectTransform;
    public Vector2 CurPos => BRectTransform.anchoredPosition;
    private Vector2 Direction;
    
    
    private Vector2 newPos;
    private float speed;

    private Action<BeeBullet> _recycle;

    public void Init(Vector2 shotPos,string shotTag,Vector2 dir,float sped,Action<BeeBullet> recycleCB)
    {
        if (BRectTransform == null)
            BRectTransform = GetComponent<RectTransform>();
        newPos = BRectTransform.anchoredPosition = shotPos;
        gameObject.tag = shotTag;
        Direction = dir;
        speed = sped;
        _recycle = recycleCB;
    }
    
    public void Move()
    {
        newPos.y = BRectTransform.anchoredPosition.y + Direction.y+speed*Time.deltaTime;
        BRectTransform.anchoredPosition = newPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tag))
        {
            _recycle?.Invoke(this);
            _recycle = null;
        }
        
    }
    
    
}
