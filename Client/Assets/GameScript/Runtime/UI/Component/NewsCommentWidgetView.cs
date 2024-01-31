using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/NewsCommentWidget.prefab")]
public partial class NewsCommentWidget : UIComponent
{
	public Image Img_Icon;
	public TextMeshProUGUI Txt_Name;
	public RectTransform RT_Comment;
	public TextMeshProUGUI Txt_Comment;
	public RectTransform RT_line;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_Icon = go.transform.Find("Img_Icon").GetComponent<Image>();
		Txt_Name = go.transform.Find("Txt_Name").GetComponent<TextMeshProUGUI>();
		RT_Comment = go.transform.Find("RT_Txt_Comment").GetComponent<RectTransform>();
		Txt_Comment = go.transform.Find("RT_Txt_Comment").GetComponent<TextMeshProUGUI>();
		RT_line = go.transform.Find("RT_line").GetComponent<RectTransform>();

	}
}