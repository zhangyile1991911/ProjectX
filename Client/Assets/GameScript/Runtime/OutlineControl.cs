using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineControl : MonoBehaviour
{
    private SpriteRenderer sr;
    public bool Outline => isOutline;

    private bool isOutline;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        isOutline = false;
    }

    public void ShowOutline()
    {
        if (isOutline) return;
        var m = sr.material;
        m.EnableKeyword("_SHOWOUTLINE");
        sr.material = m;
        
        isOutline = true;

    }

    public void HideOutline()
    {
        if (!isOutline) return;
        var m = sr.material;
        m.DisableKeyword("_SHOWOUTLINE");
        sr.material = m;
        isOutline = false;
    }
}
