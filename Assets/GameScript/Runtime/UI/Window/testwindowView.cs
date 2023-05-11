using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.testwindow,"Assets/GameRes/Prefabs/Windows/testwindow.prefab")]
public partial class testwindow : UIWindow
{
	public Button Btn_tt1;
	public Image Img_tt1;
	public Image Img_tt2;
	public Button Btn_tt2;
	public Image Img_tt3;
	public Button Btn_tt4;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_tt1 = go.transform.Find("Btn_Img_tt1").GetComponent<Button>();
		Img_tt1 = go.transform.Find("Btn_Img_tt1").GetComponent<Image>();
		Img_tt2 = go.transform.Find("Img_Btn_tt2").GetComponent<Image>();
		Btn_tt2 = go.transform.Find("Img_Btn_tt2").GetComponent<Button>();
		Img_tt3 = go.transform.Find("Img_tt3").GetComponent<Image>();
		Btn_tt4 = go.transform.Find("Btn_tt4").GetComponent<Button>();

	}
}