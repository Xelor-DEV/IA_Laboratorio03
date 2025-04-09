using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public float speed = 5f;          // Velocidad de movimiento
    public Camera povCamera;           // Referencia a la cámara en primera persona

    private Vector2 moveInput;
    private float verticalInput;

    void Start()
    {
        // Asignar automáticamente la cámara principal si no está asignada
        if (povCamera == null)
            povCamera = Camera.main;
    }

    void Update()
    {
        if (povCamera == null) return;

        // Obtener direcciones de la cámara
        Vector3 cameraForward = povCamera.transform.forward;
        Vector3 cameraRight = povCamera.transform.right;

        // Calcular dirección de movimiento relativa a la cámara
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // Movimiento vertical (Space/Y)
        Vector3 verticalMovement = Vector3.up * verticalInput;

        // Combinar y aplicar movimiento
        Vector3 totalMovement = (moveDirection + verticalMovement) * speed * Time.deltaTime;
        transform.Translate(totalMovement, Space.World);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnVerticalMove(InputAction.CallbackContext context)
    {
        verticalInput = context.ReadValue<Vector2>().y;
    }
}