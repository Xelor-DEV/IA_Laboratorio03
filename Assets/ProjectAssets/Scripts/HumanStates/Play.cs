public class Play : Human
{
    private void Awake()
    {
        typestate = TypeState.Play;
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
        if (_DataAgent.Energy.value < 0.25f)
        {
            _StateMachine.ChangeState(TypeState.Sleep);
        }
        else {
            _DataAgent.DiscountEnergy();
        }

         base.Execute();
    }
    public override void Exit()
    {

    }
}
