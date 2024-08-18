using redd096.Attributes;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Find interactables in front of owner by using raycast, and call function to interact
    /// </summary>
    [System.Serializable]
    public class InteractComponentRaycast : IObjectComponent
    {
        [Header("Necessary Components (by default use main camera and get from this gameObject)")]
        [SerializeField] Camera cam;
        [SerializeField] float maxDistance = 3;
        [Tooltip("Hit only interacts with this layer")][SerializeField] LayerMask interactLayer = -1;
        [Tooltip("Use raycast or sphere cast")][SerializeField] bool useRaycast = false;
        [DisableIf("useRaycast")][SerializeField] float radiusSphereCast = 0.02f;
        [SerializeField] QueryTriggerInteraction raycastHitTriggers = QueryTriggerInteraction.UseGlobal;
        [SerializeField] ShowDebugRedd096 gizmosRaycast = Color.cyan;

        public IObject Owner { get; set; }

        //events
        public System.Action<IInteractable> onFoundInteractable;
        public System.Action<IInteractable> onLostInteractable;
        public System.Action<IInteractable> onInteract;             //when user interact with CurrentInteractable
        public System.Action onFailInteract;                        //when user try to interact but CurrentInteractable is null

        public IInteractable CurrentInteractable;

        public void OnDrawGizmosSelected()
        {
            if (gizmosRaycast)
            {
                Gizmos.color = gizmosRaycast.ColorDebug;
                Transform tr = cam ? cam.transform : Owner.transform;
                Gizmos.DrawLine(tr.position, tr.position + tr.forward * maxDistance);
                Gizmos.DrawWireSphere(tr.position + tr.forward * maxDistance, radiusSphereCast);
                Gizmos.color = Color.white;
            }
        }

        public void Awake()
        {
            if (cam == null) cam = Camera.main;

            //be sure to have components
            if (cam == null)
                Debug.LogError($"Miss Camera on {GetType().Name}", Owner.transform.gameObject);
        }

        /// <summary>
        /// Find interactables in front of owner by using raycast and set Current Interactable
        /// </summary>
        public void ScanInteractables()
        {
            //find interactable by raycast
            IInteractable newInteractable = null;
            if (Raycast(out RaycastHit hit))
            {
                IInteractable interactable = hit.transform.GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.CanInteract(Owner))
                    newInteractable = interactable;
            }

            //if changed interactable, call events
            if (newInteractable != CurrentInteractable)
            {
                CallEvents(CurrentInteractable, newInteractable);
                CurrentInteractable = newInteractable;
            }
        }

        /// <summary>
        /// Interact with current interactable
        /// </summary>
        public void Interact()
        {
            if (CurrentInteractable != null)
            {
                CurrentInteractable.Interact(Owner);
                onInteract?.Invoke(CurrentInteractable);
            }
            else
            {
                onFailInteract?.Invoke();
            }
        }

        #region private API

        bool Raycast(out RaycastHit hit)
        {
            if (useRaycast)
                return Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistance, interactLayer, raycastHitTriggers);
            else
                return Physics.SphereCast(cam.transform.position, radiusSphereCast, cam.transform.forward, out hit, maxDistance, interactLayer, raycastHitTriggers);
        }

        void CallEvents(IInteractable previousInteractable, IInteractable newInteractable)
        {
            //lost previous interactable
            if (previousInteractable != null)
                onLostInteractable?.Invoke(previousInteractable);

            //found new interactable
            if (newInteractable != null)
                onFoundInteractable?.Invoke(newInteractable);
        }

        #endregion
    }
}