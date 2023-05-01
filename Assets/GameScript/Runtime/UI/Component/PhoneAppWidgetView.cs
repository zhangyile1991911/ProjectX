using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/PhoneAppWidget.prefab")]
public partial class PhoneAppWidget : UIComponent
{
	public Button Btn_App;
	public TextMeshProUGUI Txt_App;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_App = go.transform.Find("Btn_App").GetComponent<Button>();
		Txt_App = go.transform.Find("Txt_App").GetComponent<TextMeshProUGUI>();

	}
}