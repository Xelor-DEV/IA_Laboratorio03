// Toilet.cs
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
        base.Enter();
        agentData.isPaused = true;
        agentData.bladder.decreaseEnabled = false;
        SetDestination(destinationManager.bathroom.position);
    }

    public override void Execute()
    {
        if (HasReachedDestination())
        {
            agentData.isPaused = false;
            agentData.bladder.decreaseEnabled = true;

            if (agentData.bladder.current <= bladderEmptyThreshold)
                stateMachine.ChangeState(TypeState.Play);
        }
    }

    public override void Exit()
    {
        base.Exit();
        agentData.bladder.decreaseEnabled = false;
        StopMovement();
    }
}