using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using UnityEngine.UI;
using Yarn.Markup;


public class ShopKeeperLineView : DialogueViewBase
{
    [SerializeField]
    internal CanvasGroup canvasGroup;
    
    [SerializeField]
    internal TextMeshProUGUI lineText = null;

    [SerializeField]
    internal ButtonLongPress backgroundBtn = null;
    
    [SerializeField]
    internal bool useTypewriterEffect = false;
    
    [SerializeField]
    internal UnityEngine.Events.UnityEvent onCharacterTyped;
    
    [SerializeField]
    [Min(0)]
    internal float typewriterEffectSpeed = 0f;
    
    [SerializeField]
    [Min(0)]
    internal float holdTime = 1f;
    
    [SerializeField]
    internal bool autoAdvance = false;

    [SerializeField]
    internal bool customer = true;
    //用来保存当前正在显示的对话内容
    LocalizedLine currentLine = null;
    private CancellationTokenSource fadeEffectCts;
    private CancellationTokenSource typewriteCts;

    public Action<string> DialogueLineComplete;
    public Action<string> DialogueLineStar;
    
    public bool PlayTyping => isTyping;
    //是否在演出打字机效果
    private bool isTyping = false;
    private RectTransform _rectTransform;
    private RectTransform _backgroundRectTransform;
    private ContentSizeFitter _contentSizeFitter;
    
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        _rectTransform = GetComponent<RectTransform>();
        _backgroundRectTransform = backgroundBtn.GetComponent<RectTransform>();
        _contentSizeFitter = lineText.GetComponent<ContentSizeFitter>();
    }

    private bool isShopKeeper;
    public override async void RunLine(LocalizedLine dialogueLine,Action onDialogueLineFinished)
    {
        // cts?.Dispose();
        // cts = new CancellationTokenSource();
        isShopKeeper = dialogueLine.CharacterName.Contains("老板");
        canvasGroup.alpha = isShopKeeper ? 1 : 0;
        if (!isShopKeeper) return;
        AdjustDialoguePosition(dialogueLine.CharacterName);
        await RunLineInternal(dialogueLine,onDialogueLineFinished);
    }

    private void AdjustDialoguePosition(string characterName)
    {
        var characterObj = CharacterMgr.Instance.GetCharacterByName(characterName);
        var positionToUI= UIManager.Instance.WorldPositionToUI(characterObj.ChatNode.position);
        _backgroundRectTransform.pivot = new Vector2(0.25f, 0f);
        _backgroundRectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.anchoredPosition = positionToUI;
    }
    
    private async UniTask RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        Debug.Log("######RunLineInternal######");
        currentLine = dialogueLine;
        
        DialogueLineStar?.Invoke(dialogueLine.TextID);
        
        //开始文字演出
        await PresentLine(dialogueLine);
        isTyping = false;
        
        //文字演出结束
        lineText.maxVisibleCharacters = int.MaxValue;
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        DialogueLineComplete?.Invoke(dialogueLine.TextID);
        
        //是否等待
        if (holdTime > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(holdTime));
        }
        
        //是否自动阅读
        if (!autoAdvance)
        {
            return;
        }

        onDialogueLineFinished();
        Debug.Log("------RunLineInternal------");
    }

    private const int MaxCharacterInLine = 10;
    private async UniTask PresentLine(LocalizedLine dialogueLine)
    {
        lineText.gameObject.SetActive(true);
        canvasGroup.gameObject.SetActive(true);
        
        if (useTypewriterEffect)
        {
            lineText.maxVisibleCharacters = 0;
        }
        else
        {
            lineText.maxVisibleCharacters = int.MaxValue;
        }

        if (dialogueLine.TextWithoutCharacterName.Text.Length >= MaxCharacterInLine)
        {
            _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            Debug.Log($"lineText.rectTransform.sizeDelta = {lineText.rectTransform.sizeDelta}");
            Debug.Log($"lineText.rectTransform.rect = {lineText.rectTransform.rect}");
            var oldSizeDelta = lineText.rectTransform.sizeDelta;
            oldSizeDelta.x = MaxCharacterInLine * lineText.fontSize;
            lineText.rectTransform.sizeDelta = oldSizeDelta;
        }
        else
        {
            _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        // Debug.Log($"Text.Length = {dialogueLine.TextWithoutCharacterName.Text.Length}");
        
        lineText.text = dialogueLine.TextWithoutCharacterName.Text;
        await UniTask.NextFrame();
        
        //调整背景图大小
        var sizeDelta = lineText.rectTransform.sizeDelta;
        sizeDelta.x += 60f;
        sizeDelta.y += 60f;
        _backgroundRectTransform.sizeDelta = sizeDelta;
        
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        
        await EffectsAsync.ShowFrameBounce(_backgroundRectTransform,1f,0.8f);
        
        Debug.Log($"ShopKeeperLineView::PresentLine lineText.sizeDelta = {lineText.rectTransform.sizeDelta} ");
        Debug.Log($"_backgroundRectTransform.sizeDelta = {sizeDelta} ");
        
        
        if (useTypewriterEffect)
        {
            Debug.Log($"开始演出打字机效果");
            isTyping = true;
            
            typewriteCts = new();
            // Debug.Log($"演出打字机效果");
            await EffectsAsync.Typewriter(
                lineText,
                ()=>typewriterEffectSpeed,
                () => onCharacterTyped.Invoke(),
                typewriteCts);
            Debug.Log($"结束演出打字机效果");
        }
        
    }

    public override void DismissLine(Action onDismissalComplete)
    {
        currentLine = null;
        // Debug.Log($"当前文字展示结束");
        DismissLineInternal(onDismissalComplete);
    }

    private async void DismissLineInternal(Action onDismissComplete)
    {
        canvasGroup.interactable = false;

        await EffectsAsync.PrepareNextWord(_backgroundRectTransform, lineText,0.3f, 0.5f);
        
        Debug.Log($"结束DismissLineInternal当前文字展示渐出效果");
        
        onDismissComplete();
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (!isShopKeeper)
        {
            onDialogueLineFinished();
            return;
        }
            
        
        Debug.Log($"######ShopKeeperLineView当前文字打断######");
        currentLine = dialogueLine;

        lineText.gameObject.SetActive(true);
        canvasGroup.gameObject.SetActive(true);

        lineText.text = dialogueLine.TextWithoutCharacterName.Text; 
        lineText.maxVisibleCharacters = dialogueLine.TextWithoutCharacterName.Text.Length;
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (isTyping)
        {
            isTyping = false;
            return;
        }

        if (fadeEffectCts is {IsCancellationRequested: true})
        {
            fadeEffectCts.Cancel();
            fadeEffectCts = null;    
        }

        if (typewriteCts is {IsCancellationRequested: true})
        {
            typewriteCts.Cancel();
            typewriteCts = null;    
        }

        onDialogueLineFinished();
        Debug.Log($"-----当前文字打断-----");
    }
    
    public override void UserRequestedViewAdvancement()
    {
        if (currentLine == null)
            return;
        Debug.Log("用户前进 ShopKeeperLineView UserRequestedViewAdvancement()");
        
        if (fadeEffectCts is { IsCancellationRequested: true })
        {
            fadeEffectCts.Cancel();
            fadeEffectCts = null;    
        }

        if (typewriteCts is {IsCancellationRequested: true})
        {
            typewriteCts.Cancel();
            typewriteCts = null;    
        }
        
        requestInterrupt?.Invoke();
    }

    public override void DialogueComplete()
    {
        Debug.Log($"ShopKeeperLineView::DialogueComplete()");
        
    }
}
