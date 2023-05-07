using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Yarn.Unity;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Markup;
using YooAsset;

public class DialoguePortraitView : DialogueViewBase
{
    [SerializeField]
    public Image CharacterPortrait;
    
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        HandleMarkup(dialogueLine.Text.Attributes,onDialogueLineFinished);
    }
    
    private void HandleMarkup(List<MarkupAttribute> attributes,Action onDialogueLineFinish)
    {
        foreach (var markupAttribute in attributes)
        {
            if(markupAttribute.Name != "character")
            {
                continue;
            }

            var properties = markupAttribute.Properties;
            if (!properties.ContainsKey("name"))
            {
                continue;
            }
            
            var characterName = properties["name"].StringValue;
            var spName = ZString.Concat("Assets/Art/Character/Portrait/",characterName, "Portrait.png");
            var loadSpriteHandle = YooAssets.LoadAssetAsync<Sprite>(spName);
            loadSpriteHandle.Completed += sp =>
            {
                CharacterPortrait.sprite = sp.AssetObject as Sprite;
                CharacterPortrait.gameObject.SetActive(true);
                onDialogueLineFinish();
            };
        }
    }
    
}