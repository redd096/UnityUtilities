using redd096.Attributes;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Use this component to let a camera follow an object
    /// </summary>
    [System.Serializable]
    public class CameraFollowTargetComponent : IComponentRD
    {
        [Header("Necessary Components (by default use main camera and owner transform)")]
        [SerializeField] Transform cam;
        [SerializeField] Transform objectToFollow;
        [SerializeField] bool createCameraParentOnAwake = true;
        [SerializeField] bool calculateOffsetInAwake = true;
        [DisableIf("calculateOffsetInAwake")][SerializeField] Vector3 cameraOffset = new Vector3(0, 0.4f, 0.4f);

        public IGameObjectRD Owner { get; set; }
        public Transform Cam { get => cam; set => cam = value; }
        public Transform ObjectToFollow { get => objectToFollow; set => objectToFollow = value; }
        public Vector3 CameraOffset { get => cameraOffset; set => cameraOffset = value; }

        Quaternion startRotation;

        public void AwakeRD()
        {
            //get default values
            if (cam == null) cam = Camera.main.transform;
            if (objectToFollow == null) objectToFollow = Owner.transform;

            //be sure to have components
            if (cam == null)
                Debug.LogError("Miss Camera on " + GetType().Name, Owner.transform.gameObject);
            if (objectToFollow == null)
                Debug.LogError("Miss characterToFollow on " + GetType().Name, Owner.transform.gameObject);

            //create camera parent
            if (createCameraParentOnAwake)
                CreateCameraParent();

            //calculate offset if necessary
            if (calculateOffsetInAwake)
            {
                RecalculateOffset();
            }

            //calculate start rotation
            RecalculateStartRotation();
        }

        /// <summary>
        /// Create an empty object and set it as camera parent. 
        /// This is necessary because if you have more components that move the camera (e.g. camera shake), every component can move a parent instead of the camera, to not have conflicts
        /// </summary>
        public void CreateCameraParent(string cameraParentName = "Camera Parent (camera component)")
        {
            Transform cameraParent = new GameObject(cameraParentName).transform;
            cameraParent.SetParent(cam.parent);             //set same parent (if camera was child of something)
            cameraParent.position = cam.position;           //set start position
            cam.SetParent(cameraParent);                    //set camera parent

            //move camera parent instead of camera
            cam = cameraParent;
        }

        /// <summary>
        /// Calculate offset between camera and objectToFollow
        /// </summary>
        public void RecalculateOffset()
        {
        cameraOffset = cam.position - objectToFollow.position;
        }

        /// <summary>
        /// Calculate camera start rotation. So when user call UpdateCameraRotation, add objectToFollow rotation to start rotation
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
        /// Tell camera to moves and rotates by following object (rotate only on Y axis). 
        /// (NB we add the objectToFollow rotation, so the start rotation must be calculated with objectToFollow at rotation 0)
        /// </summary>
        public void UpdateCameraPositionAndRotationTopDown()
        {
            //rotate cam
            cam.localRotation = Quaternion.AngleAxis(objectToFollow.eulerAngles.y, Vector3.up) * startRotation;

            //move cam
            Vector3 cameraOffsetRotated = Quaternion.AngleAxis(cam.eulerAngles.y, Vector3.up) * cameraOffset;
            cam.position = objectToFollow.position + cameraOffsetRotated;

        }

        /// <summary>
        /// Tell camera to moves by following object and rotate to direction
        /// </summary>
        public void UpdateCameraPositionAndRotationInDirection(Vector3 camDirection)
        {
            //rotate cam
            cam.localRotation = Quaternion.LookRotation(camDirection, Vector3.up);

            //move cam
            Vector3 cameraOffsetRotated = Quaternion.AngleAxis(cam.eulerAngles.y, Vector3.up) * cameraOffset;
            cam.position = objectToFollow.position + cameraOffsetRotated;
        }

        /// <summary>
        /// Tell camera to rotate to look at direction, and move around the object to follow
        /// </summary>
        /// <param name="camDirection"></param>
        public void RotateAroundPivot(Vector3 camDirection)
        {
            Quaternion previousRotation = cam.localRotation;

            //rotate cam
            cam.localRotation = Quaternion.LookRotation(camDirection, Vector3.up);

            //calculate new position
            Quaternion angleRotation = cam.localRotation * Quaternion.Inverse(previousRotation);
            Vector3 offset = cam.position - objectToFollow.position;
            offset = angleRotation * offset;

            //move
            cam.position = objectToFollow.position + offset;
        }

        /// <summary>
        /// Move forward or back to object to follow position. Clamp if reach it
        /// </summary>
        /// <param name="zoomInput">move forward if > 0.5, move back if < 0.5f, else stay still</param>
        /// <param name="zoomSpeed"></param>
        /// <param name="minDistance">when zoom in, stop to this distance from the object to follow. -1 = no limit</param>
        /// <param name="maxDistance">when zoom out, stop to this distance from the object to follow. -1 = no limit</param>
        public void ZoomToPivot(float zoomInput, float zoomSpeed, float minDistance, float maxDistance)
        {
            //calculate direction
            Vector3 direction = Vector3.zero;
            Vector3 tempDir = objectToFollow.position - cam.position == Vector3.zero ? cam.forward : (objectToFollow.position - cam.position).normalized;
            if (zoomInput > 0.5f)
                direction = tempDir;
            else if (zoomInput < -0.5f)
                direction = -tempDir;

            Vector3 movement = direction * zoomSpeed;

            //clamp zoom in
            if (zoomInput > 0.5f)
            {
                if (minDistance >= 0)
                {
                    ////clamp, can't exceed object to follow position
                    //float currentDistance = (cam.position - objectToFollow.position).sqrMagnitude;
                    //if (movement.sqrMagnitude > currentDistance)
                    //    movement = Vector3.ClampMagnitude(movement, Vector3.Distance(cam.position, objectToFollow.position));

                    float currentDistance = (cam.position - objectToFollow.position).sqrMagnitude;
                    float distanceToNextPosition = (cam.position + movement - cam.position).sqrMagnitude;
                    if (distanceToNextPosition > currentDistance                                                //if exceed object to follow position, it's already too much
                        || Vector3.Distance(cam.position + movement, objectToFollow.position) < minDistance)    //else check normally distance
                    {
                        cam.position = objectToFollow.position + direction * minDistance;
                        return;
                        //movement = Vector3.ClampMagnitude(movement, Vector3.Distance(cam.position, objectToFollow.position) - minDistance);
                    }
                }
            }
            //clamp zoom out
            else if (zoomInput < -0.5f)
            {
                if (maxDistance >= 0)
                {
                    if (Vector3.Distance(cam.position + movement, objectToFollow.position) > maxDistance)
                    {
                        cam.position = objectToFollow.position + direction * maxDistance;
                        return;
                        //movement = Vector3.ClampMagnitude(movement, maxDistance - Vector3.Distance(cam.position, objectToFollow.position));
                    }
                }
            }

            //move
            cam.position += movement;
        }
    }
}