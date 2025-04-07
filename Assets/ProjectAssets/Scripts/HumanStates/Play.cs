using UnityEngine;
using Assets.ProjectAssets.Scripts;

public class Play : Human
{
    [Header("Transition Thresholds")]
    [SerializeField] private float energyThreshold = 0.2f;
    [SerializeField] private float hungerThreshold = 0.8f;
    [SerializeField] private float bladderThreshold = 0.7f;

    private void Awake()
    {
        typeState = TypeState.Play;
        LoadComponents();
    }

    public override void Enter()
    {
        hasArrived = false;
        agent.Target = destinationManager.playArea;
        agent.Type = TypeSteeringBehavior.Arrive;
        agentData.isPaused = true;

        agentData.energy.decreaseEnabled = false;
        agentData.hunger.increaseEnabled = false;
        agentData.bladder.increaseEnabled = false;
    }

    public override void Execute()
    {
        if (!hasArrived)
        {
            if (agent.TargetDistance < arrivalThreshold)
            {
                hasArrived = true;
                agentData.isPaused = false;
                agentData.energy.decreaseEnabled = true;
                agentData.hunger.increaseEnabled = true;
                agentData.bladder.increaseEnabled = true;
            }
            return;
        }

        if (agentData.energy.current <= energyThreshold)
            stateMachine.ChangeState(TypeState.Sleep);
        else if (agentData.hunger.current >= hungerThreshold)
            stateMachine.ChangeState(TypeState.Eat);
        else if (agentData.bladder.current >= bladderThreshold)
            stateMachine.ChangeState(TypeState.Toilet);
    }

    public override void Exit()
    {
        agentData.energy.decreaseEnabled = false;
        agentData.hunger.increaseEnabled = false;
        agentData.bladder.increaseEnabled = false;
    }
}
