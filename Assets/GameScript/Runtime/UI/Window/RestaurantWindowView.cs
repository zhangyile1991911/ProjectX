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
	public Transform Tran_BtnGroup;
	public Button Btn_Phone;
	public Transform Tran_OrderGroup;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_BtnGroup = go.transform.Find("Tran_BtnGroup").GetComponent<Transform>();
		Btn_Phone = go.transform.Find("Tran_BtnGroup/Btn_Phone").GetComponent<Button>();
		Tran_OrderGroup = go.transform.Find("Tran_OrderGroup").GetComponent<Transform>();

	}
}