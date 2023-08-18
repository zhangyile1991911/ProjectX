using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.PhoneWindow,"Assets/GameRes/Prefabs/Windows/PhoneWindow.prefab")]
public partial class PhoneWindow : UIWindow
{
	public Transform Tran_Bottom;
	public Transform Tran_AppGroup;
	public Transform Tran_AppRun;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_Bottom = go.transform.Find("Tran_Bottom").GetComponent<Transform>();
		Tran_AppGroup = go.transform.Find("Tran_Bottom/Tran_AppGroup").GetComponent<Transform>();
		Tran_AppRun = go.transform.Find("Tran_Bottom/Tran_AppRun").GetComponent<Transform>();

	}
}