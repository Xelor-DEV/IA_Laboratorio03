using UnityEngine;
using System;

namespace Assets.ProjectAssets.Scripts
{
    public enum TypeSteeringBehavior
    {
        Seek,
        Flee,
        Evade,
        Pursuit,
        Arrive,
        Wander,
        PathFollowing,
        ObstacleAvoidance
    }

    public class Agent : SteeringBehaviour
    {
        [Header("Agent Settings")]
        [SerializeField] private TypeSteeringBehavior type;
        [SerializeField] private Transform target;
        [Header("Arrive Settings")]
        [SerializeField] protected float slowingRadius = 5f;
        [Header("Wander Settings")]
        [SerializeField] protected float wanderDistance = 3f;  // Distancia frontal del círculo (s)
        [SerializeField] protected float wanderRadius = 1f;    // Radio del círculo (r)
        [SerializeField] protected float wanderDelta = 15f;    // Máximo cambio angular por frame
        protected float wanderAngle = 0f;                      // Ángulo actual en grados
        [Header("Path Following")]
        [SerializeField] private PathFollowing pathFollower;
        // Modificaciones en Agent.cs
        [Header("Obstacle Avoidance")]
        [SerializeField] private float _avoidDistance = 5f;    // Distancia de detección
        [SerializeField] private LayerMask _obstacleMask;      // Capa de obstáculos
        [SerializeField] private float _avoidForce = 5f;       // Fuerza de evasión

        private RaycastHit _hit; // Cambiar a un solo hit
        private Vector3 _rayDirection; // Cambiar a una sola dirección


        // Propiedades para Gizmos
        public TypeSteeringBehavior CurrentBehavior
        {
            get 
            {
                return type;
            }
        }

        public TypeSteeringBehavior Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }
        public Vector3 TargetPosition
        {
            get
            {
                if(target != null)
                {
                    return target.position;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        public Vector3 CurrentVelocity
        {
            get
            {
                return velocity;
            }
        }
        public float MaxSpeed
        {
            get
            {
                return maxSpeed;
            }
        }
        public Transform Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        public float TargetDistance
        {
            get
            {
                if (target != null)
                {
                    return Vector3.Distance(transform.position, target.position);
                }
                else
                {
                    return 0f;
                }
            }
        }
        public Vector3 TargetVelocity
        {
            get
            {
                if (target == null)
                {
                    return Vector3.zero;
                }

                MovableObject movable = target.GetComponent<MovableObject>();

                if (movable != null)
                {
                    return movable.Velocity;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        public Vector3 SeekDirection
        {
            get
            {
                if(target != null)
                {
                    return GetDirectionToTarget(target.position);
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        public Vector3 FleeDirection
        {
            get
            {
                if (target != null)
                {
                    return GetDirectionFromTarget(target.position);
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        public float SlowingRadius
        {
            get
            {
                return slowingRadius;
            }
        }
        public float MaxForce
        {
            get
            {
                return maxForce;
            }
        }
        public float WanderDistance
        {
            get
            {
                return wanderDistance;
            }
        }
        public float WanderRadius
        {
            get
            {
                return wanderRadius;
            }
        }
        public float WanderDelta
        {
            get
            {
                return wanderDelta;
            }
        }
        public float WanderAngle
        {
            get
            {
                return wanderAngle;
            }
        }
        public Vector3 CurrentWanderTarget { get; private set; }
        public PathFollowing PathFollower
        {
            get
            {
                return pathFollower;
            }
        }
        public Vector3[] RayDirections => new Vector3[] { _rayDirection }; // Mantener compatibilidad
        public RaycastHit[] Hits => new RaycastHit[] { _hit }; // Mantener compatibilidad
        public float AvoidDistance => _avoidDistance;
        public float CurrentAvoidForce { get; private set; }

        private TypeSteeringBehavior[] _allBehaviors;
        private int _currentBehaviorIndex = 0;

        // Añadir este método de inicialización
        private void Start()
        {
            // Obtener todos los valores del enum
            _allBehaviors = (TypeSteeringBehavior[])Enum.GetValues(typeof(TypeSteeringBehavior));
            _currentBehaviorIndex = Array.IndexOf(_allBehaviors, type);
        }

        // Modificar el método Update
        private void Update()
        {
            HandleBehaviorSwitch(); // Nuevo método para manejar el cambio
            Vector3 steeringForce = CalculateSteeringForce();
            ApplyForce(steeringForce);
        }

        private void HandleBehaviorSwitch()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _currentBehaviorIndex--;
                if (_currentBehaviorIndex < 0)
                    _currentBehaviorIndex = _allBehaviors.Length - 1;

                type = _allBehaviors[_currentBehaviorIndex];
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                _currentBehaviorIndex++;
                if (_currentBehaviorIndex >= _allBehaviors.Length)
                    _currentBehaviorIndex = 0;

                type = _allBehaviors[_currentBehaviorIndex];
            }
        }

        private Vector3 CalculateSteeringForce()
        {
            switch (type)
            {
                case TypeSteeringBehavior.Seek:
                    return Seek(target.position);
                case TypeSteeringBehavior.Flee:
                    return Flee(target.position);
                case TypeSteeringBehavior.Pursuit:
                    return Pursuit(target);
                case TypeSteeringBehavior.Evade:
                    return Evade(target);
                case TypeSteeringBehavior.Arrive:
                    return Arrive(target.position);
                case TypeSteeringBehavior.Wander:
                    return Wander();
                case TypeSteeringBehavior.PathFollowing:
                    return PathFollow();
                case TypeSteeringBehavior.ObstacleAvoidance:
                    return ObstacleAvoidance() + Seek(target.position);
                default:
                    return Vector3.zero;
            }
        }

        /// Comportamiento de persecución predictiva (Pursuit)
        /// Fórmula: Posición Futura = Posición Actual + Velocidad * Tiempo de Predicción
        /// Tiempo de Predicción = Distancia / Velocidad Máxima
        protected Vector3 Pursuit(Transform target)
        {
            // Obtener componente MovableObject del objetivo
            MovableObject targetMovement = target.GetComponent<MovableObject>();

            if (targetMovement == null)
            {
                return Seek(target.position); // Fallback si no tiene movimiento
            }

            // Calcular vector al objetivo y distancia
            Vector3 toTarget = target.position - transform.position;
            float distance = toTarget.magnitude;

            // Calcular tiempo de predicción (d = v*t => t = d/v)
            float predictionTime = distance / maxSpeed;

            // Calcular posición futura del objetivo
            Vector3 futurePosition = target.position + targetMovement.Velocity * predictionTime;

            // Aplicar Seek a la posición futura
            return Seek(futurePosition);
        }

        /// Comportamiento de evasión predictiva (Evade)
        /// Misma lógica que Pursuit pero aplicando Flee
        protected Vector3 Evade(Transform threat)
        {
            MovableObject threatMovement = threat.GetComponent<MovableObject>();

            if (threatMovement == null)
            {
                return Flee(threat.position); // Fallback si no tiene movimiento
            }

            Vector3 toThreat = threat.position - transform.position;
            float distance = toThreat.magnitude;

            float predictionTime = distance / maxSpeed;
            Vector3 futurePosition = threat.position + threatMovement.Velocity * predictionTime;

            return Flee(futurePosition);
        }

        /// Comportamiento de llegada suavizada (Arrive)
        /// Fórmula: desiredSpeed = maxSpeed * (distance / slowingRadius)
        ///          si distance <= slowingRadius
        /// Método de desaceleración cuadrática
        protected Vector3 Arrive(Vector3 targetPosition)
        {
            Vector3 toTarget = targetPosition - transform.position;
            float distance = toTarget.magnitude;

            // 1. Si está en el punto objetivo, detener completamente
            if (distance < 0.1f)
            {
                velocity = Vector3.zero;
                return Vector3.zero;
            }

            float desiredSpeed = maxSpeed;

            // 2. Calcular velocidad deseada con desaceleración adaptativa
            if (distance <= slowingRadius)
            {
                // Uso de función cuadrática para frenado progresivo
                float t = Mathf.Clamp01(distance / slowingRadius);
                desiredSpeed = maxSpeed * t * t; // t^2 para desaceleración no lineal
            }

            // 3. Calcular vector velocidad deseada
            Vector3 desiredVelocity = toTarget.normalized * desiredSpeed;

            // 4. Calcular fuerza de steering con frenado mejorado
            Vector3 steeringForce = desiredVelocity - velocity;

            // 5. Aumentar fuerza máxima dentro del radio de frenado
            float adjustedMaxForce = distance <= slowingRadius ?
                maxForce * 2f : // Doble fuerza para frenado
                maxForce;

            return Vector3.ClampMagnitude(steeringForce, adjustedMaxForce);
        }

        /// <summary>
        /// Comportamiento de deambular aleatorio (Wander)
        /// Fórmula: p = posición + forward * s
        ///          p' = p + (cos(θ)*r, 0, sin(θ)*r)
        /// </summary>
        protected Vector3 Wander()
        {
            // 1. Calcular punto base del círculo
            Vector3 circleCenter = transform.position + transform.forward * wanderDistance;

            // 2. Actualizar ángulo con variación aleatoria controlada
            wanderAngle += UnityEngine.Random.Range(-wanderDelta, wanderDelta);
            wanderAngle = Mathf.Repeat(wanderAngle, 360f); // Mantener entre 0-360

            // 3. Calcular desplazamiento en el círculo (coordenadas polares a cartesianas)
            float angleRad = wanderAngle * Mathf.Deg2Rad;
            Vector3 displacement = new Vector3(
                Mathf.Cos(angleRad) * wanderRadius,
                0,
                Mathf.Sin(angleRad) * wanderRadius
            );

            // 4. Calcular punto objetivo final
            Vector3 wanderTarget = circleCenter + displacement;
            CurrentWanderTarget = wanderTarget;

            // 5. Aplicar Seek hacia el punto calculado
            return Seek(wanderTarget);
        }
        /// <summary>
        /// Comportamiento de seguimiento de camino
        /// Fórmula: Fuerza = Seek(punto_actual_camino)
        /// </summary>
        private Vector3 PathFollow()
        {
            if (pathFollower == null) return Vector3.zero;

            Transform target = pathFollower.GetNextPoint(transform.position);
            return target != null ? Seek(target.position) : Vector3.zero;
        }


        protected Vector3 ObstacleAvoidance()
        {
            // 1. Calcular dirección del rayo
            _rayDirection = transform.forward;

            // 2. Lanzar rayo central
            if (Physics.Raycast(transform.position, _rayDirection, out _hit, _avoidDistance, _obstacleMask))
            {
                // 3. Calcular fuerza de evasión
                return CalculateAvoidanceForce(_hit);
            }

            return Vector3.zero;
        }
        private Vector3 CalculateAvoidanceForce(RaycastHit hit)
        {
            // 1. Obtener normal del obstáculo
            Vector3 obstacleNormal = hit.normal;
            obstacleNormal.y = 0; // Ignorar componente vertical

            // 2. Calcular dirección de evasión (reflejo de la normal)
            Vector3 avoidDirection = Vector3.Reflect(transform.forward, obstacleNormal).normalized;

            // 3. Calcular fuerza proporcional a la distancia
            float distanceFactor = 1 - (hit.distance / _avoidDistance);

            return avoidDirection * _avoidForce * distanceFactor;
        }
    }
}