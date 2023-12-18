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
	public Transform Tran_Detail;
	public TextMeshProUGUI Txt_time;
	public TextMeshProUGUI Txt_customername;
	public TextMeshProUGUI Txt_up;
	public TextMeshProUGUI Txt_titleName;
	public TextMeshProUGUI Txt_titleSale;
	public TextMeshProUGUI Txt_titleNum;
	public TextMeshProUGUI Txt_titleTotal;
	public TextMeshProUGUI Txt_orderName;
	public TextMeshProUGUI Txt_orderSale;
	public TextMeshProUGUI Txt_orderNum;
	public TextMeshProUGUI Txt_orderTotal;
	public TextMeshProUGUI Txt_down;
	public TextMeshProUGUI Txt_extra;

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
		Tran_Detail = go.transform.Find("Tran_RightArea/Tran_Detail").GetComponent<Transform>();
		Txt_time = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_time").GetComponent<TextMeshProUGUI>();
		Txt_customername = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_customername").GetComponent<TextMeshProUGUI>();
		Txt_up = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_splitLine_up").GetComponent<TextMeshProUGUI>();
		Txt_titleName = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_titleName").GetComponent<TextMeshProUGUI>();
		Txt_titleSale = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_titleSale").GetComponent<TextMeshProUGUI>();
		Txt_titleNum = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_titleNum").GetComponent<TextMeshProUGUI>();
		Txt_titleTotal = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_titleTotal").GetComponent<TextMeshProUGUI>();
		Txt_orderName = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_orderName").GetComponent<TextMeshProUGUI>();
		Txt_orderSale = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_orderSale").GetComponent<TextMeshProUGUI>();
		Txt_orderNum = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_orderNum").GetComponent<TextMeshProUGUI>();
		Txt_orderTotal = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_orderTotal").GetComponent<TextMeshProUGUI>();
		Txt_down = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_splitLine_down").GetComponent<TextMeshProUGUI>();
		Txt_extra = go.transform.Find("Tran_RightArea/Tran_Detail/Txt_extra").GetComponent<TextMeshProUGUI>();

	}
}