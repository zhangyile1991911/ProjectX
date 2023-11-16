using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/PhoneAppWidget.prefab")]
public partial class PhoneAppWidget : UIComponent
{
	public XButton XBtn_App;
	public Image Img_App;
	public TextMeshProUGUI Txt_App;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		XBtn_App = go.transform.Find("XBtn_Img_App").GetComponent<XButton>();
		Img_App = go.transform.Find("XBtn_Img_App").GetComponent<Image>();
		Txt_App = go.transform.Find("Txt_App").GetComponent<TextMeshProUGUI>();

	}
}