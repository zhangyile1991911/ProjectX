using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using YooAsset;

public class CharacterDialogData : UIOpenParam
{
    public string StoryResPath;
    public string StoryStartNode;
    public Action StoryComplete;
    public RestaurantCharacter StoryCharacter;
}
/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class CharacterDialogWindow : UIWindow
{
    public float typewriterEffectSpeed;
    public DialogueRunner DialogueRunner => _dialogueRunner;
    private DialogueRunner _dialogueRunner;
    private PortraitLineView _portraitLineView;
    private CharacterDialogData _openData;
    private ComicView _comicView;
    public override void OnCreate()
    {
        base.OnCreate();
        _dialogueRunner = uiTran.GetComponent<DialogueRunner>();
    }

    public override void OnDestroy()
    {
        _dialogueRunner.onDialogueComplete.RemoveAllListeners();
        _dialogueRunner.onNodeComplete.RemoveAllListeners();
        base.OnDestroy();
    }
    
    public override async void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);

        _openData = openParam as CharacterDialogData;
        var handler =  YooAssets.LoadAssetAsync<YarnProject>(_openData.StoryResPath);
        await handler.ToUniTask();

        var yp = handler.AssetObject as YarnProject;
        _dialogueRunner.SetProject(yp);
        
        _dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
        _dialogueRunner.onNodeComplete.AddListener(OnNodeComplete);
        
        // var dm = UniModule.GetModule<DialogueModule>();
        // dm.CurentDialogueRestaurantCharacter.InjectVariable(_dialogueRunner.VariableStorage);
        _openData.StoryCharacter.InjectVariable(_dialogueRunner.VariableStorage);

        foreach (var oneView in _dialogueRunner.dialogueViews)
        {
            if (oneView.GetType() == typeof(PortraitLineView))
            {
                _portraitLineView = oneView as PortraitLineView;
                break;
            }
        }
        
        _dialogueRunner.StartDialogue(_openData.StoryStartNode);
        
        LBtn_Background.onClick.AddListener(NextLine);
        
        // _portraitLineView.autoAdvance = true;
        // _portraitLineView.typewriterEffectSpeed = typewriterEffectSpeed;
    }

    public override void OnHide()
    {
        base.OnHide();
        _dialogueRunner.onDialogueComplete.RemoveAllListeners();
        _dialogueRunner.onNodeComplete.RemoveAllListeners();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnDialogueComplete()
    {
        Debug.Log($"OnDialogueComplete");
        _openData?.StoryComplete?.Invoke();
    }

    private void OnNodeComplete(string nodeName)
    {
        Debug.Log($"nodeName = ${nodeName}");
    }

    private void NextLine()
    {
        _portraitLineView.UserRequestedViewAdvancement();
    }
}