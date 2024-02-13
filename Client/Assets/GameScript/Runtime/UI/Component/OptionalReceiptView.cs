using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/OptionalReceipt.prefab")]
public partial class OptionalReceipt : UIComponent
{
	public Image Img_tool;
	public Transform Tran_material;
	public Image Img_receipt;
	public TextMeshProUGUI Txt_name;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Img_tool = go.transform.Find("Img_tool").GetComponent<Image>();
		Tran_material = go.transform.Find("Tran_material").GetComponent<Transform>();
		Img_receipt = go.transform.Find("Img_receipt").GetComponent<Image>();
		Txt_name = go.transform.Find("Txt_name").GetComponent<TextMeshProUGUI>();

	}
}