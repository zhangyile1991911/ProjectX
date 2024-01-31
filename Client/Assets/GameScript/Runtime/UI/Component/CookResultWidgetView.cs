using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/CookResultWidget.prefab")]
public partial class CookResultWidget : UIComponent
{
	public Button Btn_Close;
	public TextMeshProUGUI Txt_Menu;
	public TextMeshProUGUI Txt_Result;
	public TextMeshProUGUI Txt_tips;
	public Transform Tran_stars;
	public Transform Ins_StarWidgetA;
	public Transform Ins_StarWidgetB;
	public Transform Ins_StarWidgetC;
	public Transform Tran_flavor;
	public Image Img_menu;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Close = go.transform.Find("Btn_Close").GetComponent<Button>();
		Txt_Menu = go.transform.Find("Txt_Menu").GetComponent<TextMeshProUGUI>();
		Txt_Result = go.transform.Find("Txt_Result").GetComponent<TextMeshProUGUI>();
		Txt_tips = go.transform.Find("Txt_tips").GetComponent<TextMeshProUGUI>();
		Tran_stars = go.transform.Find("Tran_stars").GetComponent<Transform>();
		Ins_StarWidgetA = go.transform.Find("Tran_stars/Ins_StarWidgetA").GetComponent<Transform>();
		Ins_StarWidgetB = go.transform.Find("Tran_stars/Ins_StarWidgetB").GetComponent<Transform>();
		Ins_StarWidgetC = go.transform.Find("Tran_stars/Ins_StarWidgetC").GetComponent<Transform>();
		Tran_flavor = go.transform.Find("Tran_flavor").GetComponent<Transform>();
		Img_menu = go.transform.Find("Img_menu").GetComponent<Image>();

	}
}