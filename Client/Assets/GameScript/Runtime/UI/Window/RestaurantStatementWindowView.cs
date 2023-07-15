using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.RestaurantStatementWindow,"Assets/GameRes/Prefabs/Windows/RestaurantStatementWindow.prefab")]
public partial class RestaurantStatementWindow : UIWindow
{
	public Button Btn_bg;
	public LoopGridView Grid_StatementList;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Btn_bg = go.transform.Find("Btn_bg").GetComponent<Button>();
		Grid_StatementList = go.transform.Find("Grid_StatementList").GetComponent<LoopGridView>();

	}
}