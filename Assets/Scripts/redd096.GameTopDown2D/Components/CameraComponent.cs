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
        [CanEnable("updatePosition")] [SerializeField] Vector3 offsetPosition = new Vector3(0, 0, -10);

        [Header("Drop Camera On Death (necessary HealthComponent - default get from this gameObject)")]
        [SerializeField] bool dropCameraOnDeath = true;
        [CanEnable("dropCameraOnDeath")] [SerializeField] HealthComponent healthComponent = default;

        Transform cameraParent;

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
            if (updatePosition && cameraParent)
                cameraParent.position = transform.position + offsetPosition;
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
    }
}