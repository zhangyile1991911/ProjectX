using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAirPlane : MonoBehaviour
{
    public RectTransform AirPlaneRectTransform => controller;
    private RectTransform controller;
    
    private int hp;
    private Vector2 direction;
    private float speed;

    private Vector2 newPos;
    private Vector2 destPos;

    private Animator _animator;
    private float shot_invertal;
    public Action GameOver;

    public void Reset()
    {
         newPos = Vector3.zero;
         destPos = Vector3.zero;
         speed = 0.5f;
         shot_invertal = 0.5f;
         hp = 3;
    }
    public void Move()
    {
        var dist = Vector2.Distance(controller.anchoredPosition, destPos);
        if (dist <= 0.5f)
        {
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
            shot_invertal = 0.5f;
            return true;
        }

        shot_invertal -= Time.deltaTime;
        return false;
    }
    
    // Start is called before the first frame update
    public void Init()
    {
        controller = GetComponent<RectTransform>();
        _animator = GetComponent<Animator>();
        Reset();
    }

    public void Show()
    {
        _animator.enabled = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _animator.enabled = false;
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"PlayerAirPlane OnTriggerEnter2D {other.tag}");
        // if (other.CompareTag(tag))
        // {
        //     return;
        // }
        // // Debug.Log($"PlayerAirPlane::OnTriggerEnter2D other.name = {other.name}");
        // hp -= 1;
        // if (hp <= 0)
        // {
        //     GameOver.Invoke();
        // }
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     Debug.Log("PlayerAirPlane OnCollisionEnter2D");
    //     // if (other.CompareTag(tag))
    //     // {
    //     //     return;
    //     // }
    //     // // Debug.Log($"PlayerAirPlane::OnTriggerEnter2D other.name = {other.name}");
    //     // hp -= 1;
    //     // if (hp <= 0)
    //     // {
    //     //     GameOver.Invoke();
    //     // }
    // }
}
