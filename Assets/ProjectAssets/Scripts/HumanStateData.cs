using UnityEngine;

[CreateAssetMenu(fileName = "Human State Data", menuName = "ScriptableObjects/Human State Data", order = 1)]
public class HumanStateData : ScriptableObject
{
    [Header("Values")]
    [Range(0f, 1f)] public float currentValue;
    public float maxValue = 1f;

    [Header("Recovery Settings")]
    public float recoveryDuration = 10f;

    [Header("Depletion Settings")]
    public float depletionInterval = 5f;
    [SerializeField] public float currentDepletionTimer = 0f;
}