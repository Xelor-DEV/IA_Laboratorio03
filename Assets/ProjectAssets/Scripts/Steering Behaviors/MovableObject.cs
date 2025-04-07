using UnityEngine;
using TMPro;

namespace Assets.ProjectAssets.Scripts
{
    public class MovableObject : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private Vector3 velocity;
        [SerializeField] private TMP_Text debugText;
        [SerializeField] private Camera mainCamera;

        private Vector3 _previousPosition;
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        void Update()
        {
            velocity = (transform.position - _previousPosition) / Time.deltaTime;
            _previousPosition = transform.position;
            debugText.text = velocity.ToString();

            if (mainCamera != null)
            {
                OrientTextToCamera();
            }
        }

        private void OrientTextToCamera()
        {
            Vector3 directionToCamera = mainCamera.transform.position - debugText.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);

            debugText.transform.rotation = targetRotation;
        }
    }
}