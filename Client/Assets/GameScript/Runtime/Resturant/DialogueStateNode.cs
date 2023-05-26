
using UnityEngine;
using Yarn.Unity;

public class DialogueStateNode : IStateNode
{
    private RestaurantEnter _restaurantEnter;
    private Character _character;
    private StateMachine _machine;
    private CharacterDialogWindow _dialogWindow;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurantEnter = machine.Owner as RestaurantEnter;
    }

    public void OnEnter(object param = null)
    {
        _character = param as Character;
        _restaurantEnter.FocusOnCharacter(_character);
        
        var openData = new CharacterDialogData();
        openData.StoryResPath = "Assets/GameRes/Story/NewProject.yarnproject";
        openData.StoryStartNode = "Beginner";
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
        // throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        _restaurantEnter.NoFocusOnCharacter();
        
        _dialogWindow.DialogueRunner.RemoveCommandHandler("OrderMeal");
        _dialogWindow.DialogueRunner.RemoveCommandHandler("AddFriend");
        
        var uiManager = UniModule.GetModule<UIManager>();
        uiManager.CloseUI(UIEnum.CharacterDialogWindow);
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
        var eventModule = UniModule.GetModule<EventModule>();
        eventModule.OrderMealTopic.OnNext(info);
    }

    private void AddNPCFriendlyValue(string name,int val)
    {
        var mgr = UniModule.GetModule<CharacterMgr>();
        var chr = mgr.GetCharacter(name);
        if (chr == null) return;
        chr.AddFriendly(val);
    }
}
