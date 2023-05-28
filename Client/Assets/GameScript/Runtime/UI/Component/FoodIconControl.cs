using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class FoodIcon : UIComponent
{
    public int FoodId { get; private set; }
    
    public FoodIcon(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        Btn_Food.onClick.AddListener(ClearMaterialInfo);
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

    public void FoodMaterialInfo(int materialId)
    {
        var m = UniModule.GetModule<DataProviderModule>();
        var tb = m.GetItemBaseInfo(materialId);
        
        ParentWindow.LoadSpriteAsync(tb.UiResPath, handle =>
        {
            Img_Food.sprite = handle;
            Btn_Food.interactable = true;    
        });

        FoodId = materialId;
        Txt_name.text = tb.Name;
        Txt_name.gameObject.SetActive(true);
    }

    public void ClearMaterialInfo()
    {
        FoodId = 0;
        Img_Food.sprite = null;
        Btn_Food.interactable = false;
        Txt_name.text = null;
        Txt_name.gameObject.SetActive(false);
    }
}