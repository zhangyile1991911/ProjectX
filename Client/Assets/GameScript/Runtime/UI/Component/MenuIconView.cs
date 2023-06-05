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
	public Button Btn_icon;
	public Image Img_icon;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_icon = go.transform.Find("Btn_Img_icon").GetComponent<Button>();
		Img_icon = go.transform.Find("Btn_Img_icon").GetComponent<Image>();

	}
}