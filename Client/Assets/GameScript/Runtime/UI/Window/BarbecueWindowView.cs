using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.BarbecueWindow,"Assets/GameRes/Prefabs/Windows/BarbecueWindow.prefab")]
public partial class BarbecueWindow : UIWindow
{
	public TextMeshProUGUI Txt_tips;
	public TextMeshProUGUI Txt_timer;
	public Button Btn_start;
	public Transform Ins_CookResultWidget;
	public Slider Slider_Progress;
	public Transform Tran_QTEArea;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_tips = go.transform.Find("Txt_tips").GetComponent<TextMeshProUGUI>();
		Txt_timer = go.transform.Find("Txt_timer").GetComponent<TextMeshProUGUI>();
		Btn_start = go.transform.Find("Btn_start").GetComponent<Button>();
		Ins_CookResultWidget = go.transform.Find("Ins_CookResultWidget").GetComponent<Transform>();
		Slider_Progress = go.transform.Find("Slider_Progress").GetComponent<Slider>();
		Tran_QTEArea = go.transform.Find("Tran_QTEArea").GetComponent<Transform>();

	}
}