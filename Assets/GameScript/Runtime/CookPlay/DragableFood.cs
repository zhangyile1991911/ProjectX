using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragableFood : MonoBehaviour
{
    private Camera _mainCamera;
    private bool _isDrag;
    private Vector3 _dragOffset;

    public Action<DragableFood> Click;
    public Action<DragableFood> Drag;
    public Action<DragableFood> Put;
    // Start is called before the first frame update
    public void Init()
    {
        _mainCamera = Camera.main;
    }

    public void Begin(CompositeDisposable handler)
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0)&& !_isDrag)
            .Subscribe(CheckHit)
            .AddTo(handler);
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0) && _isDrag)
            .Subscribe(MoveFood)
            .AddTo(handler);
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0) && _isDrag)
            .Subscribe(PutFood)
            .AddTo(handler);
    }

    private void CheckHit(Unit param)
    {
        Vector2 origin = Vector2.zero;
        Vector2 direction = Vector2.zero;
        if (_mainCamera.orthographic)
        {
            origin = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            origin = ray.origin;
            direction = ray.direction;
        }

        // Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction);
        if (hit.collider != null)
        {
            if (hit.collider.transform == transform)
            {
                var position = transform.position;
                var mainCameraTransform = _mainCamera.transform;
                Vector3 camToObjDir = position - mainCameraTransform.position;
                Vector3 normalDir = Vector3.Project(camToObjDir, mainCameraTransform.forward);
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, normalDir.magnitude));
                _dragOffset = position - worldPos;
                _isDrag = true;
                Click?.Invoke(this);
            }
        }
    }

    private void MoveFood(Unit param)
    {
        // Debug.Log($"{name} MoveFood is Moving");
        Vector3 camToObjDir = transform.position - _mainCamera.transform.position;
        Vector3 normalDir = Vector3.Project(camToObjDir, _mainCamera.transform.forward);
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, normalDir.magnitude));
        Vector3 newPos = worldPos + _dragOffset;
        newPos.z = 0;
        transform.position = newPos;
        //_dragTopic?.OnNext(Unit.Default);
        Drag?.Invoke(this);
    }
    
    private void PutFood(Unit param)
    {
        _isDrag = false;
        //_putTopic?.OnNext(Unit.Default);
        Put.Invoke(this);
    }

}
