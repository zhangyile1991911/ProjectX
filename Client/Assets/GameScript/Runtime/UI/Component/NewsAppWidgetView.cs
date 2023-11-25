﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/NewsAppWidget.prefab")]
public partial class NewsAppWidget : BaseAppWidget
{
	public Image Img_BigNews;
	public LoopListView2 Grid_News;
	public Transform Ins_Detail;
	

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_BigNews = go.transform.Find("Img_BigNews").GetComponent<Image>();
		Grid_News = go.transform.Find("Grid_News").GetComponent<LoopListView2>();
		Ins_Detail = go.transform.Find("Ins_Detail").GetComponent<Transform>();

	}
}