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
    public override void OnCreate()
    {
        _dragableComp = Img_Icon.GetComponent<UIDragHandler>();
        _canDrag = new ReactiveProperty<bool>(false);
        _dragableComp.Init(UIManager.Instance.RootCanvas,_canDrag);
    }
    
    public override void OnDestroy()
    {
        
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }

    public void ShowFoodIcon(int menuId)
    {
        var tbItem = DataProviderModule.Instance.GetItemBaseInfo(menuId);
        ParentWindow.LoadSpriteAsync(tbItem.UiResPath,Img_Icon);
        _canDrag.Value = true;
    }

    public void ClearFoodIcon()
    {
        Img_Icon.sprite = null;
        _canDrag.Value = false;
    }
    
}