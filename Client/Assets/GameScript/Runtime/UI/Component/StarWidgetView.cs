using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/StarWidget.prefab")]
public partial class StarWidget : UIComponent
{
	public Image Img_starblack;
	public Image Img_starshadow;
	public Image Img_starlight;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_starblack = go.transform.Find("Img_starblack").GetComponent<Image>();
		Img_starshadow = go.transform.Find("Img_starshadow").GetComponent<Image>();
		Img_starlight = go.transform.Find("Img_starlight").GetComponent<Image>();

	}
}