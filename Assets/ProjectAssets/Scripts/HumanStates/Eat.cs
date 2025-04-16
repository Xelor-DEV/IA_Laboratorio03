// Eat.cs
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
        base.Enter();
        agentData.isPaused = true;
        agentData.hunger.decreaseEnabled = false;
        SetDestination(destinationManager.diningRoom.position);
    }

    public override void Execute()
    {
        if (HasReachedDestination())
        {
            agentData.isPaused = false;
            agentData.hunger.decreaseEnabled = true;

            if (agentData.hunger.current <= hungerSatisfiedThreshold)
                stateMachine.ChangeState(TypeState.Play);
        }
    }

    public override void Exit()
    {
        base.Exit();
        agentData.hunger.decreaseEnabled = false;
        StopMovement();
    }
}