using redd096.Attributes;
using UnityEngine;

namespace redd096.v1.Game3D
{
    /// <summary>
    /// Move and rotate camera to follow character
    /// </summary>
    [AddComponentMenu("redd096/v1/Game3D/CharacterComponents/Camera FirstPerson Component")]
    public class CameraFirstPersonComponent : MonoBehaviour
    {
        [Header("Necessary Components (by default use main camera and get from this gameObject)")]
        [SerializeField] Transform cam;
        [SerializeField] Transform characterToFollow;
        [SerializeField] bool calculateOffsetInAwake = true;
        [DisableIf("calculateOffsetInAwake")][SerializeField] Vector3 cameraOffset = new Vector3(0, 0.4f, 0.4f);
        [SerializeField] RotationComponent rotationComponent;

        protected virtual void Awake()
        {
            //get default values
            if (cam == null) cam = Camera.main.transform;
            if (characterToFollow == null) characterToFollow = transform;

            //be sure to have components
            if (cam == null)
                Debug.LogError("Miss Camera on " + name);
            if (characterToFollow == null)
                Debug.LogError("Miss characterToFollow on " + name);
            if (rotationComponent == null && TryGetComponent(out rotationComponent) == false)
                Debug.LogError("Miss Rotation Component on " + name);

            //calculate offset if necessary
            if (calculateOffsetInAwake)
            {
                cameraOffset = cam.position - characterToFollow.transform.position;
            }
        }

        protected virtual void LateUpdate()
        {
            //move cam
            Vector3 cameraOffsetRotated = Quaternion.AngleAxis(cam.eulerAngles.y, Vector3.up) * cameraOffset;
            cam.position = characterToFollow.position + cameraOffsetRotated;

            //rotate cam
            cam.localRotation = rotationComponent.XQuat * rotationComponent.YQuat;
        }
    }
}