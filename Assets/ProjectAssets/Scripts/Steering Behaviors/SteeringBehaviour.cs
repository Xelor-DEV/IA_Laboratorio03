using UnityEngine;

namespace Assets.ProjectAssets.Scripts
{
    public class SteeringBehaviour : MonoBehaviour
    {
        [Header("Steering Settings")]
        [SerializeField] protected float maxSpeed = 5f;      // Velocidad máxima (MRU)
        [SerializeField] protected float maxForce = 3f;      // Fuerza máxima aplicable
        [SerializeField] protected float acceleration = 2f;  // Aceleración (MRUV)
        protected Vector3 velocity;                          // Velocidad actual del objeto

        /// Calcula fuerza de búsqueda (Seek) usando MRUV
        /// Fórmula: F = m*a (Fuerza = masa * aceleración)
        protected Vector3 Seek(Vector3 targetPosition)
        {
            // Calcular dirección normalizada al objetivo
            Vector3 directionToTarget = GetDirectionToTarget(targetPosition);

            // Calcular velocidad deseada (MRU: V = dirección * velocidad máxima)
            Vector3 desiredVelocity = directionToTarget * maxSpeed;

            // Calcular fuerza de steering (F = Vdeseada - Vactual)
            Vector3 steeringForce = desiredVelocity - velocity;

            // Limitar fuerza máxima (Fuerza resultante <= maxForce)
            return Vector3.ClampMagnitude(steeringForce, maxForce);
        }

        /// Calcula fuerza de huida (Flee) usando MRUV
        /// Fórmula: F = m*a (Fuerza = masa * aceleración)
        protected Vector3 Flee(Vector3 threatPosition)
        {
            // Calcular dirección normalizada desde la amenaza
            Vector3 directionFromThreat = GetDirectionFromTarget(threatPosition);

            // Calcular velocidad deseada (MRU: V = dirección * velocidad máxima)
            Vector3 desiredVelocity = directionFromThreat * maxSpeed;

            // Calcular fuerza de steering (F = Vdeseada - Vactual)
            Vector3 steeringForce = desiredVelocity - velocity;

            // Limitar fuerza máxima (Fuerza resultante <= maxForce)
            return Vector3.ClampMagnitude(steeringForce, maxForce);
        }

        /// Aplica fuerza física al objeto usando MRUV
        /// Fórmula: Vf = Vi + a*t (Euler integration)
        protected void ApplyForce(Vector3 force)
        {
            // Aplicar aceleración a la velocidad (MRUV: Δv = a*t)
            velocity = velocity + (force * acceleration * Time.deltaTime);

            // Suavizado final para velocidades mínimas
            if (velocity.magnitude < 0.1f)
            {
                //velocity = Vector3.zero;
            }
            else
            {

                // Limitar velocidad máxima (|V| <= maxSpeed)
                velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            }

            // Mover objeto (MRU: Δx = v*t)
            transform.position = transform.position + (velocity * Time.deltaTime);

            // Rotación instantánea hacia la dirección de movimiento
            if (velocity != Vector3.zero)
            {
                transform.forward = velocity.normalized;
            }
        }

        /// Obtiene dirección normalizada hacia el objetivo
        /// Fórmula: D = (Target - Position).normalized
        protected Vector3 GetDirectionToTarget(Vector3 targetPosition)
        {
            return (targetPosition - transform.position).normalized;
        }

        /// Obtiene dirección normalizada desde la amenaza
        /// Fórmula: D = (Position - Target).normalized
        protected Vector3 GetDirectionFromTarget(Vector3 threatPosition)
        {
            return (transform.position - threatPosition).normalized;
        }
    }
}