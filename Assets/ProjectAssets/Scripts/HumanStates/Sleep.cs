// Sleep.cs
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
        base.Enter();
        agentData.isPaused = true;
        agentData.energy.increaseEnabled = false;
        SetDestination(destinationManager.bedroom.position);
    }

    public override void Execute()
    {
        if (HasReachedDestination())
        {
            agentData.isPaused = false;
            agentData.energy.increaseEnabled = true;

            if (agentData.energy.current >= energyFullThreshold)
                stateMachine.ChangeState(TypeState.Play);
        }
    }

    public override void Exit()
    {
        base.Exit();
        agentData.energy.increaseEnabled = false;
        StopMovement();
    }
}