
public interface IStateNode
{
    void OnCreate(StateMachine machine);
    void OnEnter(object param);
    void OnUpdate();
    void OnExit();
}
