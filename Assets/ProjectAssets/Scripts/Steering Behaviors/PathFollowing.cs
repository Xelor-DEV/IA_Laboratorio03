using UnityEngine;
using System.Collections.Generic;

namespace Assets.ProjectAssets.Scripts
{
    public class PathFollowing : MonoBehaviour
    {
        [Header("Path Settings")]
        [SerializeField] private List<Transform> pathPoints = new List<Transform>();
        [SerializeField] private float arrivalDistance = 0.5f;
        [SerializeField] private bool loopPath = true;

        [Header("Debug")]
        [SerializeField] private int currentIndex = 0;
        [SerializeField] private Color pathColor = Color.green;

        public List<Transform> PathPoints
        {
            get
            {
                return pathPoints;
            }
        }
        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
        }

        /// Obtiene el próximo punto del camino basado en la posición actual
        /// Fórmula: distance = ||posición_actual - punto_actual||
        public Transform GetNextPoint(Vector3 currentPosition)
        {
            if (pathPoints.Count == 0) return null;

            // Calcular distancia al punto actual
            float distance = Vector3.Distance(currentPosition, pathPoints[currentIndex].position);

            if (distance < arrivalDistance)
            {
                if (loopPath)
                    currentIndex = (currentIndex + 1) % pathPoints.Count;
                else
                    currentIndex = Mathf.Clamp(currentIndex + 1, 0, pathPoints.Count - 1);
            }

            return pathPoints[currentIndex];
        }

        private void OnDrawGizmos()
        {
            if (pathPoints == null || pathPoints.Count == 0) return;

            Gizmos.color = pathColor;

            // Dibujar puntos y conexiones
            for (int i = 0; i < pathPoints.Count; i++)
            {
                if (pathPoints[i] == null) continue;

                Gizmos.DrawSphere(pathPoints[i].position, 0.2f);

                if (i < pathPoints.Count - 1 && pathPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                }
            }

            // Resaltar punto actual
            if (currentIndex < pathPoints.Count && pathPoints[currentIndex] != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(pathPoints[currentIndex].position, 0.5f);
            }
        }
    }
}