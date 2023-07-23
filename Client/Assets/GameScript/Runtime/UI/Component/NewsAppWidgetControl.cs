using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SuperScrollView;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
/// <summary>
/// Auto Generate Class!!!
/// </summary>
public class NewsAppOpenData : UIOpenParam
{
    public int bigNewsId;
    public List<int> normalNewsIdList;
}

public partial class NewsAppWidget : UIComponent
{
    private Animation _animation;
    private List<NormalNewsData> _newsList;
    private Clocker _clocker;
    public NewsAppWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        
    }
    
    public override void OnCreate()
    {
        _animation = uiGo.GetComponent<Animation>();
        _newsList = new List<NormalNewsData>(10);
        Grid_News.InitGridView(0,OnGetNormalNews);
    }

    LoopGridViewItem OnGetNormalNews(LoopGridView loopGridView, int itemIndex, int row, int column)
    {
        var item = loopGridView.NewListViewItem("NewsCellWidget");
        NewsCellWidget cell;
        if (item.IsInitHandlerCalled)
        {
            cell = item.UserObjectData as NewsCellWidget;
            _clocker.AddSecond(30);
        }
        else
        {
            cell = new NewsCellWidget(item.gameObject, ParentWindow);
            item.UserObjectData = cell;
            item.IsInitHandlerCalled = true;
        }
        cell.SetNewsDetailInfo(_newsList[itemIndex]);
        return item;
    }
    
    public override void OnDestroy()
    {
        
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        
        _animation.Play("AppOnLoad");
        
        var openData = openParam as NewsAppOpenData;
        
        var handle = YooAssets.LoadAssetAsync<Sprite>("Assets/GameRes/Picture/Icon/bignews.png");
        handle.Completed += (ho) =>
        {
            Img_BigNews.sprite = ho.AssetObject as Sprite;
        };

        MakeNewsData();
        
        if (Grid_News.ItemTotalCount == _newsList.Count)
        {
            Grid_News.RefreshAllShownItem();
        }
        else
        {
            Grid_News.SetListItemCount(_newsList.Count);    
        }
        
        _clocker = UniModule.GetModule<Clocker>();

    }

    public override async void OnHide()
    {
        _animation.Play("AppOnHide");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }

    void MakeNewsData()
    {
        _newsList.Add(new NormalNewsData()
        {
            Title = "朱珠老公晒带娃日常，喂饭...",
            NewsImage = "Assets/GameRes/Picture/Icon/zhuzhu.png",
            Follow = "娱絮 34跟帖",
            Comment = "一直觉得朱珠会嫁给欧美人"
        });
        _newsList.Add(new NormalNewsData()
        {
            Title = "大道之行 筑梦丝路",
            NewsImage = "Assets/GameRes/Picture/Icon/dadao.png",
            Follow = "荔枝新闻 3跟帖",
            Comment = "中哈物流进一步发展！"
        });
        _newsList.Add(new NormalNewsData()
        {
            Title = "世界杯抽签：中国男篮与塞尔",
            NewsImage = "Assets/GameRes/Picture/Icon/lanqiu.png",
            Follow = "网易体育 1849跟帖",
            Comment = "这算是个上上签了"
        });
        _newsList.Add(new NormalNewsData()
        {
            Title = "国家统计局：4月官方制造业",
            NewsImage = "Assets/GameRes/Picture/Icon/zhizaoye.png",
            Follow = "国家统计局 475跟帖",
            Comment = "冷暖自知！"
        });
        _newsList.Add(new NormalNewsData()
        {
            Title = "国家网信办：百度、新浪微博、",
            NewsImage = "Assets/GameRes/Picture/Icon/baidu.png",
            Follow = "网信中国 2476跟帖",
            Comment = "抖音低俗色情打擦边球网贷广告泛滥，必须查处！"
        });
        _newsList.Add(new NormalNewsData()
        {
            Title = "各地土拍“冷热不均”：核心城市",
            NewsImage = "Assets/GameRes/Picture/Icon/tupai.png",
            Follow = "澎湃新闻 75跟帖",
            Comment = "都到这一步了就别再忽悠了好不好，说点实话干点实事吧"
        });
    }
    
    
}