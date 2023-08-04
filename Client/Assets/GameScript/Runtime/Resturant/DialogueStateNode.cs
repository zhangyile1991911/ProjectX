
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
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("OrderDrink",OrderDrinkCommand);
            _dialogWindow.DialogueRunner.AddCommandHandler<string,int>("AddFriend",AddNPCFriendlyValue);
            _dialogWindow.DialogueRunner.AddCommandHandler("CharacterLeave",CharacterLeave);
            _dialogWindow.DialogueRunner.AddCommandHandler<int>("AddNewMenu",AddNewMenuData);
        },openData,UILayer.Center);
        
        _clocker = UniModule.GetModule<Clocker>();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        _restaurantEnter.NoFocusOnCharacter();
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderMeal");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderDrink");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddFriend");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("CharacterLeave");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddNewMenu");
        
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
        _restaurantCharacter.DialogueOrder = menuId;
    }

    private void OrderDrinkCommand(int drinkId)
    {
        
    }

    private void AddNPCFriendlyValue(string name,int val)
    {
        var mgr = UniModule.GetModule<CharacterMgr>();
        var chr = mgr.GetCharacterByName(name);
        if (chr == null) return;
        chr.AddFriendly(val);
        PlayAddFriend(chr,val);
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
        
    }
}
