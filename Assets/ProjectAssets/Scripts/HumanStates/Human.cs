using UnityEngine;
using UnityEngine.AI;
public enum TypeState
{
    Play,
    Eat,
    Toilet,
    Sleep,
    FollowTheToy
}

public class Human : State
{
    protected NavMeshAgent navAgent;
    protected AgentDataManager agentData;
    protected DestinationManager destinationManager;

    public override void LoadComponents()
    {
        base.LoadComponents();
        navAgent = GetComponent<NavMeshAgent>();
        agentData = GetComponent<AgentDataManager>();
        destinationManager = GetComponent<DestinationManager>();
    }

    protected bool HasReachedDestination()
    {
        return !navAgent.pathPending &&
               navAgent.remainingDistance <= navAgent.stoppingDistance &&
               (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f);
    }

    protected void SetDestination(Vector3 target)
    {
        navAgent.SetDestination(target);
        navAgent.isStopped = false;
    }

    protected void StopMovement()
    {
        navAgent.isStopped = true;
    }
}
