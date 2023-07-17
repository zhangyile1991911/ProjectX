using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.ConfirmTipsWindow,"Assets/GameRes/Prefabs/Windows/ConfirmTipsWindow.prefab")]
public partial class ConfirmTipsWindow : UIWindow
{
	public TextMeshProUGUI Txt_tips;
	public Button Btn_Confirm;
	public Button Btn_Cancel;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_tips = go.transform.Find("Txt_tips").GetComponent<TextMeshProUGUI>();
		Btn_Confirm = go.transform.Find("Btn_Confirm").GetComponent<Button>();
		Btn_Cancel = go.transform.Find("Btn_Cancel").GetComponent<Button>();

	}
}