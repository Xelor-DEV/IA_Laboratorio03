using UnityEngine;


public class State : MonoBehaviour
{
    public TypeState typestate;
    public StateMachine _StateMachine;
    public virtual void LocadComponent()
    {
        _StateMachine = GetComponent<StateMachine>();
    }
    public virtual void Enter()
    {
        
    }
    public virtual void Execute()
    {

    }
    public virtual void Exit()
    {
     
    }
}
