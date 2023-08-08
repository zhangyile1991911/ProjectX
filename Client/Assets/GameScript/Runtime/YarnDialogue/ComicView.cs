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
    public Transform AnimationNode;
    private AssetOperationHandle handle;
    public void Start()
    {
        Img_display.gameObject.SetActive(false);
        AnimationNode.gameObject.SetActive(false);
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
        else if (dialogueLine.Text.TryGetAttributeWithName("play_anim", out  attribute))
        {
            playAnimation(attribute,onDialogueLineFinished);
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

    private async void playAnimation(MarkupAttribute attribute,Action onDialogueLineFinished)
    {
        var resName = attribute.Properties["play_anim"].StringValue;
        handle = YooAssets.LoadAssetAsync<Sprite>("Assets/GameRes/Prefabs/Story/"+resName+".prefab");
        await handle.ToUniTask(this);
        
        var go = handle.AssetObject as GameObject;
        var instance = Instantiate(go, AnimationNode);
        var animation = instance.GetComponent<Animation>();
        animation.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(animation.clip.length));
        onDialogueLineFinished();
    }
    
    public override void DismissLine(Action onDismissalComplete)
    {
        Img_display.gameObject.SetActive(false);
        Img_display.sprite = null;
        AnimationNode.gameObject.SetActive(false);
        for (int i = 0; i < AnimationNode.childCount; i++)
        {
            Destroy(AnimationNode.GetChild(i).gameObject);
        }
        
        handle?.Release();
        handle = null;
        onDismissalComplete();
    }
    

}
