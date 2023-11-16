﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/NewsCellWidget.prefab")]
public partial class NewsCellWidget : UIComponent
{
	public XButton XBtn_bg;
	public Image Img_News;
	public TextMeshProUGUI Txt_Title;
	public TextMeshProUGUI Txt_Follow;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		XBtn_bg = go.transform.Find("XBtn_bg").GetComponent<XButton>();
		Img_News = go.transform.Find("Img_News").GetComponent<Image>();
		Txt_Title = go.transform.Find("Txt_Title").GetComponent<TextMeshProUGUI>();
		Txt_Follow = go.transform.Find("Txt_Follow").GetComponent<TextMeshProUGUI>();

	}
}