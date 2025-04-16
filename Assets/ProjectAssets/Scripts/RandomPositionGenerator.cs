using UnityEngine;

public class RandomPositionGenerator : MonoBehaviour
{
    [Header("Cube Settings")]
    public Color cubeLineColor = Color.green;
    public Color cubeFaceColor = new Color(0, 1, 0, 0.1f);

    [Header("Plane Settings")]
    public Color planeColor = Color.red;
    [Range(0, 1)] public float planePosition = 0.5f;

    [Header("Generator Settings")]
    [SerializeField] private int precomputedPositions = 1000;

    private float[] randomX;
    private float[] randomZ;
    private int currentIndex;
    private Vector3 lastGeneratedPosition;

    private void Awake()
    {
        GenerateRandomPositions();
    }

    private void GenerateRandomPositions()
    {
        randomX = new float[precomputedPositions];
        randomZ = new float[precomputedPositions];

        for (int i = 0; i < precomputedPositions; i++)
        {
            randomX[i] = Random.Range(-0.5f, 0.5f);
            randomZ[i] = Random.Range(-0.5f, 0.5f);
        }
    }

    public Vector3 GetNextPosition()
    {
        Vector3 localPosition = new Vector3(
            randomX[currentIndex],
            planePosition - 0.5f,
            randomZ[currentIndex]
        );

        lastGeneratedPosition = transform.TransformPoint(localPosition);
        currentIndex = (currentIndex + 1) % precomputedPositions;
        return lastGeneratedPosition;
    }

    private void OnDrawGizmos()
    {
        DrawCube();
        DrawPlane();
        DrawPositionSphere();
    }

    private void DrawCube()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Color originalColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = cubeFaceColor;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = cubeLineColor;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = originalMatrix;
        Gizmos.color = originalColor;
    }

    private void DrawPlane()
    {
        Vector3 planeLocalPosition = new Vector3(0, planePosition - 0.5f, 0);
        Vector3 planeWorldPosition = transform.TransformPoint(planeLocalPosition);

        Vector3 planeSize = transform.TransformVector(new Vector3(
            1f,
            0.001f,
            1f
        ));

        Gizmos.color = planeColor;
        Gizmos.DrawCube(planeWorldPosition, planeSize);
    }

    private void DrawPositionSphere()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(lastGeneratedPosition, 0.1f);
    }
}