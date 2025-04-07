using TMPro;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State[] allStates;
    [SerializeField] private State currentState = null;
    [SerializeField] private TypeState startState;

    [Header("Show State")]
    [SerializeField] private TMP_Text showState;

    void Start()
    {
        allStates = GetComponents<State>();
        ChangeState(startState);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    public void ChangeState(TypeState newStateType)
    {
        State newState = null;

        foreach (var state in allStates)
        {
            if (state.TypeState == newStateType)
                newState = state;
            else
                state.enabled = false;
        }

        if (newState == null) return;

        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        newState.enabled = true;
        currentState.Enter();

        showState.text = "State: " + currentState.TypeState.ToString();
    }
}
