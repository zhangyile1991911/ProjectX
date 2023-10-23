using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/TipCommon.prefab")]
public partial class TipCommon : UIComponent
{
	public TextMeshProUGUI Txt_tip;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_tip = go.transform.Find("Txt_tip").GetComponent<TextMeshProUGUI>();

	}
}