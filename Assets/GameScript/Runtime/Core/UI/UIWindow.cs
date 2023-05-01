using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIWindow : IUIBase
{
    public GameObject uiGo
    {
        get => _uiGo;
        set => _uiGo = value;
    }
    public Transform uiTran
    {
        get => _uiGo.transform;
    }
    private GameObject _uiGo;

    public UILayer uiLayer
    {
        get => _uiLayer;
        set => _uiLayer = value;
    }
    
    public bool IsActive => uiGo.active;

    protected CompositeDisposable handles = new CompositeDisposable(10);
    private UILayer _uiLayer;
    public virtual void Init(GameObject go)
    {
        
    }

    public virtual void OnCreate()
    {

    }

    public virtual void OnDestroy()
    {
        handles.Clear();
        GameObject.DestroyImmediate(uiGo);
    }

    public virtual void OnShow(UIOpenParam openParam)
    {
        uiGo.SetActive(true);
    }

    public virtual void OnHide()
    {
        uiGo.SetActive(false);
        handles.Clear();
    }

    public virtual void OnUpdate()
    {
        
    }
}
