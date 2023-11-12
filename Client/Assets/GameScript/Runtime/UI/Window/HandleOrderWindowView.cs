using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.HandleOrderWindow,"Assets/GameRes/Prefabs/Windows/HandleOrderWindow.prefab")]
public partial class HandleOrderWindow : UIWindow
{
	public Transform Tran_LeftArea;
	public TextMeshProUGUI Txt_Title;
	public TextMeshProUGUI Txt_ReceipContent;
	public Image Img_checkMark;
	public Transform Tran_RightArea;
	public Image Img_mark;
	public TextMeshProUGUI Txt_OrderDetail;
	public XButton XBtn_close;
	public XButton XBtn_recipe;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_LeftArea = go.transform.Find("Tran_LeftArea").GetComponent<Transform>();
		Txt_Title = go.transform.Find("Tran_LeftArea/Txt_Title").GetComponent<TextMeshProUGUI>();
		Txt_ReceipContent = go.transform.Find("Tran_LeftArea/Txt_ReceipContent").GetComponent<TextMeshProUGUI>();
		Img_checkMark = go.transform.Find("Tran_LeftArea/Img_checkMark").GetComponent<Image>();
		Tran_RightArea = go.transform.Find("Tran_RightArea").GetComponent<Transform>();
		Img_mark = go.transform.Find("Tran_RightArea/Img_mark").GetComponent<Image>();
		Txt_OrderDetail = go.transform.Find("Tran_RightArea/Txt_OrderDetail").GetComponent<TextMeshProUGUI>();
		XBtn_close = go.transform.Find("Tran_RightArea/XBtn_close").GetComponent<XButton>();
		XBtn_recipe = go.transform.Find("Tran_RightArea/XBtn_recipe").GetComponent<XButton>();

	}
}