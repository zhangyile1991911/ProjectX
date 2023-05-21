using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/FoodItemCell.prefab")]
public partial class FoodItemCell : UIComponent
{
	public Image Img_Food;
	public TextMeshProUGUI Txt_Name;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_Food = go.transform.Find("Img_Food").GetComponent<Image>();
		Txt_Name = go.transform.Find("Txt_Name").GetComponent<TextMeshProUGUI>();

	}
}