using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.CharacterDialogWindow,"Assets/GameRes/Prefabs/Windows/CharacterDialogWindow.prefab")]
public partial class CharacterDialogWindow : UIWindow
{
	public ButtonLongPress LBtn_Background;
	public TextMeshProUGUI Txt_Line;
	public Button Btn_Skip;
	public Button Btn_SpeedUp;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		LBtn_Background = go.transform.Find("LineView/Btn_Background").GetComponent<ButtonLongPress>();
		Txt_Line = go.transform.Find("LineView/Txt_Line").GetComponent<TextMeshProUGUI>();
		Btn_Skip = go.transform.Find("LineView/Btn_Skip").GetComponent<Button>();
		Btn_SpeedUp = go.transform.Find("LineView/Btn_SpeedUp").GetComponent<Button>();

	}
}