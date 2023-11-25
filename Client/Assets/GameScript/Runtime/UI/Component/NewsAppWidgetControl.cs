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


public partial class NewsAppWidget : BaseAppWidget
{
    private Animation _animation;
    private List<cfg.phone.AppNewsInfo> _newsDataList;
    private Clocker _clocker;
    private CompositeDisposable _handlers;
    private NewsDetailWidget _detailWidget;
    public NewsAppWidget(GameObject go,UIWindow parent):base(go,parent)
    {
        WidgetType = AppType.News;
    }
    
    public override void OnCreate()
    {
        _animation = uiGo.GetComponent<Animation>();
        _newsDataList = new List<cfg.phone.AppNewsInfo>(10);
        _handlers = new CompositeDisposable();
        Grid_News.InitListView(0,OnGetNormalNews);
        _detailWidget = new NewsDetailWidget(Ins_Detail.gameObject, ParentWindow);
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        
        _animation.Play("AppOnLoad");

        // loadTitleImage();

        MakeNewsData();
        
        if (Grid_News.ItemTotalCount == _newsDataList.Count)
        {
            Grid_News.RefreshAllShownItem();
        }
        else
        {
            Grid_News.SetListItemCount(_newsDataList.Count);    
        }
        
        _clocker = UniModule.GetModule<Clocker>();

        _detailWidget.XBtn_Return.OnClick.Subscribe(param =>
        {
            _detailWidget.PlayOutAnimation();
        }).AddTo(_handlers);
    }

    public override async void OnHide()
    {
        _animation.Play("AppOnHide");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        base.OnHide();
        foreach (var one in _handlers)
        {
            one.Dispose();
        }
        _handlers.Clear();
        
    }
    
    public override void OnDestroy()
    {
        
    }

    LoopListViewItem2 OnGetNormalNews(LoopListView2 loopListView, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= _newsDataList.Count)
        {
            return null;
        }
        
        if (string.IsNullOrEmpty(_newsDataList[itemIndex].Preview))
        {
            var item = loopListView.NewListViewItem("NewsMojiCellWidget");
            NewsMojiCellWidget cell;
            if (item.IsInitHandlerCalled)
            {
                cell = item.UserObjectData as NewsMojiCellWidget;
                _clocker.AddSecond(30);
            }
            else
            {
                cell = new NewsMojiCellWidget(item.gameObject, ParentWindow);
                item.UserObjectData = cell;
                item.IsInitHandlerCalled = true;
                cell.XBtn_bg.OnClick.Subscribe((param) =>
                {
                    _detailWidget.OnShow(null);
                    _detailWidget.SetNewDetail(_newsDataList[itemIndex]);
                    _detailWidget.PlayInAnimation();
                }).AddTo(_handlers);
            }
            cell.SetNewsDetailInfo(_newsDataList[itemIndex]);
            // loopGridView.SetItemSize(new Vector2(450,80));
            loopListView.OnItemSizeChanged(itemIndex);
            return item;
        }
        else
        {
            var item = loopListView.NewListViewItem("NewsCellWidget");
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
                cell.XBtn_bg.OnClick.Subscribe((param) =>
                {
                    _detailWidget.OnShow(null);
                    _detailWidget.SetNewDetail(_newsDataList[itemIndex]);
                    _detailWidget.PlayInAnimation();
                }).AddTo(_handlers);
            }
            cell.SetNewsDetailInfo(_newsDataList[itemIndex]);
            loopListView.OnItemSizeChanged(itemIndex);
            // loopGridView.SetItemSize(new Vector2(450,150));
            return item;
        }
    }

    public override void OnUpdate()
    {
        
    }

    void loadTitleImage()
    {
        var handle = YooAssets.LoadAssetAsync<Sprite>("Assets/GameRes/Picture/Icon/bignews.png");
        handle.Completed += (ho) =>
        {
            Img_BigNews.sprite = ho.AssetObject as Sprite;
        };
    }

    void MakeNewsData()
    {
        _newsDataList = DataProviderModule.Instance.FilterNews();
        // _newsList.Add(new NormalNewsData()
        // {
        //     Title = "朱珠老公晒带娃日常，喂饭...",
        //     NewsImage = "Assets/GameRes/Picture/Icon/zhuzhu.png",
        //     Follow = "娱絮 34跟帖",
        //     Comment = "一直觉得朱珠会嫁给欧美人"
        // });
        // _newsList.Add(new NormalNewsData()
        // {
        //     Title = "大道之行 筑梦丝路",
        //     NewsImage = "Assets/GameRes/Picture/Icon/dadao.png",
        //     Follow = "荔枝新闻 3跟帖",
        //     Comment = "中哈物流进一步发展！"
        // });
        // _newsList.Add(new NormalNewsData()
        // {
        //     Title = "世界杯抽签：中国男篮与塞尔",
        //     NewsImage = "Assets/GameRes/Picture/Icon/lanqiu.png",
        //     Follow = "网易体育 1849跟帖",
        //     Comment = "这算是个上上签了"
        // });
        // _newsList.Add(new NormalNewsData()
        // {
        //     Title = "国家统计局：4月官方制造业",
        //     NewsImage = "Assets/GameRes/Picture/Icon/zhizaoye.png",
        //     Follow = "国家统计局 475跟帖",
        //     Comment = "冷暖自知！"
        // });
        // _newsList.Add(new NormalNewsData()
        // {
        //     Title = "国家网信办：百度、新浪微博、",
        //     NewsImage = "Assets/GameRes/Picture/Icon/baidu.png",
        //     Follow = "网信中国 2476跟帖",
        //     Comment = "抖音低俗色情打擦边球网贷广告泛滥，必须查处！"
        // });
        // _newsList.Add(new NormalNewsData()
        // {
        //     Title = "各地土拍“冷热不均”：核心城市",
        //     NewsImage = "Assets/GameRes/Picture/Icon/tupai.png",
        //     Follow = "澎湃新闻 75跟帖",
        //     Comment = "都到这一步了就别再忽悠了好不好，说点实话干点实事吧"
        // });
    }
    
    
}