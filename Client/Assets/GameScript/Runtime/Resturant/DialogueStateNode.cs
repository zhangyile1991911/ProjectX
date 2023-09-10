
using System;
using System.Collections.Generic;
using cfg.character;
using cfg.common;
using cfg.food;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using PlasticGui.WorkspaceWindow.NotificationBar;
using SQLite;
using UnityEngine;
using Yarn.Unity;

public class DialogueStateNodeData
{
    public RestaurantRoleBase ChatRestaurantCharacter;
    public int ChatId;
}

public class DialogueStateNode : IStateNode
{
    private RestaurantEnter _restaurantEnter;
    private RestaurantCharacter _restaurantCharacter;
    private int _chatId;
    private StateMachine _machine;
    private CharacterDialogWindow _dialogWindow;
    private Clocker _clocker;
    private CharacterBubble _curBubbleTB;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurantEnter = machine.Owner as RestaurantEnter;
    }

    public void OnEnter(object param = null)
    {
        var stateNodeData = param as DialogueStateNodeData;
        
        _restaurantCharacter = stateNodeData.ChatRestaurantCharacter as RestaurantCharacter;
        _chatId = stateNodeData.ChatId;
        
        _restaurantEnter.FocusOnCharacter(_restaurantCharacter);
        
        var openData = new CharacterDialogData();
        _curBubbleTB = DataProviderModule.Instance.GetCharacterBubble(_chatId);
        openData.StoryResPath = _curBubbleTB.DialogueContentRes;
        openData.StoryStartNode = _curBubbleTB.DialogueStartNode;
        openData.StoryComplete = DialogueComplete;
        openData.StoryCharacter = _restaurantCharacter;
        var uiManager = UniModule.GetModule<UIManager>();
        uiManager.OpenUI(UIEnum.CharacterDialogWindow, ui =>
        {
            _dialogWindow = ui as CharacterDialogWindow;
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("OrderMeal",OrderMealCommand);
            _dialogWindow.DialogueRunner.AddCommandHandler<string>("omakase",omakase);
            _dialogWindow.DialogueRunner.AddCommandHandler<int,string>("HybridOrder",HybridOrder);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("OrderDrink",OrderDrinkCommand);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("AddFriend",AddNPCFriendlyValue);
            _dialogWindow.DialogueRunner.AddCommandHandler("CharacterLeave",CharacterLeave);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("AddNewMenu",AddNewMenuData);
            // _dialogWindow.DialogueRunner.AddCommandHandler<int>("ChangePartner",ChangePartner);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("KickOut",KickOut);
        },openData,UILayer.Center);
        
        _clocker = UniModule.GetModule<Clocker>();
        _restaurantEnter.ShowCurtain();
        _restaurantEnter.ShowAndAdjustShopKeeperStand(_restaurantCharacter.SeatOccupy);
        
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _dialogWindow.NextLine();    
        }
    }

    public void OnExit()
    {
        if (_restaurantCharacter.CurBehaviour.BehaviourID == behaviour.WaitReply)
        {
            _restaurantCharacter.CurBehaviour = new CharacterOrderMeal();
        }
        else if (_restaurantCharacter.CurBehaviour.BehaviourID == behaviour.Comment)
        {
            _restaurantCharacter.CurBehaviour = new CharacterThinking();
        }
        
        _restaurantEnter.NoFocusOnCharacter();
        
        _restaurantEnter.HideCurtain();
        _restaurantEnter.HideShopKeeper();
        
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderMeal");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderDrink");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddFriend");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("CharacterLeave");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddNewMenu");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("omakase");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("ChangePartner");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("KickOut");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("HybridOrder");
        
        UIManager.Instance.CloseUI(UIEnum.CharacterDialogWindow);
        _clocker = null;
    }

    private void DialogueComplete()
    {
        if (!_curBubbleTB.Repeated)
        {
            UserInfoModule.Instance.InsertReadDialogueId(_chatId);    
        }
        _restaurantCharacter.RemoveSaidBubble(_chatId);
        _clocker.AddMinute(5);
        _machine.ChangeState<WaitStateNode>();
    }
    
    private void OrderMealCommand(int menuId)
    {
        Debug.Log($"OrderMealCommand {menuId}");
        
        OrderMealInfo info = new()
        {
            MenuId = menuId,
            CharacterId = _restaurantCharacter.CharacterId,
            OrderType = _curBubbleTB.BubbleType
        };
        // EventModule.Instance.OrderMealTopic.OnNext(info);
        info.OrderType = bubbleType.SpecifiedOrder;
        _restaurantCharacter.CurOrderInfo = info;
        _restaurantCharacter.CurBehaviour = new CharacterWaitOrder();//具体点单
        // _restaurantCharacter.DialogueOrder = menuId;
    }

    private void OrderDrinkCommand(int drinkId)
    {
        
    }

    private void HybridOrder(int menuId,string tags)
    {
        Debug.Log($"混合订单请求");
        OrderMealInfo info = new()
        {
            CharacterId = _restaurantCharacter.CharacterId,
            MenuId = menuId,
            OrderType = _curBubbleTB.BubbleType,
            flavor = new HashSet<flavorTag>(10)
        };
        var flavors = tags.Split(";");
        foreach (var str in flavors)
        {
            var flavorId = Int32.Parse(str);
            info.flavor.Add((flavorTag)flavorId);
        }

        info.OrderType = bubbleType.HybridOrder;
        _restaurantCharacter.CurOrderInfo = info;
        _restaurantCharacter.CurBehaviour = new CharacterWaitOrder();//混合点单
        // EventModule.Instance.OrderMealTopic.OnNext(info);
    }

    private void omakase(string desc)
    {
        Debug.Log($"お任せ {desc}");
        OrderMealInfo info = new()
        {
            CharacterId = _restaurantCharacter.CharacterId,
            OrderType = _curBubbleTB.BubbleType,
            flavor = new HashSet<flavorTag>(10)
        };
        var flavors = desc.Split(";");
        foreach (var str in flavors)
        {
            var flavorId = Int32.Parse(str);
            info.flavor.Add((flavorTag)flavorId);
        }
        info.OrderType = bubbleType.Omakase;
        _restaurantCharacter.CurOrderInfo = info;
        _restaurantCharacter.CurBehaviour = new CharacterWaitOrder();//omakase
        // EventModule.Instance.OrderMealTopic.OnNext(info);
    }

    private void AddNPCFriendlyValue(int val)
    {
        var mgr = UniModule.GetModule<CharacterMgr>();
        // var chr = mgr.GetCharacterByName(name);
        // if (chr == null) return;
        _restaurantCharacter.AddFriendly(val);
        PlayAddFriend(_restaurantCharacter,val);
        // DialogueNotification notification = new ();
        // notification.Character = chr;
        // notification.FriendValue = val;
        // EventModule.Instance.DialogueMsgTopic.OnNext(notification);

    }

    private void CharacterLeave()
    {
        _restaurantCharacter.CurBehaviour = new CharacterLeave();
    }

    private void PlayAddFriend(RestaurantRoleBase character,int val)
    {
        var openParam = new DialogueData();
        openParam.Character = character;
        openParam.FriendValue = val;
        UIManager.Instance.CreateTip<TipAddFriendValue>(openParam).Forget();
    }

    private void AddNewMenuData(int menuId)
    {
        var tbMenuInfo = DataProviderModule.Instance.GetMenuInfo(menuId);
        if (tbMenuInfo == null) return;
        
        UserInfoModule.Instance.AddNewMenu(menuId);
        
        var openParam = new DialogueData();
        openParam.MenuId = menuId;
        UIManager.Instance.CreateTip<TipNewMenu>(openParam).Forget();
    }

    // private void ChangePartner(int partnerId)
    // {
    //     var tbCharaBase = DataProviderModule.Instance.GetCharacterBaseInfo(partnerId);
    //     if (tbCharaBase == null)
    //     {
    //         Debug.LogWarning($"Change Partner Id = {partnerId}");
    //     }
    //     _restaurantCharacter.PartnerID = partnerId;
    // }

    private void KickOut(int characterId)
    {
        var characterObj = CharacterMgr.Instance.GetCharacterById(characterId);
        if (characterObj == null)
        {
            Debug.LogError($"KickOut characterId = {characterId} can't find character");
            return;
        }
        characterObj.CurBehaviour = new CharacterLeave();
    }
}
