using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State[] states ;
    public State currentState = null;
    public TypeState Startstate;
    // Start is called before the first frame update
    void Start()
    {
        states = GetComponents<State>();
         
        ChangeState(Startstate);
    }

    public void ChangeState(TypeState type)
    {
        foreach (var state in states) {

            if (((State)state).typestate == type)
            {
                if (currentState != null)
                    currentState.Exit();

                ((State)state).Enter();
                currentState = ((State)state);
                state.enabled = true;

            }
            else
            {
                state.enabled = false;
            }
        }
    }


    private void Update()
    {
        if(currentState!=null)
        {
            currentState.Execute();
        }
    }
}
