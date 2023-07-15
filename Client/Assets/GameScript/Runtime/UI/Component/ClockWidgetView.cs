using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/ClockWidget.prefab")]
public partial class ClockWidget : UIComponent
{
	public TextMeshProUGUI Txt_timer;
	public TextMeshProUGUI Txt_weekday;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_timer = go.transform.Find("Txt_timer").GetComponent<TextMeshProUGUI>();
		Txt_weekday = go.transform.Find("Txt_weekday").GetComponent<TextMeshProUGUI>();

	}
}