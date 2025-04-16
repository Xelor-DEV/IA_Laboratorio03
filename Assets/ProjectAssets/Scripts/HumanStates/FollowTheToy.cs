using UnityEngine;

public class FollowTheToy : Human
{
    private Health targetToy;
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

        // Actualizar destino si el juguete se mueve
        SetDestination(targetToy.transform.position);
        /*
        if (HasReachedDestination())
        {
            CollectToy();
        }
        */
    }

    private void CollectToy()
    {
        Destroy(targetToy.gameObject);
        stateMachine.ChangeState(TypeState.Play);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health toy = other.GetComponent<Health>();
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