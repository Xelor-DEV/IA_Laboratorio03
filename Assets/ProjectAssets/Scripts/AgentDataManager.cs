using System.Collections;
using UnityEngine;

[System.Serializable]
public class TransitionVariable
{
    [Range(0, 1)] public float current;

    [Header("Increase Settings")]
    public float increaseAmount = 0.1f;
    public float increaseTime = 1f;
    public bool increaseEnabled;

    [Header("Decrease Settings")]
    public float decreaseAmount = 0.1f;
    public float decreaseTime = 1f;
    public bool decreaseEnabled;

    public void Update()
    {
        if (increaseEnabled)
        {
            float rate = increaseAmount / Mathf.Max(0.001f, increaseTime);
            current += rate * Time.deltaTime;
        }

        if (decreaseEnabled)
        {
            float rate = decreaseAmount / Mathf.Max(0.001f, decreaseTime);
            current -= rate * Time.deltaTime;
        }

        current = Mathf.Clamp01(current);
    }
}

public class AgentDataManager : MonoBehaviour
{
    public bool isPaused = false;
    public TransitionVariable energy;
    public TransitionVariable hunger;
    public TransitionVariable bladder;

    private void Update()
    {
        if (isPaused) return;

        energy.Update();
        hunger.Update();
        bladder.Update();
    }
}

