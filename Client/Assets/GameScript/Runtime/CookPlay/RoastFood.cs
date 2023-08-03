using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameScript.CookPlay;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;


public class RoastFood : MonoBehaviour
{
    [HideInInspector]
    public BarbecueModule Module;
    public RoastFoodData FoodData;
    public Image frontProgressImage;
    public Image frontOverProgressImage;
    public Image backProgressImage;
    public Image backOverProgressImage;
    public List<SpriteRenderer> foodSpriteList;
    public Canvas progressCanvas;
    public bool inBox { get; private set; }
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
    // void Start()
    // {
    //     Init();
    // }

    public void Init(string resPath,Camera camera)
    {
        _collider2D = GetComponent<BoxCollider2D>();
        _curBounds = new BoundsInt();
        _leftBottom = new Vector2();
        _rightTop = new Vector2();
        _mainCamera = camera;
        _isDrag = false;

        progressCanvas.worldCamera = _mainCamera;
        
        var size = _collider2D.size;
        var offset = _collider2D.offset;
        _objectSize = new Vector2(
            size.x + offset.x,
            size.y + offset.y);
        _halfSize = new Vector2(_objectSize.x / 2f, _objectSize.y / 2f);
        _heatValue = new Vector2(0, 0);
        _currentFace = true;
        
        LoadFoodSprite(resPath);
    }
    
    public void Destroy()
    {
        foreach (var foodSp in foodSpriteList)
        {
            foodSp.sprite = null;
            foodSp.gameObject.SetActive(false);
        }
    }

    private async void LoadFoodSprite(string resPath)
    {
        var handle = YooAssets.LoadAssetAsync<Sprite>(resPath);
        await handle.ToUniTask(this);
        var  sp = handle.AssetObject as Sprite;
        foreach (var foodSp in foodSpriteList)
        {
            foodSp.sprite = sp;
            foodSp.gameObject.SetActive(true);
        }
    }

    public void StartRoast(CompositeDisposable handler)
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(CheckHit).AddTo(handler);
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0) && _isDrag)
            .Subscribe(MoveFood).AddTo(handler);
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0) && _isDrag) 
            .Subscribe(PutFood).AddTo(handler);
        this.UpdateAsObservable()
            .Where(_=>Input.GetMouseButtonDown(1))
            .Subscribe(CheckFlip).AddTo(handler);
    }
    

    private void CalculateBoxPosition()
    {
        var position = transform.position;
        
        _leftBottom.x = position.x - _halfSize.x;
        _leftBottom.y = position.y - _halfSize.y;
        
        _rightTop.x = position.x + _halfSize.x;
        _rightTop.y = position.y + _halfSize.y;
    }
    
    private void MoveFood(Unit param)
    {
        // Debug.Log($"{name} is Moving");
        var pos = transform.position;
        Vector3 camToObjDir = pos - _mainCamera.transform.position;
        Vector3 normalDir = Vector3.Project(camToObjDir, _mainCamera.transform.forward);
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, normalDir.magnitude));
        Vector3 newPos = worldPos + _dragOffset;
        newPos.z = pos.z;
        
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

    private void PutFood(Unit param)
    {
        Module.ClearArea(_curBounds);
        CalculateBoxPosition();
        bool isEmpty = Module.CheckAreaEmpty(_curBounds);
        bool isEntire = Module.EntireInArea(_leftBottom, _rightTop);
        // Debug.Log($"{name} isEmpty = {isEmpty} isEntire = {isEntire}");
        if ( isEmpty && isEntire )
        {
            Module.LockArea(_curBounds,this);
            inBox = true;
        }
        else
        {
            //原来的位置是否在烧烤区域内
            inBox = Module.RoastArea.bounds.Contains(_oldPosition);
            if (inBox)
            {
                //还原原来的位置
                Module.LockArea(_oldBounds,this);
            }
            _curBounds = _oldBounds;
            transform.position = _oldPosition;
        }
        _isDrag = false;
    }
    
    private void CheckHit(Unit param)
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

    private void CheckFlip(Unit param)
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit && hit.collider.transform == transform)
        {
            // Debug.Log($"{name} CheckFlip");
            FlipOver();
        }
    }

    public void AddHeat(float heat)
    {
        int index = _currentFace ? 1: 0;
        
        _heatValue[index] += heat;
        _heatValue[index] = Mathf.Clamp(_heatValue[index], 0,FoodData.MaxHeatCapacity);

        // var progressImg = _currentFace ? frontProgressImage : backProgressImage;
        Image progressImg = null;
        if (_currentFace)
        {
            progressImg = _heatValue[index] < FoodData.CookedHeatCapacity ? frontProgressImage : frontOverProgressImage;
        }
        else
        {
            progressImg = _heatValue[index] < FoodData.CookedHeatCapacity ? backProgressImage : backOverProgressImage;
        }

        if (_heatValue[index] < FoodData.CookedHeatCapacity)
        {
            float progress = _heatValue[index] / FoodData.CookedHeatCapacity;
            progress = Mathf.Clamp01(progress);
            progressImg.color = FoodData.CookingProgressColor;
            progressImg.fillAmount = progress;
        }
        else
        {
            var overheatProgress = (_heatValue[index] - FoodData.CookedHeatCapacity) / 
                                   (FoodData.MaxHeatCapacity - FoodData.CookedHeatCapacity);
            overheatProgress = Mathf.Clamp01(overheatProgress);
            progressImg.color = FoodData.OverCookedProgressColor;
            progressImg.fillAmount = overheatProgress;
        }
    }
    private void FlipOver()
    {
        _currentFace = !_currentFace;
    }

    public bool IsComplete()
    {
        return _heatValue[0] >= FoodData.CookedHeatCapacity
            && _heatValue[1] >= FoodData.CookedHeatCapacity;
    }

    public void Reset()
    {
        _currentFace = true;
        inBox = false;
        _heatValue = Vector2.zero;
        frontProgressImage.fillAmount = 0;
        frontOverProgressImage.fillAmount = 0;
        backProgressImage.fillAmount = 0;
        backOverProgressImage.fillAmount = 0;
    }

    
}
