// SteeringGizmos.cs
using UnityEngine;
using UnityEditor;

namespace Assets.ProjectAssets.Scripts
{
    [ExecuteAlways]
    public class SteeringGizmos : MonoBehaviour
    {
        [SerializeField] private Agent _agent;
        [SerializeField] private Color _seekColor = Color.red;
        [SerializeField] private Color _fleeColor = Color.blue;
        [SerializeField] private Color _pursuitColor = Color.green;
        [SerializeField] private Color _evadeColor = Color.yellow;
        [SerializeField] private Color _arriveColor = Color.cyan;
        [SerializeField] private Color _wanderColor = Color.magenta;
        [SerializeField] private float _arrowSize = 3f;


        private void Awake()
        {
            if (_agent == null)
                _agent = GetComponent<Agent>();
        }

        private void OnDrawGizmos()
        {
            if (_agent == null || !_agent.enabled) return;

            switch (_agent.CurrentBehavior)
            {
                case TypeSteeringBehavior.Seek:
                    DrawSeekGizmos();
                    break;
                case TypeSteeringBehavior.Flee:
                    DrawFleeGizmos();
                    break;
                case TypeSteeringBehavior.Pursuit:
                    DrawPursuitGizmos();
                    break;
                case TypeSteeringBehavior.Evade:
                    DrawEvadeGizmos();
                    break;
                case TypeSteeringBehavior.Arrive:
                    DrawArriveGizmos();
                    break;
                case TypeSteeringBehavior.Wander:
                    DrawWanderGizmos();
                    break;
                case TypeSteeringBehavior.PathFollowing:
                    DrawPathFollowingGizmos();
                    break;
                case TypeSteeringBehavior.ObstacleAvoidance:
                    DrawObstacleAvoidanceGizmos();
                    break;
            }
        }

        private void DrawSeekGizmos()
        {
            if (_agent.Target == null) return;

            Handles.color = _seekColor;
            Handles.ArrowHandleCap(0, _agent.Position,
                Quaternion.LookRotation(_agent.SeekDirection),
                _arrowSize, EventType.Repaint);

            Gizmos.color = _seekColor;
            Gizmos.DrawLine(_agent.Position, _agent.TargetPosition);

            DrawDistanceLabel(_seekColor, "Seek");
        }

        private void DrawFleeGizmos()
        {
            if (_agent.Target == null) return;

            Handles.color = _fleeColor;
            Handles.ArrowHandleCap(0, _agent.Position,
                Quaternion.LookRotation(_agent.FleeDirection),
                _arrowSize, EventType.Repaint);

            Gizmos.color = _fleeColor;
            Gizmos.DrawLine(_agent.Position, _agent.TargetPosition);

            DrawDistanceLabel(_fleeColor, "Flee");
        }

        private void DrawPursuitGizmos()
        {
            if (_agent.Target == null) return;

            Vector3 futurePosition = CalculateFuturePosition();

            // Línea de predicción
            Handles.color = _pursuitColor;
            Handles.DrawDottedLine(_agent.Position, futurePosition, 3f);

            // Curva de interceptación
            Handles.DrawBezier(
                _agent.Position,
                futurePosition,
                _agent.Position + _agent.CurrentVelocity.normalized * 2f,
                futurePosition - _agent.TargetVelocity.normalized * 2f,
                _pursuitColor,
                null,
                2f
            );

            // Etiqueta con datos
            Handles.Label(_agent.Position + Vector3.up * 1.5f,
                $"Persecución\n" +
                $"Velocidad Objetivo: {_agent.TargetVelocity.magnitude:F1}m/s\n" +
                $"Tiempo Predicción: {CalculatePredictionTime():F2}s");
        }

        private void DrawEvadeGizmos()
        {
            if (_agent.Target == null) return;

            Vector3 futurePosition = CalculateFuturePosition();

            // Línea de peligro
            Handles.color = _evadeColor;
            Handles.DrawDottedLine(_agent.Position, futurePosition, 3f);

            // Curva de evasión
            Handles.DrawBezier(
                _agent.Position,
                futurePosition,
                _agent.Position + _agent.CurrentVelocity.normalized * 2f,
                futurePosition - _agent.TargetVelocity.normalized * 2f,
                _evadeColor,
                null,
                2f
            );

            // Etiqueta con datos
            Handles.Label(_agent.Position + Vector3.up * 2f,
                $"Evasión\n" +
                $"Velocidad Amenaza: {_agent.TargetVelocity.magnitude:F1}m/s\n" +
                $"Distancia Futura: {Vector3.Distance(_agent.Position, futurePosition):F2}m");
        }

        // Modificación en SteeringGizmos.cs
        private void DrawArriveGizmos()
        {
            if (_agent.Target == null) return;

            // Radio de frenado con degradado de color
            Handles.color = new Color(_arriveColor.r, _arriveColor.g, _arriveColor.b, 0.3f);
            Handles.DrawSolidDisc(_agent.TargetPosition, Vector3.up, _agent.SlowingRadius);

            // Línea de velocidad actual
            Gizmos.color = Color.Lerp(Color.green, Color.red, _agent.CurrentVelocity.magnitude / _agent.MaxSpeed);
            Gizmos.DrawLine(_agent.Position, _agent.Position + _agent.CurrentVelocity);

            // Línea de velocidad deseada
            Vector3 desiredVelocity = _agent.SeekDirection * CalculateDesiredArriveSpeed();
            Gizmos.color = _arriveColor;
            Gizmos.DrawLine(_agent.Position, _agent.Position + desiredVelocity);

            // Etiqueta con datos dinámicos
            Handles.Label(_agent.Position + Vector3.up * 1.5f,
                $"Arrive State\n" +
                $"Velocidad Actual: {_agent.CurrentVelocity.magnitude:F2}m/s\n" +
                $"Fuerza Frenado: {CalculateBrakingForce():F2}N\n" +
                $"Distancia Objetivo: {_agent.TargetDistance:F2}m");
        }

        private void DrawWanderGizmos()
        {
            if (_agent == null) return;

            // 1. Calcular componentes del Wander
            Vector3 circleCenter = _agent.Position + _agent.transform.forward * _agent.WanderDistance;
            Vector3 wanderTarget = _agent.CurrentWanderTarget;

            // 2. Dibujar línea base al círculo
            Gizmos.color = _wanderColor;
            Gizmos.DrawLine(_agent.Position, circleCenter);

            // 3. Dibujar círculo de Wander
            Handles.color = new Color(_wanderColor.r, _wanderColor.g, _wanderColor.b, 0.3f);
            Handles.DrawSolidDisc(circleCenter, Vector3.up, _agent.WanderRadius);

            // 4. Dibujar dirección actual del ángulo
            Vector3 angleDirection = (wanderTarget - circleCenter).normalized;
            Handles.color = Color.cyan;
            Handles.ArrowHandleCap(0, circleCenter,
                Quaternion.LookRotation(angleDirection),
                1f, EventType.Repaint);

            // 5. Dibujar línea al objetivo final
            Gizmos.color = _wanderColor;
            Gizmos.DrawLine(circleCenter, wanderTarget);
            Gizmos.DrawSphere(wanderTarget, 0.2f);

            // 6. Etiqueta informativa
            Handles.Label(circleCenter + Vector3.up * 0.5f,
                $"Wander Parameters\n" +
                $"Ángulo Actual: {_agent.WanderAngle:F1}°\n" +
                $"Radio: {_agent.WanderRadius:F1}m\n" +
                $"Distancia: {_agent.WanderDistance:F1}m");
        }

        private void DrawPathFollowingGizmos()
        {
            if (_agent == null || _agent.PathFollower == null) return;

            // Dibujar conexión con el punto actual
            Gizmos.color = Color.cyan;
            Transform currentTarget = _agent.PathFollower.GetNextPoint(_agent.Position);
            if (currentTarget != null)
            {
                Gizmos.DrawLine(_agent.Position, currentTarget.position);
                Handles.Label(_agent.Position + Vector3.up * 1.5f,
                    $"Path Following\n" +
                    $"Puntos restantes: {_agent.PathFollower.PathPoints.Count - _agent.PathFollower.CurrentIndex}\n" +
                    $"Distancia actual: {Vector3.Distance(_agent.Position, currentTarget.position):F2}m");
            }
        }

        private float CalculateDesiredArriveSpeed()
        {
            float distance = _agent.TargetDistance;
            return distance <= _agent.SlowingRadius ?
                _agent.MaxSpeed * Mathf.Pow(distance / _agent.SlowingRadius, 2) :
                _agent.MaxSpeed;
        }

        private float CalculateBrakingForce()
        {
            return Mathf.Clamp(
                (_agent.CurrentVelocity.magnitude * _agent.CurrentVelocity.magnitude) /
                (2 * _agent.SlowingRadius),
                0f,
                _agent.MaxForce * 2f
            );
        }

        // SteeringGizmos.cs
        private void DrawObstacleAvoidanceGizmos()
        {
            if (_agent == null) return;

            for (int i = 0; i < _agent.RayDirections.Length; i++)
            {
                Gizmos.color = _agent.Hits[i].collider != null ? Color.red : Color.green;
                Vector3 endPoint = _agent.transform.position + _agent.RayDirections[i] * _agent.AvoidDistance;
                Gizmos.DrawLine(_agent.transform.position, endPoint);

                if (_agent.Hits[i].collider != null)
                {
                    // Dibujar normal del obstáculo
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(_agent.Hits[i].point, _agent.Hits[i].point + _agent.Hits[i].normal);

                    // Dibujar dirección de evasión
                    Gizmos.color = Color.yellow;
                    Vector3 avoidDir = Vector3.Reflect(_agent.transform.forward, _agent.Hits[i].normal);
                    Gizmos.DrawLine(_agent.Hits[i].point, _agent.Hits[i].point + avoidDir * 2f);
                }
            }

            // Etiqueta informativa
            Handles.Label(_agent.transform.position + Vector3.up * 2f,
                $"Obstacle Avoidance\n" +
                $"Fuerza evasión: {_agent.CurrentAvoidForce:F2}");
        }


        private float Remap(float value, float from1, float to1, float from2, float to2, bool clamp = true)
        {
            // 1. Clampear el valor al rango original si es necesario
            if (clamp)
            {
                if (from1 < to1 && value > to1) value = to1;
                if (from1 > to1 && value < to1) value = to1;
                value = Mathf.Clamp(value, Mathf.Min(from1, to1), Mathf.Max(from1, to1));
            }

            // 2. Calcular la proporción
            float proportion = (value - from1) / (to1 - from1);

            // 3. Aplicar al nuevo rango
            return from2 + proportion * (to2 - from2);
        }

        private Vector3 CalculateFuturePosition()
        {
            float predictionTime = CalculatePredictionTime();
            return _agent.TargetPosition + _agent.TargetVelocity * predictionTime;
        }

        private float CalculatePredictionTime()
        {
            return Vector3.Distance(_agent.Position, _agent.TargetPosition) / _agent.MaxSpeed;
        }

        private void DrawDistanceLabel(Color color, string behaviorName)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleCenter;

            Handles.Label(_agent.Position + Vector3.up,
                $"{behaviorName}\nDistancia: {_agent.TargetDistance:F2}m", style);
        }

    }
}