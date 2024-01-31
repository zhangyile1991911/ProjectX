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
	public Image Img_returnbg;
	public XButton XBtn_return;
	public Transform Tran_Title;
	public TextMeshProUGUI Txt_Title;
	public Image Img_picon;
	public TextMeshProUGUI Txt_author;
	public TextMeshProUGUI Txt_Content;
	public Transform Tran_Coment;
	public Transform Ins_CommentA;
	public Transform Ins_CommentB;
	public Transform Ins_CommentC;
	public Image Img_bottombg;
	public ScrollRect ContentScrollRect;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
	    ContentScrollRect = go.transform.Find("Scroll View").GetComponent<ScrollRect>(); 
		Img_returnbg = go.transform.Find("Img_returnbg").GetComponent<Image>();
		XBtn_return = go.transform.Find("XBtn_return").GetComponent<XButton>();
		Tran_Title = go.transform.Find("Scroll View/Viewport/Content/Tran_Title").GetComponent<Transform>();
		Txt_Title = go.transform.Find("Scroll View/Viewport/Content/Tran_Title/Txt_Title").GetComponent<TextMeshProUGUI>();
		Img_picon = go.transform.Find("Scroll View/Viewport/Content/Tran_Title/Img_picon").GetComponent<Image>();
		Txt_author = go.transform.Find("Scroll View/Viewport/Content/Tran_Title/Txt_author").GetComponent<TextMeshProUGUI>();
		Txt_Content = go.transform.Find("Scroll View/Viewport/Content/Txt_Content").GetComponent<TextMeshProUGUI>();
		Tran_Coment = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment").GetComponent<Transform>();
		Ins_CommentA = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment/Ins_CommentA").GetComponent<Transform>();
		Ins_CommentB = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment/Ins_CommentB").GetComponent<Transform>();
		Ins_CommentC = go.transform.Find("Scroll View/Viewport/Content/Tran_Coment/Ins_CommentC").GetComponent<Transform>();
		Img_bottombg = go.transform.Find("Img_bottombg").GetComponent<Image>();

	}
}