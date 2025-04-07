using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public float speed = 5f;          // Velocidad de movimiento
    public float rotationSpeed = 10f; // Velocidad de rotación
    public Camera playerCamera;       // Referencia a la cámara en tercera persona

    private Vector2 moveInput;

    void Start()
    {
        // Asignar automáticamente la cámara principal si no está asignada
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (playerCamera == null) return;

        // Obtener direcciones de la cámara (proyectadas en el plano horizontal)
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // Calcular dirección de movimiento relativa a la cámara
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // Aplicar movimiento
        if (moveDirection != Vector3.zero)
        {
            // Mover el objeto
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

            // Rotar el objeto hacia la dirección del movimiento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}