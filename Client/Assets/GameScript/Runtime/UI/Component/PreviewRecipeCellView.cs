using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/PreviewRecipeCell.prefab")]
public partial class PreviewRecipeCell : UIComponent
{
	public XButton XBtn_bg;
	public TextMeshProUGUI Txt_ReciptName;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		XBtn_bg = go.transform.Find("XBtn_bg").GetComponent<XButton>();
		Txt_ReciptName = go.transform.Find("Txt_ReciptName").GetComponent<TextMeshProUGUI>();

	}
}