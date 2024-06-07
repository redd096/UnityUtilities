using redd096.Attributes;
using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Component to interact with interactable elements
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/CharacterComponents/Interact Component")]
    public class InteractComponent : MonoBehaviour
    {
        [Header("Necessary Components (by default use main camera and get from this gameObject)")]
        [SerializeField] Camera cam;
        [Tooltip("If raycast hit a rigidbody not Interactable, add draggable component to it")][SerializeField] bool canDragEveryRigidbody;
        [EnableIf("canDragEveryRigidbody")][SerializeField] string draggableComponentType = "DraggableInteractable";
        [SerializeField] float maxDistance = 3;
        [SerializeField] LayerMask interactLayer = -1;
        [Tooltip("Use raycast or sphere cast")][SerializeField] bool useRaycast = false;
        [DisableIf("useRaycast")][SerializeField] float radiusRaycast = 0.2f;
        [SerializeField] QueryTriggerInteraction raycastHitTriggers = QueryTriggerInteraction.UseGlobal;
        [SerializeField] ShowDebugRedd096 gizmosRaycast = Color.cyan;

        //events
        public System.Action onFailInteract;                    //when user try to interact but hit nothing
        public System.Action<IInteractable, bool> onInteract;   //when user interact with an object. Parameters are interactable and returned value from OnInteract
        public System.Action<IInteractable, bool> onDismiss;    //when user try dismiss an interactable. Parameters are interactable and returned value from OnDismiss

        public IInteractable InteractableInUse => interactableInUse;

        //private
        IInteractable interactableInUse;

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (gizmosRaycast)
            {
                Gizmos.color = gizmosRaycast.ColorDebug;
                Transform tr = cam ? cam.transform : transform;
                Gizmos.DrawLine(tr.position, tr.position + tr.forward * maxDistance);
                Gizmos.DrawWireSphere(tr.position + tr.forward * maxDistance, radiusRaycast);
                Gizmos.color = Color.white;
            }
        }
#endif

        protected virtual void Awake()
        {
            if (cam == null) cam = Camera.main;

            //be sure to have components
            if (cam == null)
                Debug.LogError("Miss Camera on " + name);
        }

        /// <summary>
        /// If input is pressed, check if interact or dismiss an interactable
        /// </summary>
        /// <param name="inputPressed"></param>
        public virtual void InteractByInput(bool inputPressed)
        {
            if (inputPressed == false)
                return;

            //try interact
            if (interactableInUse == null)
            {
                if (Raycast(out RaycastHit hit))
                {
                    IInteractable interactable = hit.transform.GetComponentInParent<IInteractable>();

                    //if not an interactable but can drag every rigidbody, makes it an interactable
                    if (interactable == null && canDragEveryRigidbody)
                        interactable = RigidbodyBecomeInteractable(hit.rigidbody);

                    if (interactable != null)
                    {
                        Interact_Internal(interactable, hit.collider);
                        return;
                    }
                }

                onFailInteract?.Invoke();
            }
            //else try dismiss
            else
            {
                Dismiss_Internal();
            }
        }

        /// <summary>
        /// Try to interact with this interactable or dismiss with current interactable
        /// </summary>
        /// <param name="interactable"></param>
        public virtual void InteractWithThisInteractable(IInteractable interactable)
        {
            //try interact
            if (interactableInUse == null)
            {
                if (interactable != null)
                {
                    Interact_Internal(interactable, null);
                    return;
                }

                onFailInteract?.Invoke();
            }
            //else try dismiss
            else
            {
                Dismiss_Internal();
            }
        }

        protected virtual bool Raycast(out RaycastHit hit)
        {
            if (useRaycast)
                return Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistance, interactLayer, raycastHitTriggers);
            else
                return Physics.SphereCast(cam.transform.position, radiusRaycast, cam.transform.forward, out hit, maxDistance, interactLayer, raycastHitTriggers);
        }

        protected virtual IInteractable RigidbodyBecomeInteractable(Rigidbody hitRigidbody)
        {
            if (hitRigidbody == null)
                return null;

            //add Draggable Interactable to rigidbody
            return hitRigidbody.gameObject.AddComponent(System.Type.GetType(draggableComponentType)) as IInteractable;
        }

        protected virtual void Interact_Internal(IInteractable interactable, Collider col)
        {
            if (interactable != null)
            {
                bool result = interactable.OnInteract(this, col, cam.transform);
                onInteract?.Invoke(interactable, result);
                if (result) interactableInUse = interactable;
            }
        }

        protected virtual void Dismiss_Internal()
        {
            if (interactableInUse != null)
            {
                bool result = interactableInUse.OnDismiss(this);
                onDismiss?.Invoke(interactableInUse, result);
                if (result) interactableInUse = null;
            }
        }
    }
}