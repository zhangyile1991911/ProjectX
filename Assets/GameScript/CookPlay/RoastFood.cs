using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using GameScript.CookPlay;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class RoastFood : MonoBehaviour
{
    public BarbecueModule Module;
    public RoastFoodData FoodData;
    public Image frontProgressImage;
    public Image backProgressImage;
    
    private BoxCollider2D _collider2D;

    private BoundsInt _curBounds;

    private Vector3 _dragOffset;

    private bool _isDrag;

    private Camera _mainCamera;

    private Vector2 _leftBottom;

    private Vector2 _rightTop;

    private Vector2 _halfSize;

    private bool _currentFace;

    private Vector2 _heatValue;

    private Vector2 _oldPosition;

    private BoundsInt _oldBounds;

    private Vector2 _objectSize;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        _collider2D = GetComponent<BoxCollider2D>();
        _curBounds = new BoundsInt();
        _leftBottom = new Vector2();
        _rightTop = new Vector2();
        _mainCamera = Camera.main;
        _isDrag = false;

        var size = _collider2D.size;
        var offset = _collider2D.offset;
        _objectSize = new Vector2(
            size.x + offset.x,
            size.y + offset.y);
        _halfSize = new Vector2(_objectSize.x / 2f, _objectSize.y / 2f);
        _heatValue = new Vector2(0, 0);
        _currentFace = true;
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(CheckHit);
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButton(0) && _isDrag)
            .Subscribe(MoveFood);
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0) && _isDrag)
            .Subscribe(PutFood);
        Observable.EveryUpdate()
            .Where(_=>Input.GetMouseButtonDown(1))
            .Subscribe(CheckFlip);
        // HeatTopic.Subscribe(AddHeat);
    }

    private void CalculateBoxPosition()
    {
        var position = transform.position;
        
        _leftBottom.x = position.x - _halfSize.x;
        _leftBottom.y = position.y - _halfSize.y;
        
        _rightTop.x = position.x + _halfSize.x;
        _rightTop.y = position.y + _halfSize.y;
    }
    
    private void MoveFood(long param)
    {
        Debug.Log($"{name} is Moving");
        Vector3 camToObjDir = transform.position - _mainCamera.transform.position;
        Vector3 normalDir = Vector3.Project(camToObjDir, _mainCamera.transform.forward);
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, normalDir.magnitude));
        Vector3 newPos = worldPos + _dragOffset;
        newPos.z = 0;

        transform.position = newPos;

        Module.ClearArea(_curBounds);

        CalculateBoxPosition();
        bool isEntire = Module.EntireInArea(_leftBottom, _rightTop);
        // Debug.Log($"isEntire = {isEntire}");
        if (isEntire)
        {
             var cellWorldPosition= Module.SnapCoordinateToGrid(newPos);
             transform.position = cellWorldPosition;
             _curBounds.position = Module.SnapWorldPositionInGrid(new Vector3(
                cellWorldPosition.x - _halfSize.x,
                cellWorldPosition.y - _halfSize.y,
                0));
            // Debug.Log($"collider.bounds.size.x = {collider.bounds.size.x}");
            _curBounds.size = new Vector3Int(Mathf.CeilToInt(_objectSize.x),Mathf.CeilToInt(_objectSize.y),1);
            Module.FillArea(_curBounds);
        }
        else
        {
            transform.position = newPos;    
        }
    }

    private void PutFood(long param)
    {
        Module.ClearArea(_curBounds);
        CalculateBoxPosition();
        bool isEmpty = Module.CheckAreaEmpty(_curBounds);
        bool isEntire = Module.EntireInArea(_leftBottom, _rightTop);
        Debug.Log($"{name} isEmpty = {isEmpty} isEntire = {isEntire}");
        if ( isEmpty && isEntire )
        {
            Module.LockArea(_curBounds,this);
        }
        else
        {
            //原来的位置是否在烧烤区域内
            bool isInBox = Module.RoastArea.bounds.Contains(_oldPosition);
            if (isInBox)
            {
                //还原原来的位置
                Module.LockArea(_oldBounds,this);
            }
            _curBounds = _oldBounds;
            transform.position = _oldPosition;
        }
        _isDrag = false;
    }
    
    private void CheckHit(long param)
    {
        RaycastHit2D hit;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (hit = Physics2D.Raycast(ray.origin, ray.direction))
        {
            if (hit.collider.transform == transform)
            {
                var position = transform.position;
                var mainCameraTransform = _mainCamera.transform;
                Vector3 camToObjDir = position - mainCameraTransform.position;
                Vector3 normalDir = Vector3.Project(camToObjDir, mainCameraTransform.forward);
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, normalDir.magnitude));
                Module.UnlockArea(_curBounds,this);
                _dragOffset = position - worldPos;
                _oldPosition = position;
                _oldBounds = _curBounds;
                _isDrag = true;
            }
        }
    }

    private void CheckFlip(long param)
    {
        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit && hit.collider.transform == transform)
        {
            Debug.Log($"{name} CheckFlip");
            TurnOver();
        }
    }

    public void AddHeat(float heat)
    {
        int index = _currentFace ? 1: 0;
        
        _heatValue[index] += heat;
        _heatValue[index] = Mathf.Clamp(_heatValue[index], 0,FoodData.SpecificHeatCapacity);

        var progressImg = _currentFace ? frontProgressImage : backProgressImage;

        if (_heatValue[index] < FoodData.MediumHeatCapactiy)
        {
            float progress = _heatValue[index] / FoodData.CookedHeatCapacity;
            progress = Mathf.Clamp01(progress);
            progressImg.color = FoodData.CookingProgressColor;
            progressImg.fillAmount = progress;
                
        }
        else
        {
            var overheatProgress = (_heatValue[index] - FoodData.MediumHeatCapactiy) / 
                                   (FoodData.SpecificHeatCapacity - FoodData.MediumHeatCapactiy);
            overheatProgress = Mathf.Clamp01(overheatProgress);
            progressImg.color = FoodData.OverCookedProgressColor;
            progressImg.fillAmount = overheatProgress;
        }
    }
    private void TurnOver()
    {
        _currentFace = !_currentFace;
    }
    
}
