using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineControl : MonoBehaviour
{
    public int UserDataId;
    
    [SerializeField]public float Thickness = 0.2f;
    private SpriteRenderer sr;
    public bool Outline => isOutline;

    private bool isOutline;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        isOutline = false;
        var m = sr.material;
        m.SetFloat("_Float",0);
    }

    public void ShowOutline()
    {
        if (isOutline) return;
        var m = sr.material;
        m.EnableKeyword("_SHOWOUTLINE");
        m.SetFloat("_Float",Thickness);
        sr.material = m;
        
        isOutline = true;

    }

    public void HideOutline()
    {
        if (!isOutline) return;
        var m = sr.material;
        m.DisableKeyword("_SHOWOUTLINE");
        m.SetFloat("_Float",0);
        sr.material = m;
        isOutline = false;
    }


    public void Disappear(float duration)
    {
        sr.DOFade(0, duration);
    }
}
