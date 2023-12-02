using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/AirplaneAppWidget.prefab")]
public partial class AirplaneAppWidget : BaseAppWidget
{
	public XButton XBtn_touch;
	public Transform Tran_SpawnPoint;
	public Transform Tran_A;
	public Transform Tran_B;
	public Transform Tran_C;
	public Transform Tran_Result;
	public TextMeshProUGUI Txt_Score;
	public XButton XBtn_Restart;
	public Transform Tran_Scroll;
	public Transform Tran_Bg2;
	public Transform Tran_Bg1;
	public Transform Tran_StartPage;
	public XButton XBtn_Start;

	public Transform Tran_UI;
	public Transform Tran_hpgroup;
	public TextMeshProUGUI Txt_curscore;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
	    XBtn_touch = go.transform.Find("XBtn_touch").GetComponent<XButton>();
		Tran_SpawnPoint = go.transform.Find("Tran_SpawnPoint").GetComponent<Transform>();
		Tran_A = go.transform.Find("Tran_SpawnPoint/Tran_A").GetComponent<Transform>();
		Tran_B = go.transform.Find("Tran_SpawnPoint/Tran_B").GetComponent<Transform>();
		Tran_C = go.transform.Find("Tran_SpawnPoint/Tran_C").GetComponent<Transform>();
		Tran_Result = go.transform.Find("Tran_Result").GetComponent<Transform>();
		Txt_Score = go.transform.Find("Tran_Result/Txt_Score").GetComponent<TextMeshProUGUI>();
		XBtn_Restart = go.transform.Find("Tran_Result/XBtn_Restart").GetComponent<XButton>();
		Tran_Scroll = go.transform.Find("Tran_Scroll").GetComponent<Transform>();
		Tran_Bg2 = go.transform.Find("Tran_Scroll/Tran_Bg2").GetComponent<Transform>();
		Tran_Bg1 = go.transform.Find("Tran_Scroll/Tran_Bg1").GetComponent<Transform>();
		Tran_StartPage = go.transform.Find("Tran_StartPage").GetComponent<Transform>();
		XBtn_Start = go.transform.Find("Tran_StartPage/XBtn_Start").GetComponent<XButton>();

		Tran_UI = go.transform.Find("Tran_UI").GetComponent<Transform>();
		Tran_hpgroup = go.transform.Find("Tran_UI/Tran_hpgroup").GetComponent<Transform>();
		Txt_curscore = go.transform.Find("Tran_UI/Txt_curscore").GetComponent<TextMeshProUGUI>();

	}
}