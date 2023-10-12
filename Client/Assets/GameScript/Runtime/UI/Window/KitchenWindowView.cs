using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;

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
	public ToggleGroup ToggleG_Flavor;
	public Toggle Toggle_A;
	public Toggle Toggle_B;
	public Toggle Toggle_C;
	public Toggle Toggle_D;
	public TextMeshProUGUI Txt_num;
	public Transform Tran_refrigerator;
	public Button Btn_A;
	public Button Btn_B;
	public Button Btn_C;
	public Button Btn_D;
	public LoopGridView Grid_Refrigerator;
	public Image Img_highLight;
	public Image Img_Item;
	public TextMeshProUGUI Txt_Num;
	public TextMeshProUGUI Txt_Name;
	public Button Btn_order;
	public Button Btn_Arrow;

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
		ToggleG_Flavor = go.transform.Find("Tran_Center/ToggleG_Flavor").GetComponent<ToggleGroup>();
		Toggle_A = go.transform.Find("Tran_Center/ToggleG_Flavor/Toggle_A").GetComponent<Toggle>();
		Toggle_B = go.transform.Find("Tran_Center/ToggleG_Flavor/Toggle_B").GetComponent<Toggle>();
		Toggle_C = go.transform.Find("Tran_Center/ToggleG_Flavor/Toggle_C").GetComponent<Toggle>();
		Toggle_D = go.transform.Find("Tran_Center/ToggleG_Flavor/Toggle_D").GetComponent<Toggle>();
		Txt_num = go.transform.Find("HideNode/FoodBtnCell/Txt_num").GetComponent<TextMeshProUGUI>();
		Tran_refrigerator = go.transform.Find("Tran_refrigerator").GetComponent<Transform>();
		Btn_A = go.transform.Find("Tran_refrigerator/Btn_A").GetComponent<Button>();
		Btn_B = go.transform.Find("Tran_refrigerator/Btn_B").GetComponent<Button>();
		Btn_C = go.transform.Find("Tran_refrigerator/Btn_C").GetComponent<Button>();
		Btn_D = go.transform.Find("Tran_refrigerator/Btn_D").GetComponent<Button>();
		Grid_Refrigerator = go.transform.Find("Tran_refrigerator/Grid_Refrigerator").GetComponent<LoopGridView>();
		Img_highLight = go.transform.Find("Tran_refrigerator/Grid_Refrigerator/Viewport/Content/RefrigeratorCell/Img_highLight").GetComponent<Image>();
		Img_Item = go.transform.Find("Tran_refrigerator/Grid_Refrigerator/Viewport/Content/RefrigeratorCell/Img_Item").GetComponent<Image>();
		Txt_Num = go.transform.Find("Tran_refrigerator/Grid_Refrigerator/Viewport/Content/RefrigeratorCell/Txt_Num").GetComponent<TextMeshProUGUI>();
		Txt_Name = go.transform.Find("Tran_refrigerator/Grid_Refrigerator/Viewport/Content/RefrigeratorCell/Txt_Name").GetComponent<TextMeshProUGUI>();
		Btn_order = go.transform.Find("Btn_order").GetComponent<Button>();
		Btn_Arrow = go.transform.Find("Btn_Arrow").GetComponent<Button>();

	}
}