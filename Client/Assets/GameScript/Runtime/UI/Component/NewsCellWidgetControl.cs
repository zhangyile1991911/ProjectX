using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public class NormalNewsData
{
    public string Title;
    public string NewsImage;
    public string Follow;
    public string Comment;
};
public partial class NewsCellWidget : UIComponent
{
    public NewsCellWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
        
    }
    
    public override void OnDestroy()
    {
        
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }

    public void SetNewsDetailInfo(NormalNewsData newsData)
    {
        Txt_Title.text = newsData.Title;
        Txt_Follow.text = newsData.Follow;
        Txt_Comment.text = newsData.Comment;
        var handle = YooAssets.LoadAssetAsync<Sprite>(newsData.NewsImage);
        handle.Completed += (OH) =>
        {
            Img_News.sprite = OH.AssetObject as Sprite;
        };
    }
    
}