using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/NewsCellWidget.prefab")]
public partial class NewsCellWidget : UIComponent
{
	public TextMeshProUGUI Txt_Title;
	public Image Img_News;
	public TextMeshProUGUI Txt_Follow;
	public Transform Tran_Bottom;
	public TextMeshProUGUI Txt_Comment;
	public Image Img_Icon;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_Title = go.transform.Find("Txt_Title").GetComponent<TextMeshProUGUI>();
		Img_News = go.transform.Find("Img_News").GetComponent<Image>();
		Txt_Follow = go.transform.Find("Txt_Follow").GetComponent<TextMeshProUGUI>();
		Tran_Bottom = go.transform.Find("Tran_Bottom").GetComponent<Transform>();
		Txt_Comment = go.transform.Find("Tran_Bottom/Txt_Comment").GetComponent<TextMeshProUGUI>();
		Img_Icon = go.transform.Find("Tran_Bottom/Img_Icon").GetComponent<Image>();

	}
}