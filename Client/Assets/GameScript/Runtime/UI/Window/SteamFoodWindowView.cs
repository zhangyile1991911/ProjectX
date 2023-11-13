using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.SteamFoodWindow,"Assets/GameRes/Prefabs/Windows/SteamFoodWindow.prefab")]
public partial class SteamFoodWindow : UIWindow
{
	public XButton XBtn_Start;
	public TextMeshProUGUI Txt_CountDown;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		XBtn_Start = go.transform.Find("XBtn_Start").GetComponent<XButton>();
		Txt_CountDown = go.transform.Find("Txt_CountDown").GetComponent<TextMeshProUGUI>();

	}
}