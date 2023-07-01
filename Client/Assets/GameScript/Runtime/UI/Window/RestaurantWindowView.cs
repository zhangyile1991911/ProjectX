using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.RestaurantWindow,"Assets/GameRes/Prefabs/Windows/RestaurantWindow.prefab")]
public partial class RestaurantWindow : UIWindow
{
	public Transform Ins_ClockWidget;
	public Transform Tran_BtnGroup;
	public Button Btn_Phone;
	public Transform Tran_OrderGroup;
	public Transform Tran_BubbleGroup;
	public Transform Tran_CookFoodGroup;
	public Transform Ins_DragCookFoodIconA;
	public Transform Ins_DragCookFoodIconB;
	public Transform Ins_DragCookFoodIconC;
	public Transform Ins_DragCookFoodIconD;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Ins_ClockWidget = go.transform.Find("Ins_ClockWidget").GetComponent<Transform>();
		Tran_BtnGroup = go.transform.Find("Tran_BtnGroup").GetComponent<Transform>();
		Btn_Phone = go.transform.Find("Tran_BtnGroup/Btn_Phone").GetComponent<Button>();
		Tran_OrderGroup = go.transform.Find("Tran_OrderGroup").GetComponent<Transform>();
		Tran_BubbleGroup = go.transform.Find("Tran_BubbleGroup").GetComponent<Transform>();
		Tran_CookFoodGroup = go.transform.Find("Tran_CookFoodGroup").GetComponent<Transform>();
		Ins_DragCookFoodIconA = go.transform.Find("Tran_CookFoodGroup/Ins_DragCookFoodIconA").GetComponent<Transform>();
		Ins_DragCookFoodIconB = go.transform.Find("Tran_CookFoodGroup/Ins_DragCookFoodIconB").GetComponent<Transform>();
		Ins_DragCookFoodIconC = go.transform.Find("Tran_CookFoodGroup/Ins_DragCookFoodIconC").GetComponent<Transform>();
		Ins_DragCookFoodIconD = go.transform.Find("Tran_CookFoodGroup/Ins_DragCookFoodIconD").GetComponent<Transform>();

	}
}