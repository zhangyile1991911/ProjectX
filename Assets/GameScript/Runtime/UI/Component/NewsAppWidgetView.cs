using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/NewsAppWidget.prefab")]
public partial class NewsAppWidget : UIComponent
{
	public Image Img_BigNews;
	public LoopGridView Grid_News;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_BigNews = go.transform.Find("Img_BigNews").GetComponent<Image>();
		Grid_News = go.transform.Find("Grid_News").GetComponent<LoopGridView>();

	}
}