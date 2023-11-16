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
	public TextMeshProUGUI Txt_Name;
	public TextMeshProUGUI Txt_Comment;
	public TextMeshProUGUI Txt_like;
	public TextMeshProUGUI Txt_dislike;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_Name = go.transform.Find("Txt_Name").GetComponent<TextMeshProUGUI>();
		Txt_Comment = go.transform.Find("Txt_Comment").GetComponent<TextMeshProUGUI>();
		Txt_like = go.transform.Find("Txt_like").GetComponent<TextMeshProUGUI>();
		Txt_dislike = go.transform.Find("Txt_dislike").GetComponent<TextMeshProUGUI>();

	}
}