using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance => _instance;
    private static UIManager _instance;

    private LRUCache<UIEnum, IUIBase> _uiCachedDic;

    private Transform _bottom;

    private Transform _center;

    private Transform _top;

    private Transform _guide;

    private void Awake()
    {
        _uiCachedDic = new LRUCache<UIEnum, IUIBase>(10);
        _uiCachedDic.OnRemove += (ui) =>
        {
            ui.OnHide();
            ui.OnDestroy();
        };
        // uiStack = new Stack<IUIBase>();
        // uiList = new List<IUIBase>();
        _bottom = transform.Find("Bottom");
        _center = transform.Find("Center");
        _top = transform.Find("Top");
        _guide = transform.Find("Guide");
        
        _instance = this;
    }

    public IUIBase Get(UIEnum uiName)
    {
        IUIBase ui = null;
        if (!_uiCachedDic.TryGetValue(uiName, out ui))
        {
            return null;
        }

        return ui;
    }

    private void OnOpenUI(IUIBase ui,Action<IUIBase> onComplete,UIOpenParam openParam,UILayer layer)
    {
        ui.OnShow(openParam);
        onComplete?.Invoke(ui);
    }
    
    public void OpenUI(UIEnum uiName,Action<IUIBase> onComplete,UIOpenParam openParam,UILayer layer = UILayer.Bottom)
    {
        IUIBase ui = null;
        if (_uiCachedDic.TryGetValue(uiName, out ui))
        {
            OnOpenUI(ui, onComplete, openParam,layer);
        }
        else
        {
            LoadUI(uiName,(loadUi)=>
            {
                loadUi.OnCreate();
                OnOpenUI(loadUi,onComplete,openParam,layer);
            },layer);
        }
    }

    public void CloseUI(UIEnum uiName)
    {
        IUIBase ui = null;
        if (_uiCachedDic.TryGetValue(uiName, out ui))
        {
            ui.OnHide();
        }
    }

    private Transform getParentNode(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Bottom:
                return _bottom;
            case UILayer.Center:
                return _center;
            case UILayer.Top:
                return _top;
            case UILayer.Guide:
                return _guide;
        }

        return null;
    }
    private void LoadUI(UIEnum uiName, Action<IUIBase> onComplete,UILayer layer)
    {
        Type uiType = Type.GetType(uiName.ToString());
        var attributes = uiType.GetCustomAttributes(false);
        var uiPath = attributes
            .Where(one => one is UIAttribute)
            .Select(tmp=> (tmp as UIAttribute).ResPath).FirstOrDefault();
        
        var handle = YooAssets.LoadAssetAsync<GameObject>(uiPath);
        handle.Completed += (result) =>
        {
            var uiPrefab = result.AssetObject;
            var parentNode = getParentNode(layer);
            var uiGameObject = GameObject.Instantiate(uiPrefab,parentNode) as GameObject;
            uiGameObject.transform.localScale = Vector3.one;
            uiGameObject.transform.SetParent(parentNode,false);
            IUIBase ui = Activator.CreateInstance(uiType) as IUIBase;
            ui.uiLayer = layer;
            ui.Init(uiGameObject);
            onComplete?.Invoke(ui);
            _uiCachedDic.Add(uiName,ui);
        };
    }
    
    public T CreateUIComponent<T>(UIOpenParam openParam,Transform node,UIWindow parent)where T : UIComponent
    {
        var componentType = typeof(T);
        
        var attributes = componentType.GetCustomAttributes(false);
        var uiPath = attributes
            .Where(one => one is UIAttribute)
            .Select(tmp=> (tmp as UIAttribute).ResPath).FirstOrDefault();
        
        var handle = YooAssets.LoadAssetSync<GameObject>(uiPath);
        // await handle.Task;
        var uiPrefab = handle.AssetObject;
        
        var uiGameObject = GameObject.Instantiate(uiPrefab,node) as GameObject;
        uiGameObject.transform.localPosition = Vector3.zero;
        uiGameObject.transform.localScale = Vector3.one;
        T uiComponent = Activator.CreateInstance(componentType,new object[]{uiGameObject,parent}) as T;
        uiComponent.OnCreate();
        uiComponent.OnShow(openParam);
        return uiComponent;
    }
}
