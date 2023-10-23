using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/RefrigeratorCell.prefab")]
public partial class RefrigeratorCell : UIComponent
{
	public Image Img_highLight;
	public XButton Btn_Item;
	public Image Img_Item;
	public TextMeshProUGUI Txt_Num;
	public TextMeshProUGUI Txt_Name;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_highLight = go.transform.Find("Img_highLight").GetComponent<Image>();
		Btn_Item = go.transform.Find("XBtn_Img_Item").GetComponent<XButton>();
		Img_Item = go.transform.Find("XBtn_Img_Item").GetComponent<Image>();
		Txt_Num = go.transform.Find("Txt_Num").GetComponent<TextMeshProUGUI>();
		Txt_Name = go.transform.Find("Txt_Name").GetComponent<TextMeshProUGUI>();

	}
}