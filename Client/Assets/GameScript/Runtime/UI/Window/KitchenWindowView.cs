using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.KitchenWindow,"Assets/GameRes/Prefabs/Windows/KitchenWindow.prefab")]
public partial class KitchenWindow : UIWindow
{
	public Transform Tran_refrigerator;
	public XButton XBtn_A;
	public XButton XBtn_B;
	public XButton XBtn_C;
	public XButton XBtn_D;
	public LoopGridView Grid_Refrigerator;
	public XButton XBtn_pick;
	public XButton XBtn_order;
	public XButton XBtn_cancel;
	public LoopListView2 List2_PreviewRecipt;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_refrigerator = go.transform.Find("Tran_refrigerator").GetComponent<Transform>();
		XBtn_A = go.transform.Find("Tran_refrigerator/XBtn_A").GetComponent<XButton>();
		XBtn_B = go.transform.Find("Tran_refrigerator/XBtn_B").GetComponent<XButton>();
		XBtn_C = go.transform.Find("Tran_refrigerator/XBtn_C").GetComponent<XButton>();
		XBtn_D = go.transform.Find("Tran_refrigerator/XBtn_D").GetComponent<XButton>();
		Grid_Refrigerator = go.transform.Find("Tran_refrigerator/Grid_Refrigerator").GetComponent<LoopGridView>();
		XBtn_pick = go.transform.Find("XBtn_pick").GetComponent<XButton>();
		XBtn_order = go.transform.Find("XBtn_order").GetComponent<XButton>();
		XBtn_cancel = go.transform.Find("XBtn_cancel").GetComponent<XButton>();
		List2_PreviewRecipt = go.transform.Find("List2_PreviewRecipt").GetComponent<LoopListView2>();

	}
}