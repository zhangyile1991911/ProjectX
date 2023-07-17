
using UnityEngine;
using Yarn.Unity;

public class DialogueStateNodeData
{
    public RestaurantCharacter ChatRestaurantCharacter;
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
        
        _restaurantCharacter = stateNodeData.ChatRestaurantCharacter;
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
            Customer = _restaurantCharacter
        };
        EventModule.Instance.OrderMealTopic.OnNext(info);
        _restaurantCharacter.DialogueOrder(menuId);
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
    }

    private void CharacterLeave()
    {
        _restaurantCharacter.SetLeave();
    }
}
