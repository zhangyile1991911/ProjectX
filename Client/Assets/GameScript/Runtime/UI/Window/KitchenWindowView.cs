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
	public Transform Ins_CookToolIconA;
	public Transform Ins_CookToolIconB;
	public Transform Ins_CookToolIconC;
	public Transform Ins_CookToolIconD;
	public Transform Ins_CookToolIconF;
	public TextMeshProUGUI Txt_num;

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
		Ins_CookToolIconA = go.transform.Find("Tran_Center/Ins_CookToolIconA").GetComponent<Transform>();
		Ins_CookToolIconB = go.transform.Find("Tran_Center/Ins_CookToolIconB").GetComponent<Transform>();
		Ins_CookToolIconC = go.transform.Find("Tran_Center/Ins_CookToolIconC").GetComponent<Transform>();
		Ins_CookToolIconD = go.transform.Find("Tran_Center/Ins_CookToolIconD").GetComponent<Transform>();
		Ins_CookToolIconF = go.transform.Find("Tran_Center/Ins_CookToolIconF").GetComponent<Transform>();
		Txt_num = go.transform.Find("HideNode/FoodBtnCell/Txt_num").GetComponent<TextMeshProUGUI>();

	}
}