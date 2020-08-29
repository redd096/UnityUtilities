namespace redd096
{
    using UnityEngine;

    public class CanvasFaceCamera : MonoBehaviour
    {
        [Header("Important")]
        [Tooltip("Enable or disable Update")]
        [SerializeField] bool lookAlwaysCamera = true;
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

            //if not look to camera, disable update - else enable it
            this.enabled = lookAlwaysCamera;
        }

        void Update()
        {
            if (cam)
            {
                //look at camera
                Vector3 position = cam.transform.position;

                //ignore y axis
                if (ignoreYAxis)
                    position.y = canvas.transform.position.y;

                //look at camera, but rotate 180 to look same direction (so left of the camera is the same of canvas left)
                canvas.transform.LookAt(position);
                canvas.transform.Rotate(0, 180, 0);
            }
        }
    }
}