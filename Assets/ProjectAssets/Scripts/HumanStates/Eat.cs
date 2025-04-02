public class Eat : Human
{
    private void Awake()
    {
        typestate = TypeState.Eat;
        LocadComponent();
    }
    public override void LocadComponent()
    {
        base.LocadComponent();
    }
    public override void Enter()
    {

    }
    public override void Execute()
    {
        base.Execute();
    }
    public override void Exit()
    {

    }
}
