using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    public static async UniTask Typewriter(TextMeshProUGUI text, Func<float> lettersPerSecond, Action onCharacterType,CancellationTokenSource cts)
    {
        // Debug.Log($"开始运行 打字机效果");
        text.maxVisibleCharacters = 0;
        await UniTask.NextFrame(cancellationToken:cts.Token);

        var characterCount = text.textInfo.characterCount;
        if (lettersPerSecond() <= 0 || characterCount == 0)
        {
            text.maxVisibleCharacters = characterCount;
            return;
        }
        
        // Debug.Log($"每一个字需要 {secondsPerLetter}秒显示");
        var accumulator = Time.deltaTime;
        while (text.maxVisibleCharacters < characterCount)
        {
            if (cts.IsCancellationRequested)
            {
                return;
            }
            float secondsPerLetter = 1.0f / lettersPerSecond();
            while (accumulator >= secondsPerLetter)
            {
                text.maxVisibleCharacters += 1;
                onCharacterType?.Invoke();
                accumulator -= secondsPerLetter;
                // Debug.Log($"当前显示了 {text.maxVisibleCharacters}个字");
            }
            accumulator += Time.deltaTime;
            await UniTask.NextFrame(cancellationToken:cts.Token);
        }

        text.maxVisibleCharacters = characterCount;
    }
}

public class PortraitLineView : DialogueViewBase
{
    [SerializeField]
    internal CanvasGroup canvasGroup;
    
    [SerializeField]
    internal TextMeshProUGUI lineText = null;

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
    
    //用来保存当前正在显示的对话内容
    LocalizedLine currentLine = null;
    private CancellationTokenSource cts;

    public Action<string> DialogueLineComplete;
    public Action<string> DialogueLineStar;
    
    public bool PlayTyping => isTyping;
    //是否在演出打字机效果
    private bool isTyping = false;
    
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    private void HandleMarkup(List<MarkupAttribute> attributes)
    {
        foreach (var markupAttribute in attributes)
        {
            switch (markupAttribute.Name)
            {
                case "mood":
                    MoodAnimation(markupAttribute.Properties);
                    break;
            }
        }
    }

    private void MoodAnimation(IReadOnlyDictionary<string,MarkupValue> moods)
    {
        switch (moods["mood"].StringValue)
        {
            case "angry":
                LineAngryAnimation();
                break;
        }
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

    public override async void RunLine(LocalizedLine dialogueLine,Action onDialogueLineFinished)
    {
        cts?.Dispose();
        cts = new CancellationTokenSource();
        
        HandleMarkup(dialogueLine.Text.Attributes);

        await RunLineInternal(dialogueLine,onDialogueLineFinished);
    }
    
    private async UniTask RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
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
    }

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

        lineText.text = dialogueLine.TextWithoutCharacterName.Text;
        
        if (useFadeEffect)
        {//演出渐入效果
            // Debug.Log($"演出渐入效果");
            await EffectsAsync.FadeAlpha(canvasGroup, 0, 1, fadeInTime,cts);
        }
        
        if (useTypewriterEffect)
        {
            isTyping = true;
            // setting the canvas all back to its defaults because if we didn't also fade we don't have anything visible
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            // Debug.Log($"演出打字机效果");
            await EffectsAsync.Typewriter(
                lineText,
                ()=>typewriterEffectSpeed,
                () => onCharacterTyped.Invoke(),
                cts);
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
        var interactable = canvasGroup.interactable;
        canvasGroup.interactable = false;
        

        if (useFadeEffect)
        {
            // Debug.Log($"当前文字展示渐出效果");
            await EffectsAsync.FadeAlpha(canvasGroup, 1, 0, fadeOutTime,cts);
        }

        lineDoingTween?.Pause();

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = interactable;
        onDismissComplete();
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        // Debug.Log($"当前文字打断");
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
        
        cts?.Cancel();
        cts = null;
        onDialogueLineFinished();
    }
    
    public override void UserRequestedViewAdvancement()
    {
        if (currentLine == null)
            return;

        requestInterrupt?.Invoke();
    }
}
