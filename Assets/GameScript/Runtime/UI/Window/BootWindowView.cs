using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.BootWindow,"Assets/GameRes/Prefabs/Windows/BootWindow.prefab")]
public partial class BootWindow : UIWindow
{
	public Button Btn_Restaurant;
	public Button Btn_FryFood;
	public Button Btn_Barbecue;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Restaurant = go.transform.Find("Btn_Restaurant").GetComponent<Button>();
		Btn_FryFood = go.transform.Find("Btn_FryFood").GetComponent<Button>();
		Btn_Barbecue = go.transform.Find("Btn_Barbecue").GetComponent<Button>();

	}
}