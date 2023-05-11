using System;

public class DialogueStateNode : IStateNode
{
    private RestaurantEnter _restaurantEnter;
    private Character _character;
    public void OnCreate(StateMachine machine)
    {
        _restaurantEnter = machine.Owner as RestaurantEnter;
    }

    public void OnEnter(object param = null)
    {
        _character = param as Character;
        _restaurantEnter.FocusOnCharacter(_character);
        var uiManager = UniModule.GetModule<UIManager>();
        uiManager.OpenUI(UIEnum.CharacterDialogWindow,null,null,UILayer.Center);
    }

    public void OnUpdate()
    {
        // throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        _restaurantEnter.NoFocusOnCharacter();
        var uiManager = UniModule.GetModule<UIManager>();
        uiManager.CloseUI(UIEnum.CharacterDialogWindow);
    }
}
