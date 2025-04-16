// Play.cs
using UnityEngine;

public class Play : Human
{
    [Header("Transition Thresholds")]
    [SerializeField] private float energyThreshold = 0.2f;
    [SerializeField] private float hungerThreshold = 0.8f;
    [SerializeField] private float bladderThreshold = 0.7f;

    private bool firstPositionReached;

    private void Awake()
    {
        typeState = TypeState.Play;
        LoadComponents();
    }

    public override void Enter()
    {
        base.Enter();
        firstPositionReached = false;
        agentData.isPaused = true;
        ToggleNeeds(false);

        SetDestination(destinationManager.randomPositionGenerator.GetNextPosition());
    }

    public override void Execute()
    {
        if (!firstPositionReached)
        {
            if (HasReachedDestination())
            {
                firstPositionReached = true;
                agentData.isPaused = false;
                ToggleNeeds(true);
            }
            return;
        }

        if (HasReachedDestination())
        {
            SetDestination(destinationManager.randomPositionGenerator.GetNextPosition());
        }

        CheckStateTransitions();
    }

    private void ToggleNeeds(bool state)
    {
        agentData.energy.decreaseEnabled = state;
        agentData.hunger.increaseEnabled = state;
        agentData.bladder.increaseEnabled = state;
    }

    private void CheckStateTransitions()
    {
        if (agentData.energy.current <= energyThreshold)
            stateMachine.ChangeState(TypeState.Sleep);
        else if (agentData.hunger.current >= hungerThreshold)
            stateMachine.ChangeState(TypeState.Eat);
        else if (agentData.bladder.current >= bladderThreshold)
            stateMachine.ChangeState(TypeState.Toilet);
    }

    public override void Exit()
    {
        base.Exit();
        ToggleNeeds(false);
        StopMovement();
    }
}