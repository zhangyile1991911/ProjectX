﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.OrderQueueWindow,"Assets/GameRes/Prefabs/Windows/OrderQueueWindow.prefab")]
public partial class OrderQueueWindow : UIWindow
{
	public Transform Tran_Queue;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_Queue = go.transform.Find("Tran_Queue").GetComponent<Transform>();

	}
}