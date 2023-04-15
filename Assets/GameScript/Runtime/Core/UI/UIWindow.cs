using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : IUIBase
{
    public GameObject uiGo
    {
        get => _uiGo;
        set => _uiGo = value;
    }
    private GameObject _uiGo;
    
    public virtual void Init(GameObject go)
    {
        
    }

    public virtual UILayer GetLayer()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnCreate()
    {

    }

    public virtual void OnDestroy()
    {
        
    }

    public virtual void OnShow(UIOpenParam openParam)
    {
        uiGo.SetActive(true);
    }

    public virtual void OnHide()
    {
        uiGo.SetActive(false);
    }

    public virtual void OnUpdate()
    {
        
    }
}
