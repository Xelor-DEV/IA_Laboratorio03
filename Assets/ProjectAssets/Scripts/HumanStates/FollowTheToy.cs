using UnityEngine;

public class FollowTheToy : Human
{
    private EntityIdentity targetToy;
    private AIEye aiEye;

    private void Awake()
    {
        typeState = TypeState.FollowTheToy;
        LoadComponents();
    }

    public override void LoadComponents()
    {
        base.LoadComponents();
        aiEye = GetComponent<AIEye>();
    }

    public override void Enter()
    {
        base.Enter();
        agentData.isPaused = true;

        if (aiEye.ViewToy != null)
        {
            targetToy = aiEye.ViewToy;
            SetDestination(targetToy.transform.position);
        }
        else
        {
            stateMachine.ChangeState(TypeState.Play);
        }
    }

    public override void Execute()
    {
        if (targetToy == null)
        {
            stateMachine.ChangeState(TypeState.Play);
            return;
        }

        SetDestination(targetToy.transform.position);
    }

    private void CollectToy()
    {
        Destroy(targetToy.gameObject);
        stateMachine.ChangeState(TypeState.Play);
    }
    private void OnTriggerEnter(Collider other)
    {
        EntityIdentity toy = other.GetComponent<EntityIdentity>();
        if (toy != null && toy == targetToy && toy.Entity == Entity.Toy)
        {
            CollectToy();
        }
    }
   
    public override void Exit()
    {
        base.Exit();
        StopMovement();
    }
}