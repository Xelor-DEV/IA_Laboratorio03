using UnityEngine;

public class State : MonoBehaviour
{
    [SerializeField] protected StateMachine stateMachine;
    [SerializeField] protected TypeState typeState;

    public TypeState TypeState
    {
        get 
        { 
            return typeState; 
        }
        set 
        { 
            typeState = value; 
        }
    }

    public virtual void LoadComponents()
    {
        stateMachine = GetComponent<StateMachine>();
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
