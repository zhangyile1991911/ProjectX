using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using Yarn.Markup;
using YooAsset;
using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using DG.DemiEditor;
using DG.Tweening;
using UnityEditor;

public class EmojiView : DialogueViewBase
{
    public Image Img_Emoji;

    public RectTransform Rect_Emoji;
    private AssetOperationHandle handle;

    private Tween _tween;
    // Start is called before the first frame update
    void Start()
    {
        Img_Emoji.gameObject.SetActive(false);
    }
    

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        string emojiType="";
        string characterName="";
        int emojiSeqCount = 0;
        if (dialogueLine.Text.TryGetAttributeWithName("character",out var charAttribute))
        {
            characterName = charAttribute.Properties["name"].StringValue;
        }
        if(dialogueLine.Text.TryGetAttributeWithName("emoji",out var emojiAttribute))
        {
            emojiType = emojiAttribute.Properties["emoji"].StringValue;
        }

        if (dialogueLine.Text.TryGetAttributeWithName("emoji_seq", out var emojiSeqattribute))
        {
            emojiSeqCount = emojiSeqattribute.Properties["emoji_seq"].IntegerValue;
        }

        if (characterName.IsNullOrEmpty()) return;
        
        if (!emojiType.IsNullOrEmpty())
        {
            ShowEmoji(emojiType,characterName,onDialogueLineFinished);    
        }

        if (emojiSeqCount > 0)
        {
            DoEmojiSeq(emojiSeqCount,emojiSeqattribute.Properties);
        }
    }
    
    public override void DismissLine(Action onDismissalComplete)
    {
        Img_Emoji.gameObject.SetActive(false);
        Img_Emoji.sprite = null;
        handle?.Release();
        handle = null;
        _tween?.Kill(false);
        onDismissalComplete();
    }

    private Vector2 worldPositionToUI(string characterName)
    {
        var mgr = UniModule.GetModule<CharacterMgr>();
        var chr = mgr.GetCharacterByName(characterName);
        //将世界坐标转换到UI坐标
        var uiCamera = UIManager.Instance.UICamera;
        var uiCanvas = UIManager.Instance.RootCanvas;
        var screenPosition = uiCamera.WorldToScreenPoint(chr.EmojiWorldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.GetComponent<RectTransform>(),screenPosition,uiCamera,out var localPos);
        return localPos;
    }
    private async void ShowEmoji(string emojiType,string characterName,Action onDialogueLineFinished)
    {
        Vector2 localPos = worldPositionToUI(characterName);
        Debug.Log($"ShowEmoji pos = {localPos}");
        switch (emojiType)
        {
            case "whistle":
                handle= YooAssets.LoadAssetAsync<Sprite>("Assets/GameRes/Picture/Story/emoji/music.png");
                await handle.ToUniTask();
                var emojiSp = handle.AssetObject as Sprite;
                Img_Emoji.sprite = emojiSp;
                Img_Emoji.gameObject.SetActive(true);
                
                Rect_Emoji.anchoredPosition = localPos;
                localPos.y += 20f;
                _tween = Rect_Emoji
                    .DOLocalJump(localPos,5,2,2.5f)
                    .SetLoops(-1,LoopType.Yoyo);
                break;
            case "sleep":
                handle = YooAssets.LoadAssetAsync<Sprite>("Assets/GameRes/Picture/Story/emoji/sleep.png");
                await handle.ToUniTask();
                var emojiSpSleep = handle.AssetObject as Sprite;
                Img_Emoji.sprite = emojiSpSleep;
                Img_Emoji.gameObject.SetActive(true);

                Rect_Emoji.anchoredPosition = localPos;
                localPos.y += 20f;
                _tween = Rect_Emoji
                    .DOLocalJump(localPos, 5, 2, 2.5f)
                    .SetLoops(-1, LoopType.Yoyo);
                break;

        }

        onDialogueLineFinished();
    }

    private async void DoEmojiSeq(int total,IReadOnlyDictionary<string,MarkupValue> prop)
    {
        
    }
}
