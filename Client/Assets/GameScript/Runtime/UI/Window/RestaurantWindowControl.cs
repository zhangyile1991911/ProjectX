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
    // class ChatBubbleComparer : Comparer<ChatBubble>
    // {
    //     public override int Compare(ChatBubble x, ChatBubble y)
    //     {
    //         return 1;
    //     }
    // }
    private ClockWidget _clockWidget;
    
    private List<ChatBubble> _bubbleList;
    // private List<CookResult> _cookResults;
    private List<DragCookFoodIcon> _dragCookFoodIcons;
    private HashSet<int> _floatingBubbleChatId;
    // private StateMachine _machine;
    public override void OnCreate()
    {
        base.OnCreate();
        
        _bubbleList = new List<ChatBubble>(20);

        _clockWidget = new ClockWidget(Ins_ClockWidget.gameObject,this);
        // _cookResults = new List<CookResult>(4);

        _floatingBubbleChatId = new HashSet<int>(20);
        
        _dragCookFoodIcons = new List<DragCookFoodIcon>(4);
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconA.gameObject,this));
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconB.gameObject,this));
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconC.gameObject,this));
        _dragCookFoodIcons.Add(new DragCookFoodIcon(Ins_DragCookFoodIconD.gameObject,this));
        
        // EventModule.Instance.CookFinishSub.Subscribe(showCookFood).AddTo(uiTran);
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    private FlowControlWindowData openData;
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        openData = openParam as FlowControlWindowData;
        Btn_Phone.OnClickAsObservable().Subscribe(ClickPhone).AddTo(handles);
        Btn_Close.OnClickAsObservable().Subscribe(ClickClose).AddTo(handles);
        
        EventModule.Instance.CharacterLeaveSub.Subscribe(RemoveChatBubble).AddTo(handles);
        showCookFood();
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

    private void ClickClose(Unit param)
    {
        UIManager.Instance.OpenUI(UIEnum.ConfirmTipsWindow, 
(IUIBase ui) =>
            {
                var win = ui as ConfirmTipsWindow;
                win.SetTipsInfo("是否现在就要闭店",ClickConfirm,ClickCancel);
            }, null,UILayer.Top);
    }

    private void ClickConfirm()
    {
        UIManager.Instance.CloseUI(UIEnum.ConfirmTipsWindow);
        // EventModule.Instance.CloseRestaurantTopic.OnNext(Unit.Default);
        openData.StateMachine.ChangeState<StatementStateNode>();
    }

    private void ClickCancel()
    {
        UIManager.Instance.CloseUI(UIEnum.ConfirmTipsWindow);
    }
    
    private void ClickPhone(Unit param)
    {
        UniModule.GetModule<Clocker>().AddOneMinute();
        var uiManager = UniModule.GetModule<UIManager>();
        var phone = UIManager.Instance.Get(UIEnum.PhoneWindow);
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

    public async void GenerateChatBubble(int chatId,RestaurantRoleBase restaurantCharacter,Action<ChatBubble> ClickBubble)
    {
        if (_floatingBubbleChatId.Contains(chatId)) return;
        var uiManager = UniModule.GetModule<UIManager>();
        var bubble = await uiManager.CreateUIComponent<ChatBubble>(null,Tran_BubbleGroup,this);
        bubble.SetBubbleInfo(chatId,restaurantCharacter,ClickBubble);
        _bubbleList.Add(bubble);
        _floatingBubbleChatId.Add(chatId);
    }

    public void RemoveChatBubble(RestaurantRoleBase restaurantCharacter)
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

    private void showCookFood()
    {
        // if (_cookResults.Count >= 4) return;
        // _cookResults.Add(result);
        // var index = _cookResults.Count - 1;
        // _dragCookFoodIcons[index].ShowFoodIcon(result,soldCustomerFood);
        var list = UserInfoModule.Instance.CookResults;
        for (int i = 0; i < 4; i++)
        {
            if (i < list.Count)
            {
                var one = list[i];
                _dragCookFoodIcons[i].ShowFoodIcon(one,soldCustomerFood);    
            }
            else
            {
                _dragCookFoodIcons[i].ClearFoodIcon();
            }
        }
    }

    private void soldCustomerFood(CookResult result,int characterId)
    {
        UserInfoModule.Instance.SoldMealId(result.menuId);
        OrderMealInfo info = new();
        info.MenuId = result.menuId;
        info.operation = 1;
        info.CharacterId = characterId;
        EventModule.Instance.OrderMealTopic.OnNext(info);
    }
}