using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class PreviewRecipeCell : UIComponent
{
    private Action<int,bool> confirm;
    private int _recipeId;
    private bool _isFull;
    public PreviewRecipeCell(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        XBtn_bg.OnClick.Subscribe(param =>
        {
            confirm?.Invoke(_recipeId,_isFull);
        }).AddTo(uiRectTran);
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

    public void SetRecipe(int recipeId,string name,bool isFull,Action<int,bool> cb)
    {
        _recipeId = recipeId;
        _isFull = isFull;
        
        Txt_ReciptName.text = name;
        Txt_ReciptName.color = isFull ? Color.white : Color.red;

        confirm = cb;
    }

    
    
}