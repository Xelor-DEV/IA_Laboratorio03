using Assets.ProjectAssets.Scripts;
using UnityEngine;

public class Sleep : Human
{
    [Header("Transition Thresholds")]
    [SerializeField] private float energyFullThreshold = 0.9f;

    private void Awake()
    {
        typeState = TypeState.Sleep;
        LoadComponents();
    }

    public override void Enter()
    {
        hasArrived = false;
        agent.Target = destinationManager.bedroom;
        agent.Type = TypeSteeringBehavior.Arrive;
        agentData.isPaused = true;

        agentData.energy.increaseEnabled = false;
    }

    public override void Execute()
    {
        if (!hasArrived)
        {
            if (agent.TargetDistance < arrivalThreshold)
            {
                hasArrived = true;
                agentData.isPaused = false;
                agentData.energy.increaseEnabled = true;
            }
            return;
        }

        // Verificar ambas condiciones
        if (agentData.energy.current >= energyFullThreshold)
        {
            stateMachine.ChangeState(TypeState.Play);
        }
    }

    public override void Exit()
    {
        agentData.energy.increaseEnabled = false;
    }
}
