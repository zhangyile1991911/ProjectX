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
	public Slider Slider_Progress;
	public Slider Slider_Temperature;
	public Image Img_Temperature;
	public Transform Tran_QTEArea;
	public TextMeshProUGUI Txt_Tips;
	public Transform Ins_CookResultWidget;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Start = go.transform.Find("Btn_Start").GetComponent<Button>();
		Slider_Progress = go.transform.Find("Slider_Progress").GetComponent<Slider>();
		Slider_Temperature = go.transform.Find("Slider_Temperature").GetComponent<Slider>();
		Img_Temperature = go.transform.Find("Slider_Temperature/Img_Temperature").GetComponent<Image>();
		Tran_QTEArea = go.transform.Find("Tran_QTEArea").GetComponent<Transform>();
		Txt_Tips = go.transform.Find("Txt_Tips").GetComponent<TextMeshProUGUI>();
		Ins_CookResultWidget = go.transform.Find("Ins_CookResultWidget").GetComponent<Transform>();

	}
}