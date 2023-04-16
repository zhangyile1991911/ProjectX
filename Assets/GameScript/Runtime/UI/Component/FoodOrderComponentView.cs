using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/FoodOrderComponent.prefab")]
public partial class FoodOrderComponent : UIComponent
{
	public Image Img_Food;
	public Image Img_BgName;
	public TextMeshProUGUI Txt_Name;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_Food = go.transform.Find("Img_Food").GetComponent<Image>();
		Img_BgName = go.transform.Find("Img_BgName").GetComponent<Image>();
		Txt_Name = go.transform.Find("Txt_Name").GetComponent<TextMeshProUGUI>();

	}
}