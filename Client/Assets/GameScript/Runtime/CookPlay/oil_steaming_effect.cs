using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class oil_steaming_effect : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private DOTweenAnimation[] _animations;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    public async void PlayEffect(List<oil_steaming_effect> pool)
    {
        if (_animations == null)
        {
            _animations = transform.GetComponents<DOTweenAnimation>();    
        }
        gameObject.SetActive(true);
        foreach (var one in _animations)
        {
            one.DORestart();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        gameObject.SetActive(false);
        pool.Add(this);
        Debug.Log($"show_oil_steaming recycle = {pool.Count}");
    }

    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        _sprite.color = Color.white;
    }
}
