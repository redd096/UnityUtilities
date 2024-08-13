using redd096.Attributes;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Use this component to move a topdown camera
    /// </summary>
    [System.Serializable]
    public class TopDownCameraComponent : ICharacterComponent
    {
        [Header("Necessary Components (by default use main camera and owner transform)")]
        [SerializeField] Transform cam;
        [SerializeField] Transform objectToFollow;
        [SerializeField] bool calculateOffsetInAwake = true;
        [DisableIf("calculateOffsetInAwake")][SerializeField] Vector3 cameraOffset = new Vector3(0, 0.4f, 0.4f);

        public ICharacter Owner { get; set; }
        public Transform Cam { get => cam; set => cam = value; }
        public Transform ObjectToFollow { get => objectToFollow; set => objectToFollow = value; }
        public Vector3 CameraOffset { get => cameraOffset; set => cameraOffset = value; }

        Quaternion startRotation;

        public void Awake()
        {
            //get default values
            if (cam == null) cam = Camera.main.transform;
            if (objectToFollow == null) objectToFollow = Owner.transform;

            //be sure to have components
            if (cam == null)
                Debug.LogError("Miss Camera on " + GetType().Name);
            if (objectToFollow == null)
                Debug.LogError("Miss characterToFollow on " + GetType().Name);

            //calculate offset if necessary
            if (calculateOffsetInAwake)
            {
                RecalculateOffset();
            }

            //calculate start rotation
            RecalculateStartRotation();
        }

        /// <summary>
        /// Calculate offset between camera and objectToFollow
        /// </summary>
        public void RecalculateOffset()
        {
            cameraOffset = cam.position - objectToFollow.transform.position;
        }

        /// <summary>
        /// Calculate camera start rotation. So when user call UpdateCameraRotation, add player rotation to start rotation
        /// </summary>
        public void RecalculateStartRotation()
        {
            startRotation = cam.localRotation;
        }

        /// <summary>
        /// Tell camera to moves to follow object
        /// </summary>
        public void UpdateCameraPosition()
        {
            cam.position = objectToFollow.position + cameraOffset;
        }

        /// <summary>
        /// Tell camera to moves and rotates by following object
        /// </summary>
        public void UpdateCameraPositionAndRotation()
        {
            //rotate cam
            cam.localRotation = Quaternion.AngleAxis(objectToFollow.eulerAngles.y, Vector3.up) * startRotation;

            //move cam
            Vector3 cameraOffsetRotated = Quaternion.AngleAxis(cam.eulerAngles.y, Vector3.up) * cameraOffset;
            cam.position = objectToFollow.position + cameraOffsetRotated;

        }
    }
}