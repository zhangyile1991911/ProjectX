using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.CalenderWindow,"Assets/GameRes/Prefabs/Windows/CalenderWindow.prefab")]
public partial class CalenderWindow : UIWindow
{
	public Image Img_weather;
	public TextMeshProUGUI Txt_weather;
	public TextMeshProUGUI Txt_date;
	public XButton XBtn_continue;
	public TextMeshProUGUI Txt_tip;
	public Transform Go_weather;
	public Transform Ins_NowDate;
	public Transform Ins_PreviouDate;
	public Image Img_previous;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_weather = go.transform.Find("Img_weather").GetComponent<Image>();
		Txt_weather = go.transform.Find("Txt_weather").GetComponent<TextMeshProUGUI>();
		Txt_date = go.transform.Find("Txt_date").GetComponent<TextMeshProUGUI>();
		XBtn_continue = go.transform.Find("XBtn_continue").GetComponent<XButton>();
		Txt_tip = go.transform.Find("tipbg/Txt_tip").GetComponent<TextMeshProUGUI>();
		Go_weather = go.transform.Find("Go_weather").GetComponent<Transform>();
		Ins_NowDate = go.transform.Find("Ins_NowDate").GetComponent<Transform>();
		Ins_PreviouDate = go.transform.Find("Ins_PreviouDate").GetComponent<Transform>();
		Img_previous = go.transform.Find("Img_previous").GetComponent<Image>();
	}
}