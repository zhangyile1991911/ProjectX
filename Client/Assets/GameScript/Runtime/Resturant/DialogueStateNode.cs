
using UnityEngine;
using Yarn.Unity;

public class DialogueStateNodeData
{
    public Character ChatCharacter;
    public int ChatId;
}

public class DialogueStateNode : IStateNode
{
    private RestaurantEnter _restaurantEnter;
    private Character _character;
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
        
        _character = stateNodeData.ChatCharacter;
        _chatId = stateNodeData.ChatId;
        
        _restaurantEnter.FocusOnCharacter(_character);
        
        var openData = new CharacterDialogData();
        var bubbleTB = DataProviderModule.Instance.GetCharacterBubble(_chatId);
        openData.StoryResPath = bubbleTB.DialogueContentRes;
        openData.StoryStartNode = bubbleTB.DialogueStartNode;
        openData.StoryComplete = DialogueComplete; 
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
        
        // _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderMeal");
        // _dialogWindow.DialogueRunner.RemoveCommandHandler("AddFriend");
        
        UIManager.Instance.CloseUI(UIEnum.CharacterDialogWindow);
    }

    private void DialogueComplete()
    {
        _machine.ChangeState<WaitStateNode>();
    }

    // [YarnCommand("OrderMeal")]
    private void OrderMealCommand(int mealId)
    {
        Debug.Log($"OrderMealCommand {mealId}");
        OrderMealInfo info = new()
        {
            MealId = mealId,
            Customer = _character
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
