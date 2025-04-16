using UnityEngine;
using UnityEngine.AI;

public enum Algorithm
{
    None,
    SamplePosition,
    MoveToTargetPosition,
    CalculatePath,
    FindClosestEdge,
    RayCast
}

public class AlgorithmTest : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Algorithm currentAlgorithm = Algorithm.None;
    [Header("Sample Position Variables")]
    [SerializeField] private float range = 10;
    [SerializeField] private float radiusSphere = 1;
    [SerializeField] private Color gizmosColor = Color.red;
    [SerializeField] private int executionAttempts = 30;
    [Header("Move To Target Position Variables")]
    [SerializeField] private Transform targetTransform;
    [Header("Calculate Path Variables")]
    [SerializeField] private Transform pathTargetTransform;
    [SerializeField] private Color pathColor = Color.magenta;
    private NavMeshPath calculatedPath;

    [Header("Debug Variables")]
    [SerializeField] private Algorithm lastExecutedAlgorithm = Algorithm.None;
    [SerializeField] private Vector3 lastSamplePosition;
    [SerializeField] private Vector3 lastTargetPosition;
    [SerializeField] private Vector3 lastMoveToPosition;

    private int currentAlgorithmIndex = 0;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        calculatedPath = new NavMeshPath();
    }

    private void Update()
    {
        HandleAlgorithmSelection();
        ExecuteCurrentAlgorithm();
        HandleReExecution();
    }

    private void HandleAlgorithmSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAlgorithmIndex = (currentAlgorithmIndex + 1) % System.Enum.GetValues(typeof(Algorithm)).Length;
            currentAlgorithm = (Algorithm)currentAlgorithmIndex;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAlgorithmIndex--;
            if (currentAlgorithmIndex < 0)
                currentAlgorithmIndex = System.Enum.GetValues(typeof(Algorithm)).Length - 1;

            currentAlgorithm = (Algorithm)currentAlgorithmIndex;
        }
    }

    private void ExecuteCurrentAlgorithm()
    {
        if (currentAlgorithm == Algorithm.None) return;

        switch (currentAlgorithm)
        {
            case Algorithm.SamplePosition:
                ExecuteSamplePosition();
                break;
            case Algorithm.MoveToTargetPosition:
                ExecuteMoveToTargetPosition();
                break;
            case Algorithm.CalculatePath:
                ExecuteCalculatePath();
                break;
        }

        lastExecutedAlgorithm = currentAlgorithm;
        currentAlgorithm = Algorithm.None;
    }

    private void HandleReExecution()
    {
        if (Input.GetKeyDown(KeyCode.R) && lastExecutedAlgorithm != Algorithm.None)
        {
            currentAlgorithm = lastExecutedAlgorithm;
        }
    }

    private void ExecuteSamplePosition()
    {
        if (SamplePosition(transform.position, range, out lastSamplePosition))
        {
            if (agent != null)
            {
                agent.SetDestination(lastSamplePosition);
                lastTargetPosition = lastSamplePosition;
            }
        }
    }

    private void ExecuteMoveToTargetPosition()
    {
        if (targetTransform == null)
        {
            Debug.Log("No assigned target transform");
            return;
        }

        MoveToTargetPosition(targetTransform.position);
    }

    private void ExecuteCalculatePath()
    {
        if (pathTargetTransform == null)
        {
            Debug.Log("No path target assigned");
            return;
        }

        CalculatePath(pathTargetTransform.position);
    }

    private bool SamplePosition(Vector3 positionCenter, float range, out Vector3 resultPosition)
    {
        for (int i = 0; i < executionAttempts; ++i)
        {
            Vector3 randomPoint = positionCenter + Random.insideUnitSphere * range;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, radiusSphere, NavMesh.AllAreas))
            {
                resultPosition = hit.position;
                return true;
            }
        }

        resultPosition = Vector3.zero;
        return false;
    }

    private void MoveToTargetPosition(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, radiusSphere, NavMesh.AllAreas))
        {
            lastMoveToPosition = hit.position;

            agent.SetDestination(hit.position);
            agent.isStopped = false;
        }
        else
        {
            Debug.Log("Target position is not in the NavMesh");
            agent.isStopped = true;
        }
    }

    private void CalculatePath(Vector3 targetPosition)
    {
        agent.ResetPath();

        if (NavMesh.CalculatePath(agent.transform.position, targetPosition, NavMesh.AllAreas, calculatedPath))
        {
            if (calculatedPath.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(calculatedPath);
                Debug.Log("Path calculated successfully!");
            }
            else
            {
                Debug.LogWarning("Partial path. Target may not be reachable");
            }
        }
        else
        {
            Debug.LogError("Failed to calculate path");
        }
    }

    private void OnDrawGizmos()
    {
        if (lastExecutedAlgorithm == Algorithm.SamplePosition)
        {
            // Draw search area
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, range);

            // Draw found position
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(lastSamplePosition, radiusSphere);

            // Draw path to target
            if (agent != null && agent.hasPath)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, lastTargetPosition);
            }
        }
        else if (lastExecutedAlgorithm == Algorithm.MoveToTargetPosition)
        {
            // Visualización para MoveToTargetPosition
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(lastMoveToPosition, radiusSphere);

            if (targetTransform != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, targetTransform.position);
            }
        }
        else if (lastExecutedAlgorithm == Algorithm.CalculatePath)
        {
            // Dibujar el path calculado
            if (calculatedPath != null && calculatedPath.corners.Length > 1)
            {
                Gizmos.color = pathColor;
                for (int i = 0; i < calculatedPath.corners.Length - 1; i++)
                {
                    Gizmos.DrawSphere(calculatedPath.corners[i], 0.3f);
                    Gizmos.DrawLine(calculatedPath.corners[i], calculatedPath.corners[i + 1]);
                }

                // Dibujar conexión con el objetivo final
                Gizmos.color = Color.white;
                Gizmos.DrawLine(calculatedPath.corners[calculatedPath.corners.Length - 1],
                               pathTargetTransform.position);
            }

            // Dibujar objetivo del path
            if (pathTargetTransform != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(pathTargetTransform.position, 1f);
            }
        }
    }
}