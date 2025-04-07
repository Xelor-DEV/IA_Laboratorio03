using UnityEngine;
using Assets.ProjectAssets.Scripts;
public enum TypeState
{
    Play,
    Eat,
    Toilet,
    Sleep
}

public class Human : State
{
    protected AgentDataManager agentData;
    protected Agent agent;
    protected DestinationManager destinationManager;
    [SerializeField] protected bool hasArrived;

    [Header("Settings")]
    [SerializeField] protected float arrivalThreshold = 0.7f;

    public override void LoadComponents()
    {
        base.LoadComponents();
        agentData = GetComponent<AgentDataManager>();
        agent = GetComponent<Agent>();
        destinationManager = GetComponent<DestinationManager>();
    }
}
