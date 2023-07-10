using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class RestaurantWindow : UIWindow
{
    class ChatBubbleComparer : Comparer<ChatBubble>
    {
        public override int Compare(ChatBubble x, ChatBubble y)
        {
            return 1;
        }
    }
    private ClockWidget _clockWidget;
    
    private List<ChatBubble> _bubbleList;
    private List<CookResult> _cookResults;
    private List<DragCookFoodIcon> _dragCookFoodIcons;
    private HashSet<int> _floatingBubbleChatId;
    private StateMachine _machine;
    public override void OnCreate()
    {
        base.OnCreate();
        
        _bubbleList = new List<ChatBubble>(20);

        _clockWidget = new ClockWidget(Ins_ClockWidget.gameObject,this);
        _cookResults = new List<CookResult>(4);

        _floatingBubbleChatId = new HashSet<int>(20);
        
        _dragCookFoodIcons = new List<DragCookFoodIcon>(4);
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconA.gameObject,this));
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconB.gameObject,this));
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconC.gameObject,this));
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconD.gameObject,this));
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        Btn_Phone.OnClickAsObservable().Subscribe(ClickPhone).AddTo(handles);
        EventModule.Instance.CookFinishSub.Subscribe(showCookFood).AddTo(uiTran);
        // Btn_Bubble.OnClickAsObservable().Subscribe(ClickTest).AddTo(handles);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void ClickPhone(Unit param)
    {
        UniModule.GetModule<Clocker>().AddOneMinute();
        var uiManager = UniModule.GetModule<UIManager>();
        var phone = uiManager.Get(UIEnum.PhoneWindow);
        if (phone == null)
        {
            uiManager.OpenUI(UIEnum.PhoneWindow,null,null,UILayer.Center);   
        }
        else if (phone.IsActive)
        {
            uiManager.CloseUI(UIEnum.PhoneWindow);    
        }
        else
        {
            uiManager.OpenUI(UIEnum.PhoneWindow,null,null,UILayer.Center);    
        }
    }

    public async void GenerateChatBubble(int chatId,RestaurantCharacter restaurantCharacter,Action<ChatBubble> ClickBubble)
    {
        if (_floatingBubbleChatId.Contains(chatId)) return;
        var uiManager = UniModule.GetModule<UIManager>();
        var bubble = await uiManager.CreateUIComponent<ChatBubble>(null,Tran_BubbleGroup,this);
        bubble.SetBubbleInfo(chatId,restaurantCharacter,ClickBubble);
        _bubbleList.Add(bubble);
        _floatingBubbleChatId.Add(chatId);
    }

    public void RemoveChatBubble(RestaurantCharacter restaurantCharacter)
    {
        var uiManager = UniModule.GetModule<UIManager>();
        for (int i = _bubbleList.Count - 1; i >= 0; i--)
        {
            if (_bubbleList[i].Owner == restaurantCharacter)
            {
                _bubbleList.RemoveAt(i);
                uiManager.DestroyUIComponent(_bubbleList[i]);
            }
        }
    }

    public void RemoveChatBubble(ChatBubble bubble)
    {
        _bubbleList.Remove(bubble);
        var uiManager = UniModule.GetModule<UIManager>();
        uiManager.DestroyUIComponent(bubble);
        _floatingBubbleChatId.Add(bubble.ChatId);
    }

    private void showCookFood(CookResult result)
    {
        if (_cookResults.Count >= 4) return;
        _cookResults.Add(result);
        var index = _cookResults.Count - 1;
        _dragCookFoodIcons[index].ShowFoodIcon(result);
    }

}