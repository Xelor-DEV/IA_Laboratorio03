using Assets.ProjectAssets.Scripts;
using UnityEngine;

public class Toilet : Human
{
    [Header("Transition Thresholds")]
    [SerializeField] private float bladderEmptyThreshold = 0.1f;

    private void Awake()
    {
        typeState = TypeState.Toilet;
        LoadComponents();
    }

    public override void Enter()
    {
        hasArrived = false;
        agent.Target = destinationManager.bathroom;
        agent.Type = TypeSteeringBehavior.Arrive;
        agentData.isPaused = true;
        agentData.bladder.decreaseEnabled = false;
    }

    public override void Execute()
    {
        if (!hasArrived)
        {
            if (agent.TargetDistance < arrivalThreshold)
            {
                hasArrived = true;
                agentData.isPaused = false;
                agentData.bladder.decreaseEnabled = true;
            }
            return;
        }

        if (agentData.bladder.current <= bladderEmptyThreshold)
            stateMachine.ChangeState(TypeState.Play);
    }

    public override void Exit()
    {
        agentData.bladder.decreaseEnabled = false;
    }
}
