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
        HandleMarkup(dialogueLine.Text,onDialogueLineFinished);
    }
    
    private void HandleMarkup(MarkupParseResult text,Action onDialogueLineFinish)
    {
        // if (text.TryGetAttributeWithName("character", out var attribute))
        // {
        //     if (attribute.Properties.ContainsKey("name"))
        //     {
        //         var characterName = attribute.Properties["name"].StringValue;
        //         // loadPortrait(characterName,onDialogueLineFinish);
        //     }
        // }
    }

    private void loadPortrait(string characterName,Action onDialogueLineFinish)
    {
        // var mgr = UniModule.GetModule<CharacterMgr>();
        // switch (characterName)
        // {
        //     case "老板":
        //         CharacterPortrait.gameObject.SetActive(true);
        //         var bossHandle = YooAssets.LoadAssetAsync<Sprite>("Assets/GameRes/Picture/Character/Portrait/MerchantPortrait.png");
        //         bossHandle.Completed += sp =>
        //         {
        //             CharacterPortrait.sprite = sp.AssetObject as Sprite;
        //             CharacterPortrait.gameObject.SetActive(true);
        //             onDialogueLineFinish();
        //         };
        //         break;
        //     case "旁白":
        //         CharacterPortrait.gameObject.SetActive(false);
        //         break;
        //     default:
        //         var chr = mgr.GetCharacterByName(characterName);
        //         var showPortrait = chr != null;
        //         CharacterPortrait.gameObject.SetActive(showPortrait);
        //         if (showPortrait)
        //         {
        //             var loadSpriteHandle = YooAssets.LoadAssetAsync<Sprite>(chr.TBBaseInfo.PortraitPath);
        //             loadSpriteHandle.Completed += sp =>
        //             {
        //                 CharacterPortrait.sprite = sp.AssetObject as Sprite;
        //                 CharacterPortrait.gameObject.SetActive(true);
        //                 onDialogueLineFinish();
        //             };    
        //         }
        //         break;
        // }
        
    }
    
}
