using UnityEngine;

public enum UILayer
{
    Bottom,
    Center,
    Top,
    Guide,
}

public interface IUIBase
{
    GameObject uiGo { get; set; }
    
    void Init(GameObject go);
    
    UILayer GetLayer();
    
    void OnCreate();

    void OnDestroy();
    
    void OnShow(UIOpenParam openParam);

    void OnHide();
    
    void OnUpdate();
}
