using UnityEngine;
using System.Collections;

public class ToySpawner : MonoBehaviour
{
    [Header("Cube Settings")]
    public Color cubeLineColor = Color.green;
    public Color cubeFaceColor = new Color(0, 1, 0, 0.1f);

    [Header("Plane Settings")]
    public Color planeColor = Color.red;
    [Range(0, 1)] public float planePosition = 0.5f;

    [Header("Spawning Settings")]
    public GameObject[] toys;
    public float spawnInterval = 1f;

    private int[] randomIndices = new int[100];
    private float[] randomPositions = new float[1000]; // Arreglo para posiciones
    private int currentIndex = 0;
    private int currentPositionIndex = 0; // Índice para posiciones

    private void Start()
    {
        GenerateRandomValues();
        StartCoroutine(SpawnRoutine());
    }

    private void GenerateRandomValues()
    {
        if (toys == null || toys.Length == 0)
        {
            Debug.LogError("No toys assigned!");
            return;
        }

        // Generar índices aleatorios para los juguetes
        for (int i = 0; i < randomIndices.Length; i++)
        {
            randomIndices[i] = Random.Range(0, toys.Length);
        }

        // Generar posiciones aleatorias entre -0.5 y 0.5
        for (int i = 0; i < randomPositions.Length; i++)
        {
            randomPositions[i] = Random.Range(-0.5f, 0.5f);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        SpawnToy();
        currentIndex = (currentIndex + 1) % randomIndices.Length;
        yield return new WaitForSeconds(spawnInterval);
        StartCoroutine(SpawnRoutine());
    }

    private void SpawnToy()
    {
        if (toys == null || toys.Length == 0) return;

        int selectedIndex = randomIndices[currentIndex];
        Vector3 spawnPos = CalculateSpawnPosition();
        Instantiate(toys[selectedIndex], spawnPos, Quaternion.identity);
    }

    private Vector3 CalculateSpawnPosition()
    {
        // Obtener valores precalculados para X y Z
        float x = randomPositions[currentPositionIndex];
        float z = randomPositions[(currentPositionIndex + 1) % randomPositions.Length];

        Vector3 localPosition = new Vector3(
            x,
            planePosition - 0.5f,
            z
        );

        // Avanzar al siguiente par de posiciones
        currentPositionIndex = (currentPositionIndex + 2) % randomPositions.Length;

        return transform.TransformPoint(localPosition);
    }

    // Los métodos de visualización (OnDrawGizmos, DrawCube, DrawPlane) se mantienen igual
    private void OnDrawGizmosSelected()
    {
        DrawCube();
        DrawPlane();
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
        Vector3 planeWorldPosition = transform.TransformPoint(new Vector3(
            0,
            planePosition - 0.5f,
            0
        ));

        Vector3 planeSize = transform.TransformVector(new Vector3(
            1f,
            0.001f,
            1f
        ));

        Gizmos.color = planeColor;
        Gizmos.DrawCube(planeWorldPosition, planeSize);
    }
}