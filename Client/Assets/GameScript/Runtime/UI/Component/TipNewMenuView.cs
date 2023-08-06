using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/TipNewMenu.prefab")]
public partial class TipNewMenu : UIComponent
{
	public TextMeshProUGUI Txt_Desc;
	public Image Img_Icon;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_Desc = go.transform.Find("Txt_Desc").GetComponent<TextMeshProUGUI>();
		Img_Icon = go.transform.Find("Img_Icon").GetComponent<Image>();

	}
}