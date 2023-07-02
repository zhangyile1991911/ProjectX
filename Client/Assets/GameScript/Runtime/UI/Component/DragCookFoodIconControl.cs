using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class DragCookFoodIcon : UIComponent
{
    public DragCookFoodIcon(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }

    private UIDragHandler _dragableComp;
    private ReactiveProperty<bool> _canDrag;
    private Camera _mainCamera;
    private CookResult _cookResult;
    public override void OnCreate()
    {
        _dragableComp = Img_Icon.GetComponent<UIDragHandler>();
        _canDrag = new ReactiveProperty<bool>(false);
        _dragableComp.Init(UIManager.Instance.RootCanvas,_canDrag);
        _mainCamera = Camera.main;
    }
    
    public override void OnDestroy()
    {
        
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        _dragableComp.OnBeginDragCB += ListenBeginDrag;
        _dragableComp.OnDragCB += ListenDrag;
        _dragableComp.OnEndDragCB += ListenEndDrag;
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }

    public void ShowFoodIcon(CookResult food)
    {
        _cookResult = food;
        var tbItem = DataProviderModule.Instance.GetItemBaseInfo(_cookResult.menuId);
        ParentWindow.LoadSpriteAsync(tbItem.UiResPath,Img_Icon);
        _canDrag.Value = true;
    }

    public void ClearFoodIcon()
    {
        Img_Icon.sprite = null;
        _canDrag.Value = false;
        _cookResult = null;
    }

    private void ListenBeginDrag()
    {
        
    }
    
    private void ListenDrag()
    {
        
    }
    
    private void ListenEndDrag()
    {
        if (_cookResult == null)
        {
            return;
        }
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit && hit.transform.CompareTag("RestaurantCharacter"))
        {
            var character = hit.transform.GetComponent<RestaurantCharacter>();
            character.ReceiveFood(_cookResult);
            ClearFoodIcon();
        }
    }
    
}