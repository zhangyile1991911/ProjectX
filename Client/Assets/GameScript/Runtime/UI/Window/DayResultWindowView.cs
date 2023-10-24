using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.DayResultWindow,"Assets/GameRes/Prefabs/Windows/DayResultWindow.prefab")]
public partial class DayResultWindow : UIWindow
{
	public TextMeshProUGUI Txt_DailyNote;
	public Button Btn_NextDay;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_DailyNote = go.transform.Find("Scroll View/Viewport/Txt_DailyNote").GetComponent<TextMeshProUGUI>();
		Btn_NextDay = go.transform.Find("Btn_NextDay").GetComponent<Button>();

	}
}