using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    public Action OnBeginDragCB;
    public Action OnDragCB;
    public Action OnEndDragCB;
    
    private RectTransform rectTransform;
    private Canvas canvas;

    private Vector2 origin_pos;
    private IReactiveProperty<bool> _canDrag;
    public void Init(Canvas root,IReactiveProperty<bool> drag)
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = root;
        _canDrag = drag;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_canDrag.Value)
        {
            origin_pos = rectTransform.anchoredPosition;
            OnBeginDragCB?.Invoke();
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (_canDrag.Value)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            OnDragCB?.Invoke();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_canDrag.Value)
        {
            // 处理拖拽释放事件，例如更新位置或触发相应的操作
            Debug.Log("Drag released!");
            rectTransform.anchoredPosition = origin_pos;
            OnEndDragCB?.Invoke();
        }
    }
}
