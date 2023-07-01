using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.FryingFoodWindow,"Assets/GameRes/Prefabs/Windows/FryingFoodWindow.prefab")]
public partial class FryingFoodWindow : UIWindow
{
	public Button Btn_Start;
	public Slider Slider_Progress;
	public Slider Slider_Temperature;
	public Image Img_Temperature;
	public Transform Tran_QTEArea;
	public TextMeshProUGUI Txt_Tips;
	public Transform Tran_Result;
	public Button Btn_Close;
	public TextMeshProUGUI Txt_Menu;
	public TextMeshProUGUI Txt_Result;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Start = go.transform.Find("Btn_Start").GetComponent<Button>();
		Slider_Progress = go.transform.Find("Slider_Progress").GetComponent<Slider>();
		Slider_Temperature = go.transform.Find("Slider_Temperature").GetComponent<Slider>();
		Img_Temperature = go.transform.Find("Slider_Temperature/Img_Temperature").GetComponent<Image>();
		Tran_QTEArea = go.transform.Find("Tran_QTEArea").GetComponent<Transform>();
		Txt_Tips = go.transform.Find("Txt_Tips").GetComponent<TextMeshProUGUI>();
		Tran_Result = go.transform.Find("Tran_Result").GetComponent<Transform>();
		Btn_Close = go.transform.Find("Tran_Result/Btn_Close").GetComponent<Button>();
		Txt_Menu = go.transform.Find("Tran_Result/Txt_Menu").GetComponent<TextMeshProUGUI>();
		Txt_Result = go.transform.Find("Tran_Result/Txt_Result").GetComponent<TextMeshProUGUI>();

	}
}