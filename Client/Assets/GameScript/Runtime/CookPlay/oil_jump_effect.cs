using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class oil_jump_effect : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Sequence _sequence;
    private Tweener _fade;
    private Tweener _rotate;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public async void PlayEffect(List<oil_jump_effect> pools)
    {
        if (_sequence == null)
        {
            var dest = new Vector3();
            dest.x = Random.Range(5f, 20f);
            dest.x = pools.Count % 2 == 0 ? dest.x : 0-dest.x; 
            // dest.y = 4f + Random.Range(5f, 10f);
            _sequence = transform.DOJump(dest, Random.Range(3f,7f), 1, 0.5f).SetAutoKill(false);
            _fade = _spriteRenderer.DOFade(0, 0.45f).SetAutoKill(false);
            _rotate = transform.DOLocalRotate(new Vector3(0, 360f, 0), 0.5f, RotateMode.LocalAxisAdd).SetAutoKill(false);
        }
        else
        {
            _spriteRenderer.color = Color.white;
            transform.localRotation = Quaternion.identity;
            _sequence.Restart();
            _fade.Restart();
            _rotate.Restart(); 
        }
        gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        gameObject.SetActive(false);
        pools.Add(this);
    }
}
