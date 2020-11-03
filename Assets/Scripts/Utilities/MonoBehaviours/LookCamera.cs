namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/MonoBehaviours/Look Camera")]
    public class LookCamera : MonoBehaviour
    {
        [Header("Important")]
        [Tooltip("Don't follow y axis of the camera (up or down)")]
        [SerializeField] bool ignoreYAxis = false;

        [Header("Override, if you don't want to use defaults")]
        [Tooltip("Default is main camera")]
        [SerializeField] Camera cam;
        [Tooltip("Default is this gameObject")]
        [SerializeField] GameObject objectToRotate;

        void Start()
        {
            //get main camera
            if (cam == null)
                cam = Camera.main;

            //get this gameObject
            if (objectToRotate == null)
                objectToRotate = gameObject;
        }

        void Update()
        {
            if (cam && objectToRotate)
            {
                //look at camera
                Vector3 cameraPosition = cam.transform.position;

                //ignore y axis
                if (ignoreYAxis)
                    cameraPosition.y = objectToRotate.transform.position.y;

                //get look rotation
                Vector3 direction = cameraPosition - objectToRotate.transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                //set rotation
                objectToRotate.transform.rotation = rotation;
            }
        }
    }
}