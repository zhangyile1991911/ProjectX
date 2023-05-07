using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.RestaurantWindow,"Assets/GameRes/Prefabs/Windows/RestaurantWindow.prefab")]
public partial class RestaurantWindow : UIWindow
{
	public Transform Ins_ClockWidget;
	public Transform Tran_BtnGroup;
	public Button Btn_Phone;
	public Button Btn_Bubble;
	public Transform Tran_OrderGroup;
	public Transform Tran_BubbleGroup;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Ins_ClockWidget = go.transform.Find("Ins_ClockWidget").GetComponent<Transform>();
		Tran_BtnGroup = go.transform.Find("Tran_BtnGroup").GetComponent<Transform>();
		Btn_Phone = go.transform.Find("Tran_BtnGroup/Btn_Phone").GetComponent<Button>();
		Btn_Bubble = go.transform.Find("Tran_BtnGroup/Btn_Bubble").GetComponent<Button>();
		Tran_OrderGroup = go.transform.Find("Tran_OrderGroup").GetComponent<Transform>();
		Tran_BubbleGroup = go.transform.Find("Tran_BubbleGroup").GetComponent<Transform>();

	}
}