using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/NewsDetailWidget.prefab")]
public partial class NewsDetailWidget : UIComponent
{
	public Transform Tran_Content;
	public TextMeshProUGUI Txt_Title;
	public TextMeshProUGUI Txt_From;
	public TextMeshProUGUI Txt_Content;
	public Transform Tran_Sep;
	public Transform Tran_Coment;
	public Transform Ins_CommentA;
	public Transform Ins_CommentB;
	public Transform Ins_CommentC;
	public XButton XBtn_Return;
	public ScrollRect ScrollRect;
	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_Content = go.transform.Find("Scroll View/Viewport/Content/Tran_Content").GetComponent<Transform>();
		Txt_Title = go.transform.Find("Scroll View/Viewport/Content/Tran_Content/Txt_Title").GetComponent<TextMeshProUGUI>();
		Txt_From = go.transform.Find("Scroll View/Viewport/Content/Tran_Content/Txt_From").GetComponent<TextMeshProUGUI>();
		Txt_Content = go.transform.Find("Scroll View/Viewport/Content/Tran_Content/Txt_Content").GetComponent<TextMeshProUGUI>();
		Tran_Sep = go.transform.Find("Scroll View/Viewport/Content/Tran_Sep").GetComponent<Transform>();
		Tran_Coment = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment").GetComponent<Transform>();
		Ins_CommentA = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment/Ins_CommentA").GetComponent<Transform>();
		Ins_CommentB = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment/Ins_CommentB").GetComponent<Transform>();
		Ins_CommentC = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment/Ins_CommentC").GetComponent<Transform>();
		XBtn_Return = go.transform.Find("Scroll View/Viewport/Content/XBtn_Return").GetComponent<XButton>();
		ScrollRect = go.transform.Find("Scroll View").GetComponent<ScrollRect>();
	}
}