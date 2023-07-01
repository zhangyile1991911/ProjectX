using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/DragCookFoodIcon.prefab")]
public partial class DragCookFoodIcon : UIComponent
{
	public Image Img_Icon;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_Icon = go.transform.Find("Img_Icon").GetComponent<Image>();

	}
}