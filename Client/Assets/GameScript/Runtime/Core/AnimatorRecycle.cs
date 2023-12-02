using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
public class AnimatorRecycle : MonoBehaviour
{
    private Animator _animator;

    public void StartAnimation(string AnimationName,ObjectPool<AnimatorRecycle> objectPool)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.Play(AnimationName);
        var casi = _animator.GetCurrentAnimatorStateInfo(0);
        if (casi.normalizedTime >= 1.0f)
        {
            objectPool.Release(this);
        }
    }
}
