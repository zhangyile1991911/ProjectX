
using System;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using PlasticGui.WorkspaceWindow.NotificationBar;
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
        var bubbleTB = DataProviderModule.Instance.GetCharacterBubble(_chatId);
        openData.StoryResPath = bubbleTB.DialogueContentRes;
        openData.StoryStartNode = bubbleTB.DialogueStartNode;
        openData.StoryComplete = DialogueComplete;
        openData.StoryCharacter = _restaurantCharacter;
        var uiManager = UniModule.GetModule<UIManager>();
        uiManager.OpenUI(UIEnum.CharacterDialogWindow, ui =>
        {
            _dialogWindow = ui as CharacterDialogWindow;
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("OrderMeal",OrderMealCommand);
            _dialogWindow.DialogueRunner.AddCommandHandler<string>("omakase",omakase);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("OrderDrink",OrderDrinkCommand);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("AddFriend",AddNPCFriendlyValue);
            _dialogWindow.DialogueRunner.AddCommandHandler("CharacterLeave",CharacterLeave);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("AddNewMenu",AddNewMenuData);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("ChangePartner",ChangePartner);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("KickOut",KickOut);
        },openData,UILayer.Center);
        
        _clocker = UniModule.GetModule<Clocker>();
        _restaurantEnter.ShowCurtain();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        _restaurantEnter.NoFocusOnCharacter();
        _restaurantEnter.HideCurtain();
        
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderMeal");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderDrink");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddFriend");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("CharacterLeave");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddNewMenu");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("omakase");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("ChangePartner");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("KickOut");
        
        UIManager.Instance.CloseUI(UIEnum.CharacterDialogWindow);
        _clocker = null;
    }

    private void DialogueComplete()
    {
        var tb = DataProviderModule.Instance.GetCharacterBubble(_chatId);
        if (!tb.Repeated)
        {
            UserInfoModule.Instance.InsertReadDialogueId(_chatId);    
        }
        _clocker.AddMinute(5);
        _machine.ChangeState<WaitStateNode>();
    }
    
    private void OrderMealCommand(int menuId)
    {
        Debug.Log($"OrderMealCommand {menuId}");
        OrderMealInfo info = new()
        {
            MenuId = menuId,
            CharacterId = _restaurantCharacter.CharacterId
        };
        EventModule.Instance.OrderMealTopic.OnNext(info);
        _restaurantCharacter.CurOrderMealInfo = info;
        // _restaurantCharacter.DialogueOrder = menuId;
    }

    private void OrderDrinkCommand(int drinkId)
    {
        
    }

    private void omakase(string desc)
    {
        Debug.Log($"お任せ {desc}");
        OrderMealInfo info = new()
        {
            CharacterId = _restaurantCharacter.CharacterId,
            flavor = new List<int>(10)
        };
        var flavors = desc.Split(";");
        foreach (var str in flavors)
        {
            var flavorId = Int16.Parse(str);
            info.flavor.Add(flavorId);
        }
        _restaurantCharacter.CurOrderMealInfo = info;
        EventModule.Instance.OrderMealTopic.OnNext(info);
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
        _restaurantCharacter.SetLeave();
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

    private void ChangePartner(int partnerId)
    {
        var tbCharaBase = DataProviderModule.Instance.GetCharacterBaseInfo(partnerId);
        if (tbCharaBase == null)
        {
            Debug.LogWarning($"Change Partner Id = {partnerId}");
        }
        _restaurantCharacter.PartnerID = partnerId;
    }

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
