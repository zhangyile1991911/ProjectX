using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/CalenderSheetWidget.prefab")]
public partial class CalenderSheetWidget : UIComponent
{
	public TextMeshProUGUI Txt_month;
	public TextMeshProUGUI Txt_tips;
	public TextMeshProUGUI Txt_day;
	public TextMeshProUGUI Txt_weekday;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_month = go.transform.Find("Txt_month").GetComponent<TextMeshProUGUI>();
		Txt_tips = go.transform.Find("Txt_tips").GetComponent<TextMeshProUGUI>();
		Txt_day = go.transform.Find("Txt_day").GetComponent<TextMeshProUGUI>();
		Txt_weekday = go.transform.Find("Txt_weekday").GetComponent<TextMeshProUGUI>();

	}
}