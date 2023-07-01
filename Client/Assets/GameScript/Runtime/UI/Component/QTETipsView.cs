using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/QTETips.prefab")]
public partial class QTETips : UIComponent
{
	public TextMeshProUGUI Txt_tips;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_tips = go.transform.Find("Txt_tips").GetComponent<TextMeshProUGUI>();

	}
}