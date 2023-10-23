using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flavor : MonoBehaviour
{
    public cfg.food.qteTag Tag;

    public bool IsEnable => _outlineControl.Outline;

    private OutlineControl _outlineControl;
    // Start is called before the first frame update
    void Start()
    {
        _outlineControl = GetComponent<OutlineControl>();
    }

    public void EnableTag()
    {
        _outlineControl.ShowOutline();
    }

    public void DisableTag()
    {
        _outlineControl.HideOutline();
    }

}
