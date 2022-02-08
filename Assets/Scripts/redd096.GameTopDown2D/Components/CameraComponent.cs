using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Camera Component")]
    public class CameraComponent : MonoBehaviour
    {
        [Header("Camera - by default is MainCamera")]
        [SerializeField] Camera cameraToControl = default;

        [Header("Update camera position to follow this object")]
        [SerializeField] bool updatePosition = true;
        [Tooltip("Use LateUpdate or Update to move the camera?")] [CanEnable("updatePosition")] [SerializeField] bool useLateUpdate = true;
        [CanEnable("updatePosition")] [SerializeField] Vector3 offsetPosition = new Vector3(0, 0, -10);

        [Header("Clamp camera position to pixelsPerUnit")]
        [SerializeField] bool usePixelClamp = false;
        [CanEnable("usePixelClamp")] [SerializeField] float pixelsPerUnit = 16;

        [Header("Drop Camera On Death (necessary HealthComponent - default get from this gameObject)")]
        [SerializeField] bool dropCameraOnDeath = true;
        [CanEnable("dropCameraOnDeath")] [SerializeField] HealthComponent healthComponent = default;

        Transform cameraParent;
        Vector3 movement;
        Vector3 oldPosition;

        void OnEnable()
        {
            //get references
            if (cameraToControl == null) cameraToControl = Camera.main;
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();

            //set cam parent
            if (cameraToControl && cameraParent == null)
            {
                cameraParent = new GameObject("Camera Parent (camera component)").transform;
                cameraParent.SetParent(cameraToControl.transform.parent);               //set same parent (if camera was child of something)
                cameraParent.localPosition = cameraToControl.transform.localPosition;   //set start local position
                cameraToControl.transform.SetParent(cameraParent);					    //set camera parent
            }

            //add events
            if (healthComponent)
            {
                healthComponent.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            //remove events
            if (healthComponent)
            {
                healthComponent.onDie -= OnDie;
            }
        }

        void Update()
        {
            //update camera position if necessary
            if (updatePosition && useLateUpdate == false)
            {
                if (cameraParent)
                {
                    MoveCamera();
                }
            }
        }

        void LateUpdate()
        {
            //update camera position if necessary
            if (updatePosition && useLateUpdate)
            {
                if (cameraParent)
                {
                    MoveCamera();
                }
            }
        }

        void OnDie(HealthComponent whoDied)
        {
            //drop camera on death, if setted
            if (dropCameraOnDeath && cameraToControl)
            {
                //only if child of this gameObject
                foreach (Camera childCam in GetComponentsInChildren<Camera>())
                {
                    if (childCam == cameraToControl)
                    {
                        cameraToControl.transform.SetParent(null);
                        break;
                    }
                }
            }
        }

        #region private API

        void MoveCamera()
        {
            //move camera clamped to pixelPerUnit
            if (usePixelClamp)
            {
                movement = (transform.position + offsetPosition) - cameraParent.position;   //calculate movement
                movement = PixelClamp(movement);                                            //clamp movement to pixelPerUnit
                oldPosition = PixelClamp(cameraParent.position);                            //clamp old position to pixelsPerUnit

                cameraParent.position = oldPosition + movement;                             //set new position using clamped values
            }
            //just move camera to position + offset
            else
            {
                cameraParent.position = transform.position + offsetPosition;
            }
        }

        Vector3 PixelClamp(Vector3 position)
        {
            //round position * pixelsPerUnit, then divide by pixelsPerUnit
            return new Vector3(Mathf.RoundToInt(position.x * pixelsPerUnit), Mathf.RoundToInt(position.y * pixelsPerUnit), Mathf.RoundToInt(position.z * pixelsPerUnit)) / pixelsPerUnit;
        }

        #endregion
    }
}