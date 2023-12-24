using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Febucci.UI;
using UnityEngine.UI;
using Yarn.Markup;

public static class EffectsAsync
{
    public static async UniTask FadeAlpha(CanvasGroup canvasGroup, float from, float to, float fadeTime,CancellationTokenSource cts)
    {
        canvasGroup.alpha = from;
        var timeElapse = 0f;
        while (timeElapse < fadeTime)
        {
            // Debug.Log($"进行alpha动画");
            // return;
            var faction = timeElapse / fadeTime;
            timeElapse += Time.deltaTime;
            var a = Mathf.Lerp(from, to, faction);
            canvasGroup.alpha = a;
            await UniTask.NextFrame(cancellationToken:cts.Token);
        }

        canvasGroup.alpha = to;
        if (to == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    
    
    public static async UniTask Typewriter(TextAnimator_TMP text, Func<float> lettersPerSecond, Action onCharacterType,CancellationTokenSource cts)
    {
        // Debug.Log($"开始运行 打字机效果");
        text.TMProComponent.maxVisibleCharacters = 0;
        
        await UniTask.NextFrame(cancellationToken:cts.Token);

        var characterCount = text.TMProComponent.textInfo.characterCount;//text.textInfo.characterCount;
        if (lettersPerSecond() <= 0 || characterCount == 0)
        {
            // text.maxVisibleCharacters = characterCount;
            text.TMProComponent.maxVisibleCharacters = characterCount;
            return;
        }
        
        // Debug.Log($"每一个字需要 {secondsPerLetter}秒显示");
        var accumulator = Time.deltaTime;
        while (text.TMProComponent.maxVisibleCharacters < characterCount)
        {
            if (cts.IsCancellationRequested)
            {
                return;
            }
            float secondsPerLetter = 1.0f / lettersPerSecond();
            while (accumulator >= secondsPerLetter)
            {
                text.TMProComponent.maxVisibleCharacters += 1;
                onCharacterType?.Invoke();
                accumulator -= secondsPerLetter;
                // Debug.Log($"当前显示了 {text.maxVisibleCharacters}个字");
            }
            accumulator += Time.deltaTime;
            await UniTask.NextFrame(cancellationToken:cts.Token);
        }

        text.TMProComponent.maxVisibleCharacters = characterCount;
    }
    
    public static async UniTask ShowFrameBounce(RectTransform rectTransform,float to,float time)
    {
        rectTransform.DOScale(to, time).SetEase(Ease.OutBack);
        await UniTask.Delay(TimeSpan.FromSeconds(time*0.4f));
    }

    public static async UniTask PrepareNextWord(RectTransform rectTransform,TextMeshProUGUI  lineText,float to,float time)
    {
        rectTransform.DOScale(to, time).SetEase(Ease.InBack);
        var pre = time * 0.4f;
        var remain = time - pre;
        await UniTask.Delay(TimeSpan.FromSeconds(time * 0.4f));
        lineText.text = null;
        lineText.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(remain));
    }
}

public class PortraitLineView : DialogueViewBase
{
    [SerializeField]
    internal CanvasGroup canvasGroup;
    
    [SerializeField]
    internal TextMeshProUGUI lineText = null;

    [SerializeField]
    internal TextAnimator_TMP TextAnimator = null;

    [SerializeField]
    internal ButtonLongPress backgroundBtn = null;

    [SerializeField]
    internal bool useFadeEffect = true;
    
    [SerializeField]
    [Min(0)]
    internal float fadeInTime = 0.25f;
    
    [SerializeField]
    [Min(0)]
    internal float fadeOutTime = 0.05f;
    
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
    // private CancellationTokenSource frameEffectCts;

    public Action<string> DialogueLineComplete;
    public Action<string> DialogueLineStar;
    
    public bool PlayTyping => isTyping;
    //是否在演出打字机效果
    private bool isTyping = false;
    private RectTransform _rectTransform;
    private RectTransform _backgroundRectTransform;
    private ContentSizeFitter _contentSizeFitter;
    private LocalizedLine _previousLine;
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        _rectTransform = GetComponent<RectTransform>();
        _backgroundRectTransform = backgroundBtn.GetComponent<RectTransform>();
        _contentSizeFitter = lineText.GetComponent<ContentSizeFitter>();
    }

    private void HandleMarkup(MarkupParseResult attributes)
    {
        if (attributes.TryGetAttributeWithName("mood", out var mood))
        {
            MoodAnimation(mood.Properties);
        }
        else if (attributes.TryGetAttributeWithName("dialogue_wave",out var dialogue))
        {
            DialogueAnimation(dialogue.Properties);    
        }
    }

    private void MoodAnimation(IReadOnlyDictionary<string,MarkupValue> properties)
    {
        var str = properties["mood"].StringValue;
        switch (str)
        {
            case "angry":
                LineAngryAnimation();
                break;
        }
    }

    private Tweener dialogueWave;
    private void DialogueAnimation(IReadOnlyDictionary<string,MarkupValue> properties)
    {
        var waveVal = properties["dialogue_wave"].IntegerValue;
        dialogueWave = _rectTransform.DOShakePosition(1, new Vector3(1, waveVal, 0), 10, 90f, false, true);
    }

    private Tween lineDoingTween;
    private void LineAngryAnimation()
    {
        if (lineDoingTween == null)
        {
            lineDoingTween = lineText.rectTransform.DOPunchPosition(new Vector3(20, 20, 1), 2.5f, 20, 1f);    
        }
        else
        {
            lineDoingTween.Restart();
        }
    }

    private bool isCustomer;
    public override async void RunLine(LocalizedLine dialogueLine,Action onDialogueLineFinished)
    {
        // cts?.Dispose();
        // cts = new CancellationTokenSource();
        isCustomer = !dialogueLine.CharacterName.Contains("老板");
        canvasGroup.alpha = isCustomer ? 1 : 0;
        lineText.gameObject.SetActive(isCustomer);
        if (isCustomer)
        {
            canvasGroup.alpha = 1;
            HandleMarkup(dialogueLine.Text);
            AdjustDialoguePosition(dialogueLine.CharacterName);
            await RunLineInternal(dialogueLine,onDialogueLineFinished);        
        }
    }

    private void AdjustDialoguePosition(string characterName)
    {
        var characterObj = CharacterMgr.Instance.GetCharacterByName(characterName);
        var positionToUI= UIManager.Instance.WorldPositionToUI(characterObj.ChatNode.position);
        switch (characterObj.SeatOccupy)
        {
            case 0:
            case 1:
                _backgroundRectTransform.pivot = new Vector2(0.25f, 0f);
                _backgroundRectTransform.anchoredPosition = Vector2.zero;
                break;
            case 2:
            case 3:
                _backgroundRectTransform.pivot = new Vector2(0.75f, 0f);
                _backgroundRectTransform.anchoredPosition = Vector2.zero;
                break;
        }
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
        
        // lineText.text = dialogueLine.TextWithoutCharacterName.Text;
        TextAnimator.TMProComponent.text = dialogueLine.TextWithoutCharacterName.Text;
        await UniTask.NextFrame();
        
        //调整背景图大小
        var sizeDelta = lineText.rectTransform.sizeDelta;
        // sizeDelta.x *= 1.3f;
        // sizeDelta.y *= 1.5f;
        sizeDelta.x += 30f;
        sizeDelta.y += 30f;
        _backgroundRectTransform.sizeDelta = sizeDelta;
        
        // if (useFadeEffect)
        // {//演出渐入效果
        //     Debug.Log($"开始演出渐入效果");
        //     fadeEffectCts = new ();
        //     await EffectsAsync.FadeAlpha(canvasGroup, 0, 0.5f, fadeInTime,fadeEffectCts);
        //     Debug.Log($"结束演出渐入效果");
        // }
        
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        
        await EffectsAsync.ShowFrameBounce(_backgroundRectTransform,1f,0.8f);
        
        Debug.Log($"PortraitLineView::PresentLine lineText.sizeDelta = {lineText.rectTransform.sizeDelta} ");
        Debug.Log($"_backgroundRectTransform.sizeDelta = {sizeDelta} ");
        
        
        if (useTypewriterEffect)
        {
            Debug.Log($"开始演出打字机效果");
            isTyping = true;
            // setting the canvas all back to its defaults because if we didn't also fade we don't have anything visible
            // canvasGroup.alpha = 1f;
            // canvasGroup.interactable = true;
            // canvasGroup.blocksRaycasts = true;
            
            typewriteCts = new();
            // Debug.Log($"演出打字机效果");
            await EffectsAsync.Typewriter(
                TextAnimator,//lineText,
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
        // var interactable = canvasGroup.interactable;
        canvasGroup.interactable = false;
        
        // if (useFadeEffect)
        // {
        //     Debug.Log($"开始DismissLineInternal当前文字展示渐出效果");
        //     fadeEffectCts = new ();
        //     await EffectsAsync.FadeAlpha(canvasGroup, 1, 0, fadeOutTime,fadeEffectCts);
        // }

        await EffectsAsync.PrepareNextWord(_backgroundRectTransform, lineText,0.3f, 0.5f);
        
        Debug.Log($"结束DismissLineInternal当前文字展示渐出效果");

        lineDoingTween?.Pause();
        
        dialogueWave?.Pause();
        dialogueWave = null;

        // canvasGroup.alpha = 0;
        // canvasGroup.blocksRaycasts = false;
        // canvasGroup.interactable = interactable;
        
        onDismissComplete();
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (!isCustomer)
        {
            onDialogueLineFinished();
            return;
        }
            
        Debug.Log($"######PortraitLineView当前文字打断######");
        
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

        // if (frameEffectCts.IsCancellationRequested)
        // {
        //     frameEffectCts.Cancel();
        //     frameEffectCts = null;
        // }

        onDialogueLineFinished();
        Debug.Log($"-----当前文字打断-----");
    }
    
    public override void UserRequestedViewAdvancement()
    {
        if (currentLine == null)
            return;
        Debug.Log("用户前进 PortraitLineView UserRequestedViewAdvancement()");
        
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
        Debug.Log($"PortraitLineView::DialogueComplete()");
        // runLineTask.SuppressCancellationThrow();
    }
}
