namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/MonoBehaviours/Canvas Face Camera")]
    public class CanvasFaceCamera : MonoBehaviour
    {
        [Header("Important")]
        [Tooltip("Don't follow y axis of the camera (up or down)")]
        [SerializeField] bool ignoreYAxis = false;

        [Header("Override, if you don't want to use defaults")]
        [Tooltip("Default is main camera")]
        [SerializeField] Camera cam;
        [Tooltip("Default is canvas on this object or childs")]
        [SerializeField] Canvas canvas;

        void Start()
        {
            //get main camera
            if (cam == null)
                cam = Camera.main;

            //get canvas on this object or childs
            if (canvas == null)
                canvas = GetComponentInChildren<Canvas>();

            //set world camera
            canvas.worldCamera = cam;
        }

        void Update()
        {
            if (cam && canvas)
            {
                //look at camera
                Vector3 cameraPosition = cam.transform.position;

                //ignore y axis
                if (ignoreYAxis)
                    cameraPosition.y = canvas.transform.position.y;

                //get look rotation
                Vector3 direction = cameraPosition - canvas.transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                //set rotation
                canvas.transform.rotation = rotation;

                //look at camera, but rotate 180 to look same direction (so left of the camera is the same of canvas left)
                //canvas.transform.LookAt(cameraPosition);
                //canvas.transform.Rotate(0, 180, 0);
            }
        }
    }
}