using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using Yarn.Markup;
using YooAsset;

public class ComicView : DialogueViewBase
{
    public Image Img_display;
    private AssetOperationHandle handle;
    public void Start()
    {
        Img_display.gameObject.SetActive(false);
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        //1 静态图片 停留x秒结束
        //2 帧动画  等待播放结束
        //3 dotween或者animation动画 等待播放结束
        if(dialogueLine.Text.TryGetAttributeWithName("show_picture",out var attribute))
        {
            // Debug.Log($"ComicView {attribute.Properties}");
            showPicture(attribute,onDialogueLineFinished);
        }
    }

    private void showPicture(MarkupAttribute attribute,Action onDialogueLineFinished)
    {
        var resName = attribute.Properties["show_picture"].StringValue;
        // var duration = attribute.Properties["duration"].IntegerValue;
        handle = YooAssets.LoadAssetAsync<Sprite>("Assets/GameRes/Picture/Story/"+resName+".jpg");
        handle.Completed += async sp =>
        {
            Img_display.sprite = sp.AssetObject as Sprite;
            Img_display.gameObject.SetActive(true);
            // Img_display.SetNativeSize();
            // await UniTask.Delay(duration);
            // Img_display.gameObject.SetActive(false);
            onDialogueLineFinished();
        };
    }
    
    public override void DismissLine(Action onDismissalComplete)
    {
        Img_display.gameObject.SetActive(false);
        Img_display.sprite = null;
        handle?.Release();
        handle = null;
        onDismissalComplete();
    }
    

}
