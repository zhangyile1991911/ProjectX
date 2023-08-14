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
	public Image Img_bg1;
	public Image Img_bg2;
	public Image Img_Player;
	public Image Img_Touch;
	public Transform Tran_SpawnPoint;
	public Transform Tran_A;
	public Transform Tran_B;
	public Transform Tran_C;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_bg1 = go.transform.Find("Img_bg1").GetComponent<Image>();
		Img_bg2 = go.transform.Find("Img_bg2").GetComponent<Image>();
		Img_Player = go.transform.Find("Img_Player").GetComponent<Image>();
		Img_Touch = go.transform.Find("Img_Touch").GetComponent<Image>();
		Tran_SpawnPoint = go.transform.Find("Tran_SpawnPoint").GetComponent<Transform>();
		Tran_A = go.transform.Find("Tran_SpawnPoint/Tran_A").GetComponent<Transform>();
		Tran_B = go.transform.Find("Tran_SpawnPoint/Tran_B").GetComponent<Transform>();
		Tran_C = go.transform.Find("Tran_SpawnPoint/Tran_C").GetComponent<Transform>();

	}
}