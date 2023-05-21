using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.KitchenWindow,"Assets/GameRes/Prefabs/Windows/KitchenWindow.prefab")]
public partial class KitchenWindow : UIWindow
{
	public Transform Tran_Bottom;
	public Button Btn_produce;
	public Transform Ins_FoodIconA;
	public Transform Ins_FoodIconB;
	public Transform Ins_FoodIconC;
	public Transform Ins_FoodIconD;
	public Transform Ins_FoodIconE;
	public Transform Ins_MenuIcon1;
	public Transform Ins_MenuIcon2;
	public Transform Ins_MenuIcon3;
	public Transform Ins_MenuIcon4;
	public Transform Ins_MenuIcon5;
	public Transform Ins_MenuIcon6;
	public Transform Ins_MenuIcon7;
	public Transform Ins_MenuIcon8;
	public Transform Tran_RightTop;
	public Transform Tran_tagGroup;
	public Button Btn_ClassifyA;
	public Button Btn_ClassifyB;
	public Button Btn_ClassifyC;
	public Button Btn_ClassifyD;
	public LoopGridView Grid_FoodItem;
	public Transform Tran_Center;
	public Button Btn_ToolA;
	public Button Btn_ToolB;
	public Button Btn_ToolC;
	public Button Btn_ToolD;
	public Button Btn_ToolE;

	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Tran_Bottom = go.transform.Find("Tran_Bottom").GetComponent<Transform>();
		Btn_produce = go.transform.Find("Tran_Bottom/Btn_produce").GetComponent<Button>();
		Ins_FoodIconA = go.transform.Find("Tran_Bottom/FoodBg/Ins_FoodIconA").GetComponent<Transform>();
		Ins_FoodIconB = go.transform.Find("Tran_Bottom/FoodBg/Ins_FoodIconB").GetComponent<Transform>();
		Ins_FoodIconC = go.transform.Find("Tran_Bottom/FoodBg/Ins_FoodIconC").GetComponent<Transform>();
		Ins_FoodIconD = go.transform.Find("Tran_Bottom/FoodBg/Ins_FoodIconD").GetComponent<Transform>();
		Ins_FoodIconE = go.transform.Find("Tran_Bottom/FoodBg/Ins_FoodIconE").GetComponent<Transform>();
		Ins_MenuIcon1 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon1").GetComponent<Transform>();
		Ins_MenuIcon2 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon2").GetComponent<Transform>();
		Ins_MenuIcon3 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon3").GetComponent<Transform>();
		Ins_MenuIcon4 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon4").GetComponent<Transform>();
		Ins_MenuIcon5 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon5").GetComponent<Transform>();
		Ins_MenuIcon6 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon6").GetComponent<Transform>();
		Ins_MenuIcon7 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon7").GetComponent<Transform>();
		Ins_MenuIcon8 = go.transform.Find("Tran_Bottom/MenuBg/Ins_MenuIcon8").GetComponent<Transform>();
		Tran_RightTop = go.transform.Find("Tran_RightTop").GetComponent<Transform>();
		Tran_tagGroup = go.transform.Find("Tran_RightTop/Tran_tagGroup").GetComponent<Transform>();
		Btn_ClassifyA = go.transform.Find("Tran_RightTop/Tran_tagGroup/Btn_ClassifyA").GetComponent<Button>();
		Btn_ClassifyB = go.transform.Find("Tran_RightTop/Tran_tagGroup/Btn_ClassifyB").GetComponent<Button>();
		Btn_ClassifyC = go.transform.Find("Tran_RightTop/Tran_tagGroup/Btn_ClassifyC").GetComponent<Button>();
		Btn_ClassifyD = go.transform.Find("Tran_RightTop/Tran_tagGroup/Btn_ClassifyD").GetComponent<Button>();
		Grid_FoodItem = go.transform.Find("Tran_RightTop/Grid_FoodItem").GetComponent<LoopGridView>();
		Tran_Center = go.transform.Find("Tran_Center").GetComponent<Transform>();
		Btn_ToolA = go.transform.Find("Tran_Center/Btn_ToolA").GetComponent<Button>();
		Btn_ToolB = go.transform.Find("Tran_Center/Btn_ToolB").GetComponent<Button>();
		Btn_ToolC = go.transform.Find("Tran_Center/Btn_ToolC").GetComponent<Button>();
		Btn_ToolD = go.transform.Find("Tran_Center/Btn_ToolD").GetComponent<Button>();
		Btn_ToolE = go.transform.Find("Tran_Center/Btn_ToolE").GetComponent<Button>();

	}
}