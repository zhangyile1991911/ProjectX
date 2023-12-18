using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
public class AnimatorAutoRecycle : MonoBehaviour
{
    private Animator _animator;
    private ObjectPool<AnimatorAutoRecycle> _pool;
    public void StartAnimation(string AnimationName,ObjectPool<AnimatorAutoRecycle> objectPool)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        
        _animator.enabled = true;
        _animator.Play(AnimationName);
        _pool = objectPool;
    }

    private void Update()
    {
        var casi = _animator.GetCurrentAnimatorStateInfo(0);
        if (casi.normalizedTime >= 1.0f)
        {
            _animator.enabled = false;
            _pool.Release(this);
        }
    }
}
