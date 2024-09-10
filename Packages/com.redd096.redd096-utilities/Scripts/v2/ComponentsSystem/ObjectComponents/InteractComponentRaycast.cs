using redd096.Attributes;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Find interactables in front of owner by using raycast, and call function to interact
    /// </summary>
    [System.Serializable]
    public class InteractComponentRaycast : IComponentRD
    {
        [Header("Necessary Components (by default use main camera and get from this gameObject)")]
        [SerializeField] Camera cam;
        [SerializeField] float maxDistance = 3;
        [Tooltip("Hit only interacts with this layer")][SerializeField] LayerMask interactLayer = -1;
        [Tooltip("Use raycast or sphere cast")][SerializeField] bool useRaycast = false;
        [DisableIf("useRaycast")][SerializeField] float radiusSphereCast = 0.02f;
        [SerializeField] QueryTriggerInteraction raycastHitTriggers = QueryTriggerInteraction.UseGlobal;
        [SerializeField] ShowDebugRedd096 gizmosRaycast = Color.cyan;

        public IGameObjectRD Owner { get; set; }

        //events
        public System.Action<ISimpleInteractable> onFoundInteractable;
        public System.Action<ISimpleInteractable> onLostInteractable;
        public System.Action<ISimpleInteractable> onInteract;           //when user interact with Interactable
        public System.Action onFailInteract;                            //when user try to interact but Interactable is null or CanInteract return

        public ISimpleInteractable CurrentInteractable;

        public void OnDrawGizmosSelectedRD()
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

        public void AwakeRD()
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
            ISimpleInteractable newInteractable = null;
            if (Raycast(out RaycastHit hit))
            {
                ISimpleInteractable interactable = hit.transform.GetComponentInParent<ISimpleInteractable>();
                if (interactable != null && interactable.CanInteract(Owner))
                    newInteractable = interactable;
            }

            //if changed interactable, call events
            CheckChangeInteractable(newInteractable);
        }

        /// <summary>
        /// Try interact with current interactable
        /// </summary>
        public void TryInteract()
        {
            if (CurrentInteractable != null && CurrentInteractable.CanInteract(Owner))
            {
                CurrentInteractable.Interact(Owner);
                onInteract?.Invoke(CurrentInteractable);
            }
            else
            {
                onFailInteract?.Invoke();
            }
        }

        /// <summary>
        /// Set this as current interactable and try to interact with it
        /// </summary>
        /// <param name="interactable"></param>
        public void TryInteract(ISimpleInteractable interactable)
        {
            //if changed interactable, call events
            CheckChangeInteractable(interactable);

            //interact
            TryInteract();
        }

        #region private API

        bool Raycast(out RaycastHit hit)
        {
            if (useRaycast)
                return Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistance, interactLayer, raycastHitTriggers);
            else
                return Physics.SphereCast(cam.transform.position, radiusSphereCast, cam.transform.forward, out hit, maxDistance, interactLayer, raycastHitTriggers);
        }

        void CheckChangeInteractable(ISimpleInteractable newInteractable)
        {
            //if changed interactable, call events
            if (newInteractable != CurrentInteractable)
            {
                if (CurrentInteractable != null)
                    onLostInteractable?.Invoke(CurrentInteractable);

                if (newInteractable != null)
                    onFoundInteractable?.Invoke(newInteractable);

                //and set current interactable
                CurrentInteractable = newInteractable;
            }
        }

        #endregion
    }
}