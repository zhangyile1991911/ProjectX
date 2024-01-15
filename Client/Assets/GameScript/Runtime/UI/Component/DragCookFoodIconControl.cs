using System;
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
    private RaycastHit[] _raycastHits;
    public Action<CookResult,int> GiveAction;
    public override void OnCreate()
    {
        _dragableComp = Img_Icon.GetComponent<UIDragHandler>();
        _canDrag = new ReactiveProperty<bool>(false);
        _dragableComp.Init(UIManager.Instance.RootCanvas,_canDrag);
        _mainCamera = UIManager.Instance.UICamera;
    }
    
    public override void OnDestroy()
    {
        GiveAction = null;
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
        _dragableComp.OnBeginDragCB -= ListenBeginDrag;
        _dragableComp.OnDragCB -= ListenDrag;
        _dragableComp.OnEndDragCB -= ListenEndDrag;
    }

    public override void OnUpdate()
    {
        
    }

    public void ShowFoodIcon(CookResult food,Action<CookResult,int> cb)
    {
        _cookResult = food;
        var tbItem = DataProviderModule.Instance.GetItemBaseInfo(_cookResult.MenuId);
        ParentWindow.LoadSpriteAsync(tbItem.UiResPath,Img_Icon);
        _canDrag.Value = true;
        GiveAction = cb;
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
        // Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawRay(ray.origin,ray.direction*100f);
    }
    
    private void ListenEndDrag()
    {
        if (_cookResult == null)
        {
            return;
        }
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawRay(ray.origin,ray.direction*100f);
        // RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction,999f);
        _raycastHits ??= new RaycastHit[2];
        Physics.RaycastNonAlloc(ray, _raycastHits);
        //todo 优化成noalloc版本
        // RaycastHit hit;
        // Physics.Raycast(ray, out hit);
        foreach (var hit in _raycastHits)
        {
            if (hit.transform != null && hit.transform.CompareTag("RestaurantCharacter"))
            {
                var character = hit.transform.GetComponent<RestaurantCharacter>();
                if (!character.IsWaitForOrder())
                {
                    Debug.Log($"当前角色{character.CharacterName} 没有进入等餐状态");
                    return;
                }
                character.ReceiveFood(_cookResult);
                GiveAction?.Invoke(_cookResult,character.CharacterId);
                ClearFoodIcon();
                break;
            }
        }
        
    }
    
}