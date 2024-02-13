using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UniRx;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class CookResultWidget : UIComponent
{
    private List<StarWidget> _starWidgets;
    private List<FlavorTagWidget> _flavorTagWidgets;
    public CookResultWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        _starWidgets = new List<StarWidget>(3);
        _starWidgets.Add(new StarWidget(Ins_StarWidgetA.gameObject,this.ParentWindow));
        _starWidgets.Add(new StarWidget(Ins_StarWidgetB.gameObject,this.ParentWindow));
        _starWidgets.Add(new StarWidget(Ins_StarWidgetC.gameObject,this.ParentWindow));

        _flavorTagWidgets = new List<FlavorTagWidget>(5);
    }
    
    public override void OnDestroy()
    {
        _starWidgets.Clear();
        _flavorTagWidgets.Clear();
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
    
    public async void ShowGameOver(CookResult cookResult)
    {
        var itemTb = DataProviderModule.Instance.GetItemBaseInfo(cookResult.MenuId);
        Txt_Menu.text = itemTb.Name;
        
        var score = (cookResult.Score/cookResult.MaxScore)*100f;
        
        _starWidgets[0].HalfLight(Mathf.Clamp01(score/33f));
        _starWidgets[1].HalfLight(Mathf.Clamp01((score-33f)/33f));
        _starWidgets[2].HalfLight(Mathf.Clamp01((score-66f)/33f));

        if (_flavorTagWidgets.Count < cookResult.Tags.Count)
        {
            var prefab = await ParentWindow.LoadPrefabAsync("Assets/GameRes/Prefabs/Components/FlavorTagWidget.prefab");
            var create_count = cookResult.Tags.Count - _flavorTagWidgets.Count;
            for (int i = 0; i < create_count; i++)
            {
                var goObj = GameObject.Instantiate(prefab, Tran_flavor);
                var tagWidget = new FlavorTagWidget(goObj, ParentWindow);
                _flavorTagWidgets.Add(tagWidget);
            }
        }

        foreach (var one in _flavorTagWidgets)
        {
            one.OnHide();
        }

        int index = 0;
        foreach (var tagId in cookResult.Tags)
        {
            var str = DataProviderModule.Instance.FlavourStr((int)tagId);
            _flavorTagWidgets[index].Txt_flavor.text = str;
            _flavorTagWidgets[index].OnShow(null);
            index++;
        }
        
    }
}