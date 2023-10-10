using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/RecipetWidget.prefab")]
public partial class RecipetWidget : UIComponent
{
	public Image Img_icon;
	public TextMeshProUGUI Txt_iconName;
	public TextMeshProUGUI Txt_content;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_icon = go.transform.Find("Img_icon").GetComponent<Image>();
		Txt_iconName = go.transform.Find("Txt_iconName").GetComponent<TextMeshProUGUI>();
		Txt_content = go.transform.Find("Txt_content").GetComponent<TextMeshProUGUI>();

	}
}