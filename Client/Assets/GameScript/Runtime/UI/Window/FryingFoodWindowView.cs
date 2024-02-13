using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.FryingFoodWindow,"Assets/GameRes/Prefabs/Windows/FryingFoodWindow.prefab")]
public partial class FryingFoodWindow : UIWindow
{
	public Button Btn_Start;
	public TextMeshProUGUI Txt_curTemperature;
	public Transform Tran_QTEArea;
	public Transform Ins_CookResultWidget;

	public Slider Slider_Progress;
	public RectTransform Tran_minus;
	public Slider Slider_Temperature;
	public Image Img_Temperature;

	public Transform Tran_Clock;
	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Start = go.transform.Find("Btn_Start").GetComponent<Button>();
		Txt_curTemperature = go.transform.Find("Txt_curTemperature").GetComponent<TextMeshProUGUI>();
		Tran_QTEArea = go.transform.Find("Tran_QTEArea").GetComponent<Transform>();
		Ins_CookResultWidget = go.transform.Find("Ins_CookResultWidget").GetComponent<Transform>();
		
		Slider_Progress = go.transform.Find("Slider_Progress").GetComponent<Slider>();

		Tran_Clock = go.transform.Find("Tran_Clock").GetComponent<Transform>();
		Tran_minus = go.transform.Find("Tran_Clock/Tran_minus").GetComponent<RectTransform>();
		Slider_Temperature = go.transform.Find("Slider_Temperature").GetComponent<Slider>();
		Img_Temperature = go.transform.Find("Slider_Temperature/Background/mask/Img_Temperature").GetComponent<Image>();

	}
}