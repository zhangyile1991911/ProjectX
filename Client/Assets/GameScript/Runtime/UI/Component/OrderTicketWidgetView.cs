using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/OrderTicketWidget.prefab")]
public partial class OrderTicketWidget : UIComponent
{
	public TextMeshProUGUI Txt_result;
	public TextMeshProUGUI Txt_content;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_result = go.transform.Find("Txt_result").GetComponent<TextMeshProUGUI>();
		Txt_content = go.transform.Find("Txt_content").GetComponent<TextMeshProUGUI>();

	}
}