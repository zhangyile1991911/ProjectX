using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/MenuIcon.prefab")]
public partial class MenuIcon : UIComponent
{
	public Image Img_icon;
	public TextMeshProUGUI Txt_num;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_icon = go.transform.Find("Img_icon").GetComponent<Image>();
		Txt_num = go.transform.Find("Txt_num").GetComponent<TextMeshProUGUI>();

	}
}