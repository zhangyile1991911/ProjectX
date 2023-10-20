using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class XButton : Button
{
    public IObservable<PointerEventData> OnClick => _onClick;
    private readonly Subject<PointerEventData> _onClick;

    protected XButton()
    {
        _onClick = new Subject<PointerEventData>();
    }
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        _onClick.OnNext(eventData);
    }
}
