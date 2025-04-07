using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Alinea el forward del objeto con el de la c�mara
        transform.forward = mainCamera.transform.forward;
    }
}