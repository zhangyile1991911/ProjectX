using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI((int)UIEnum.CharacterDialogWindow,"Assets/GameRes/Prefabs/Windows/CharacterDialogWindow.prefab")]
public partial class CharacterDialogWindow : UIWindow
{
	public DialogueRunner DialogueRunner;
	public InMemoryVariableStorage VariableStorage;
	public OptionsListView OptionsListView;
	public override void Init(GameObject go)
	{
	    uiGo = go;

	    DialogueRunner = go.transform.Find("LineView").GetComponent<DialogueRunner>();
	    VariableStorage = go.transform.Find("LineView").GetComponent<InMemoryVariableStorage>();
	    OptionsListView = go.transform.Find("Options List View").GetComponent<OptionsListView>();
	}
}