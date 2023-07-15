using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/StatementWidget.prefab")]
public partial class StatementWidget : UIComponent
{
	public Image Img_Icon;
	public TextMeshProUGUI Txt_Name;
	public TextMeshProUGUI Txt_Num;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_Icon = go.transform.Find("Img_Icon").GetComponent<Image>();
		Txt_Name = go.transform.Find("Txt_Name").GetComponent<TextMeshProUGUI>();
		Txt_Num = go.transform.Find("Txt_Num").GetComponent<TextMeshProUGUI>();

	}
}