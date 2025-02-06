namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Subscribes to an <see cref="ARRaycastHitEventAsset"/>. When the event is raised, the
    /// <see cref="prefabToPlace"/> is instantiated at, or moved to, the hit position.
    /// </summary>
    public class ARPlaceObject : MonoBehaviour
    {
        const float k_PrefabHalfSize = 0.025f;

        [SerializeField]
        [Tooltip("The prefab to be placed or moved.")]
        GameObject m_PrefabToPlace;

        [SerializeField]
        [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
        ARRaycastHitEventAsset m_RaycastHitEvent;

        [SerializeField]
        GameObject m_ArrowObject;

        GameObject m_SpawnedObject;
        PlaneDetectionController planeDetectionController;
        bool isPlaced = false;
        private Transform cameraTransform;

        /// <summary>
        /// The prefab to be placed or moved.
        /// </summary>
        public GameObject prefabToPlace
        {
            get => m_PrefabToPlace;
            set => m_PrefabToPlace = value;
        }

        /// <summary>
        /// The spawned prefab instance.
        /// </summary>
        public GameObject spawnedObject
        {
            get => m_SpawnedObject;
            set => m_SpawnedObject = value;
        }

        void OnEnable()
        {
            planeDetectionController = gameObject.GetComponent<PlaneDetectionController>();
            if (m_RaycastHitEvent == null || m_PrefabToPlace == null)
            {
                Debug.LogWarning($"{nameof(ARPlaceObject)} component on {name} has null inputs and will have no effect in this scene.", this);
                return;
            }

            if (m_RaycastHitEvent != null)
                m_RaycastHitEvent.eventRaised += PlaceObjectAt;
        }

        void OnDisable()
        {
            if (m_RaycastHitEvent != null)
                m_RaycastHitEvent.eventRaised -= PlaceObjectAt;
        }

        public void PlaceObjectAt(object sender, ARRaycastHit hitPose)
        {
            if (m_PrefabToPlace == null) return;

            cameraTransform = Camera.main.transform;

            if (m_SpawnedObject == null)
            {
                // Spawn the object at the arrow's position
                m_SpawnedObject = Instantiate(m_PrefabToPlace, m_ArrowObject.transform.position, Quaternion.identity);
            }

            // Face the character towards the camera
            Vector3 targetPosition = new Vector3(cameraTransform.position.x, m_SpawnedObject.transform.position.y, cameraTransform.position.z);
            m_SpawnedObject.transform.LookAt(targetPosition);

            // Apply rotation fix (adjust values if needed)
            m_SpawnedObject.transform.Rotate(0, 70f, 0);

            // Optional: Reset parent to avoid AR anchor affecting rotation
            m_SpawnedObject.transform.parent = null;

            // Disable plane detection and hide the arrow
            if (!isPlaced)
            {
                isPlaced = true;
                planeDetectionController?.TogglePlaneDetection();
                m_ArrowObject.SetActive(false);
            }
        }
    }
}
