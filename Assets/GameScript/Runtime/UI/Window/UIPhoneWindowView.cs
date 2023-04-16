using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.PhoneWindow,"Assets/GameRes/Prefabs/UIPhoneWindow.prefab")]
public partial class PhoneWindow : UIWindow
{
	public Transform Tran_Bottom;
	public Transform Tran_AppGroup;
	public Image Img_Icon;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_Bottom = go.transform.Find("Tran_Bottom").GetComponent<Transform>();
		Tran_AppGroup = go.transform.Find("Tran_AppGroup").GetComponent<Transform>();
		Img_Icon = go.transform.Find("Tran_AppGroup/App/Img_Icon").GetComponent<Image>();

	}
}