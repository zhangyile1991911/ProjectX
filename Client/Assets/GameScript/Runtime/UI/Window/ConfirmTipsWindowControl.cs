using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class ConfirmTipsWindow : UIWindow
{

    public Action Confirm;
    public Action Cancel;
    
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
        Btn_Confirm.OnClickAsObservable().Subscribe(_=>
        {
            Confirm?.Invoke();
        }).AddTo(handles);

        Btn_Cancel.OnClickAsObservable().Subscribe(_ =>
        {
            Cancel?.Invoke();
        });
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public void SetTipsInfo(string tips, Action confirmCB, Action cancelCB)
    {
        Txt_tips.text = tips;
        this.Confirm = confirmCB;
        this.Cancel = cancelCB;
    }
}