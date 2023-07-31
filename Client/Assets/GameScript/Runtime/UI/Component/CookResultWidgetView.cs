using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/CookResultWidget.prefab")]
public partial class CookResultWidget : UIComponent
{
	public Button Btn_Close;
	public TextMeshProUGUI Txt_Menu;
	public TextMeshProUGUI Txt_Result;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Close = go.transform.Find("Btn_Close").GetComponent<Button>();
		Txt_Menu = go.transform.Find("Txt_Menu").GetComponent<TextMeshProUGUI>();
		Txt_Result = go.transform.Find("Txt_Result").GetComponent<TextMeshProUGUI>();

	}
}