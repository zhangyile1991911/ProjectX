using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class OptionalReceipt : UIComponent
{
    private List<Image> _materials;
    public Action<int,bool> StartCook;
    public XButton BgButton;
    public bool IsFull { get; private set; }
    public int ReceiptId { get; private set; }
    
    public OptionalReceipt(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        base.OnCreate();
        _materials = ListPool<Image>.Get();
        BgButton = uiGo.GetComponent<XButton>();
        BgButton.OnClick.Subscribe(OnClick).AddTo(uiGo);
    }
    
    public override void OnDestroy()
    {
        ListPool<Image>.Release(_materials);
        base.OnDestroy();
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

    private void OnClick(PointerEventData eventData)
    {
        StartCook?.Invoke(ReceiptId,IsFull);
    }

    public async void SetReceipt(int receiptId,List<int> _choicedMaterial)
    {
        ReceiptId = receiptId;
        var tb = DataProviderModule.Instance.GetMenuInfo(receiptId);
        
        if (_materials.Count > 0) return;
        
        // ParentWindow.LoadSpriteAsync("Assets/GameRes/Picture/UI/Kitchen/guo.png",Img_tool);
        var prefab = await ParentWindow.LoadPrefabAsync("Assets/GameRes/Prefabs/Components/FoodMaterialImg.prefab");
        
        for (int i = 0;i < tb.RelatedMaterial.Count;i++)
        {
            var one = tb.RelatedMaterial[i];
            var obj = GameObject.Instantiate(prefab,Tran_material);
            var img = obj.GetComponent<Image>();
            _materials.Add(img);
            var itemTb =  DataProviderModule.Instance.GetItemBaseInfo(one);
            ParentWindow.LoadSpriteAsync(itemTb.UiResPath, img);
        }

        IsFull = true;
        for (int i = 0;i < tb.RelatedMaterial.Count;i++)
        {
            var needMaterial = tb.RelatedMaterial[i];
            var c = _choicedMaterial.Count(id => id == needMaterial );
            _materials[i].color = c > 0 ? Color.white : Color.gray;
            IsFull &= (c > 0);
        }

        Txt_name.text = tb.Name;
    }

    public void RefreshMaterial(List<int> _choicedMaterial)
    {
        var tb = DataProviderModule.Instance.GetMenuInfo(ReceiptId);
        for (int i = 0;i < tb.RelatedMaterial.Count;i++)
        {
            var needMaterial = tb.RelatedMaterial[i];
            var c = _choicedMaterial.Count(id => id == needMaterial );
            _materials[i].color = c > 0 ? Color.white : Color.gray;
        }
    }
}