using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/FlavorTagWidget.prefab")]
public partial class FlavorTagWidget : UIComponent
{
	public TextMeshProUGUI Txt_flavor;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_flavor = go.transform.Find("Txt_flavor").GetComponent<TextMeshProUGUI>();

	}
}