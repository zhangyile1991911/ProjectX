using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/WeatherApp.prefab")]
public partial class WeatherApp : BaseAppWidget
{
	public Image Img_Sky;
	public Image Img_Sun;
	public Image Img_Moutain;
	public Image Img_grass;
	public Transform Tran_Cloud;
	public Transform Tran_falling;
	public Transform Tran_UI;
	public Transform Tran_panel;
	public TextMeshProUGUI Txt_day;
	public TextMeshProUGUI Txt_condition;
	public XButton XBtn_arrow;
	public Image Img_show;
	public TextMeshProUGUI Txt_temp;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_Sky = go.transform.Find("Img_Sky").GetComponent<Image>();
		Img_Sun = go.transform.Find("Img_Sun").GetComponent<Image>();
		Img_Moutain = go.transform.Find("Img_Moutain").GetComponent<Image>();
		Img_grass = go.transform.Find("Img_grass").GetComponent<Image>();
		Tran_Cloud = go.transform.Find("Tran_Cloud").GetComponent<Transform>();
		Tran_falling = go.transform.Find("Tran_falling").GetComponent<Transform>();
		Tran_UI = go.transform.Find("Tran_UI").GetComponent<Transform>();
		Tran_panel = go.transform.Find("Tran_UI/Tran_panel").GetComponent<Transform>();
		Txt_day = go.transform.Find("Tran_UI/Tran_panel/Txt_day").GetComponent<TextMeshProUGUI>();
		Txt_condition = go.transform.Find("Tran_UI/Tran_panel/Txt_condition").GetComponent<TextMeshProUGUI>();
		XBtn_arrow = go.transform.Find("Tran_UI/Tran_panel/XBtn_arrow").GetComponent<XButton>();
		Img_show = go.transform.Find("Tran_UI/Tran_panel/Img_show").GetComponent<Image>();
		Txt_temp = go.transform.Find("Tran_UI/Tran_panel/Txt_temp").GetComponent<TextMeshProUGUI>();

	}
}