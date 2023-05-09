using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
[UI(0,"Assets/GameRes/Prefabs/Components/ChatBubble.prefab")]
public partial class ChatBubble : UIComponent
{
	public TextMeshProUGUI Txt_content;


	public override void Init(GameObject go)
	{
	    uiGo = go;
	    
		Txt_content = go.transform.Find("Txt_content").GetComponent<TextMeshProUGUI>();

	}
}