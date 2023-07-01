using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/MenuIcon.prefab")]
public partial class MenuIcon : UIComponent
{
	public Button Btn_icon;
	public Image Img_icon;
	public Image Img_Highlight;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_icon = go.transform.Find("Btn_Img_icon").GetComponent<Button>();
		Img_icon = go.transform.Find("Btn_Img_icon").GetComponent<Image>();
		Img_Highlight = go.transform.Find("Img_Highlight").GetComponent<Image>();

	}
}