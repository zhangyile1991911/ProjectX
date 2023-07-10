
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
            _dialogWindow.DialogueRunner.AddCommandHandler<string,int>("AddFriend",AddNPCFriendlyValue);
        },openData,UILayer.Center);
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        _restaurantEnter.NoFocusOnCharacter();
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderMeal");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddFriend");

        UIManager.Instance.CloseUI(UIEnum.CharacterDialogWindow);
    }

    private void DialogueComplete()
    {
        _machine.ChangeState<WaitStateNode>();
        UserInfoModule.Instance.InsertReadDialogueId(_chatId);
    }
    
    private void OrderMealCommand(int mealId)
    {
        Debug.Log($"OrderMealCommand {mealId}");
        OrderMealInfo info = new()
        {
            MealId = mealId,
            Customer = _restaurantCharacter
        };
        EventModule.Instance.OrderMealTopic.OnNext(info);
    }

    private void AddNPCFriendlyValue(string name,int val)
    {
        var mgr = UniModule.GetModule<CharacterMgr>();
        var chr = mgr.GetCharacterByName(name);
        if (chr == null) return;
        chr.AddFriendly(val);
    }
    
}
