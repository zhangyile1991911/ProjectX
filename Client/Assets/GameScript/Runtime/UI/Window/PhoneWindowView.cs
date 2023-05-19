using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.PhoneWindow,"Assets/GameRes/Prefabs/Windows/PhoneWindow.prefab")]
public partial class PhoneWindow : UIWindow
{
	public Transform Tran_Bottom;
	public Button Btn_Home;
	public Transform Tran_AppGroup;
	public Transform Tran_AppRun;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_Bottom = go.transform.Find("Tran_Bottom").GetComponent<Transform>();
		Btn_Home = go.transform.Find("Tran_Bottom/Btn_Home").GetComponent<Button>();
		Tran_AppGroup = go.transform.Find("Tran_AppGroup").GetComponent<Transform>();
		Tran_AppRun = go.transform.Find("Tran_AppRun").GetComponent<Transform>();

	}
}