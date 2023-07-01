using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/CookToolIcon.prefab")]
public partial class CookToolIcon : UIComponent
{
	public Button Btn_Click;
	public Image Img_Click;
	public Image Img_Highlight;
	public TextMeshProUGUI Txt_toolsName;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Click = go.transform.Find("Btn_Img_Click").GetComponent<Button>();
		Img_Click = go.transform.Find("Btn_Img_Click").GetComponent<Image>();
		Img_Highlight = go.transform.Find("Img_Highlight").GetComponent<Image>();
		Txt_toolsName = go.transform.Find("Txt_toolsName").GetComponent<TextMeshProUGUI>();

	}
}