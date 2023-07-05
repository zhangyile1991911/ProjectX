using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.UI;
using YooAsset;

public class FoodBtnCell : UIComponent
{
    public TextMeshProUGUI Txt_num;
    public Image Img_food;
    public Button Btn_food;
    
    private Action<int> _onclick;
    private int _foodId;
    private int _foodNum;
    public FoodBtnCell(GameObject go, UIWindow parent) : base(go, parent)
    {
    }
    
    public override void Init(GameObject go)
    {
        uiGo = go;
        
        Img_food = go.transform.GetComponent<Image>();
        Btn_food = go.transform.GetComponent<Button>();
        Txt_num = go.transform.Find("Txt_num").GetComponent<TextMeshProUGUI>();
        
        Btn_food.OnClickAsObservable().Subscribe(onClick).AddTo(uiTran);
    }
    
    public override void OnCreate()
    {
        base.OnCreate();
        
    }
    
    public override void OnDestroy()
    {
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
        base.OnUpdate();
    }

    public void SetFoodInfo(int foodId,int num,bool canChoice,Action<int> click)
    {
        _foodId = foodId;
        _foodNum = num;
        _onclick = click;

        Txt_num.text = _foodNum.ToString();
        var dataProviderModule = UniModule.GetModule<DataProviderModule>();
        var foodTb = dataProviderModule.GetItemBaseInfo(foodId);
        var handler = YooAssets.LoadAssetAsync<Sprite>(foodTb.UiResPath);
        handler.Completed += (opera) =>
        {
            Img_food.sprite = opera.AssetObject as Sprite;
        };
        Btn_food.interactable = canChoice;
    }

    private void onClick(Unit param)
    {
        _onclick?.Invoke(_foodId);
    }
}
