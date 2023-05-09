public class DialogueStateNode : IStateNode
{
    private RestaurantEnter _restaurantEnter;
    public void OnCreate(StateMachine machine)
    {
        _restaurantEnter = machine.Owner as RestaurantEnter;
    }

    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
