using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/FoodIcon.prefab")]
public partial class FoodIcon : UIComponent
{
	public Button Btn_Food;
	public Image Img_Food;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Food = go.transform.Find("Btn_Img_Food").GetComponent<Button>();
		Img_Food = go.transform.Find("Btn_Img_Food").GetComponent<Image>();

	}
}