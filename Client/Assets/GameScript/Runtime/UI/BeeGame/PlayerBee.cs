using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBee : MonoBehaviour
{
    public RectTransform AirPlaneRectTransform => controller;
    private RectTransform controller;

    public bool IsAlive => hp.Value > 0;
    public  IOptimizedObservable<int> HPTopic => hp;
    private IntReactiveProperty hp;
    private Vector2 direction;
    private float speed;

    private Vector2 newPos;
    private Vector2 destPos;

    private Animator _animator;
    private Image _avatar;
    private float shot_invertal;
    
    public void Reset()
    {
         newPos = Vector3.zero;
         destPos = Vector3.zero;
         speed = 0.5f;
         shot_invertal = 0.8f;
         hp.Value = 3;
    }
    public bool Move()
    {
        var dist = Vector2.Distance(controller.anchoredPosition, destPos);
        if (dist <= 0.5f)
        {
            return false;
        }
        newPos.x += direction.x * speed;
        newPos.y += direction.y * speed;
        controller.anchoredPosition = newPos;
        return true;
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
            shot_invertal = 0.8f;
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
        _avatar = GetComponent<Image>();
        hp = new IntReactiveProperty(3);
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

    private readonly Color transparent = new Color(255,255,255,0);
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tag))
        {
            _avatar.DOColor(transparent, 0.2f).SetLoops(2, LoopType.Yoyo);
            // Debug.Log("BeeBoss OnTriggerEnter2D");
            hp.Value -= 1;
        }
    }

}
