using Assets.ProjectAssets.Scripts;
using UnityEngine;

public class Eat : Human
{
    [Header("Transition Thresholds")]
    [SerializeField] private float hungerSatisfiedThreshold = 0.1f;

    private void Awake()
    {
        typeState = TypeState.Eat;
        LoadComponents();
    }

    public override void Enter()
    {
        hasArrived = false;
        agent.Target = destinationManager.diningRoom;
        agent.Type = TypeSteeringBehavior.Arrive;
        agentData.isPaused = true;
        agentData.hunger.decreaseEnabled = false;
    }

    public override void Execute()
    {
        if (!hasArrived)
        {
            if (agent.TargetDistance < arrivalThreshold)
            {
                hasArrived = true;
                agentData.isPaused = false;
                agentData.hunger.decreaseEnabled = true;
            }
            return;
        }

        if (agentData.hunger.current <= hungerSatisfiedThreshold)
            stateMachine.ChangeState(TypeState.Play);
    }

    public override void Exit()
    {
        agentData.hunger.decreaseEnabled = false;
    }
}
