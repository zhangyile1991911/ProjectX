using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlasticGui.WebApi.Responses;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class RefrigeratorCell : UIComponent
{
    private Action<int> ClickCB;
    private int itemId;
    private int itemNum;
    public RefrigeratorCell(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        Btn_Item.OnClick.Subscribe((param) =>
        {
            ClickCB?.Invoke(itemId);
        }).AddTo(uiTran);
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

    public void SetCellInfo(int id,int num,Action<int> cb)
    {
        Img_Item.gameObject.SetActive(true);
        var item_tb = DataProviderModule.Instance.GetItemBaseInfo(id);
        loadRes(item_tb.UiResPath);
        itemId = id;
        itemNum = num;
        Txt_Num.text = num.ToString();
        Txt_Name.text = item_tb.Name;
        ClickCB = cb;

    }

    private void loadRes(string res)
    {
        var itemSp = YooAssets.LoadAssetSync<Sprite>(res);
        // itemSp.Completed += (param) =>
        // {
        //     Img_Item.sprite = param.AssetObject as Sprite;
        // };
        // await itemSp.ToUniTask();
        Img_Item.sprite = itemSp.AssetObject as Sprite;
    }
}