using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.CharacterDialogWindow,"Assets/GameRes/Prefabs/Windows/CharacterDialogWindow.prefab")]
public partial class CharacterDialogWindow : UIWindow
{
	public Button Btn_Background;
	public TextMeshProUGUI Txt_Line;
	public Image Img_tail;
	public Button Btn_Skip;
	public Button Btn_SpeedUp;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_Background = go.transform.Find("LineView/Btn_Background").GetComponent<Button>();
		Txt_Line = go.transform.Find("LineView/Btn_Background/Txt_Line").GetComponent<TextMeshProUGUI>();
		Img_tail = go.transform.Find("LineView/Btn_Background/Img_tail").GetComponent<Image>();
		Btn_Skip = go.transform.Find("LineView/Btn_Skip").GetComponent<Button>();
		Btn_SpeedUp = go.transform.Find("LineView/Btn_SpeedUp").GetComponent<Button>();
	}
}