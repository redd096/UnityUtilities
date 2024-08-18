using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Find interactables in a radius around character. And call function to interact
    /// </summary>
    [System.Serializable]
    public class InteractComponentRadius : IObjectComponent
    {
        [Tooltip("Use OverlapSphere to find interactables (3d) or OverlapCircle (2d)")][SerializeField] bool findInteractablesIn3D = false;
        [Tooltip("Area to check for interactables")][SerializeField] float radiusInteract = 1f;
        [Tooltip("Hit only interacts with this layer")][SerializeField] LayerMask interactLayer = -1;
        [SerializeField] ShowDebugRedd096 showRadiusInteract = Color.cyan;

        public IObject Owner { get; set; }

        //events
        public System.Action<IInteractable> onFoundInteractable;
        public System.Action<IInteractable> onLostInteractable;
        public System.Action<IInteractable> onInteract;             //when user interact with CurrentInteractable
        public System.Action onFailInteract;                        //when user try to interact but CurrentInteractable is null

        public IInteractable CurrentInteractable;

        public void OnDrawGizmosSelected()
        {
            //draw area interactable
            if (showRadiusInteract)
            {
                Gizmos.color = showRadiusInteract.ColorDebug;
                Gizmos.DrawWireSphere(Owner.transform.position, radiusInteract);
                Gizmos.color = Color.white;
            }
        }

        /// <summary>
        /// Find interactables in radius and set Current Interactable
        /// </summary>
        public void ScanInteractables()
        {
            //find nearest interactable
            var possibleInteractables = findInteractablesIn3D ? GetPossibleInteractables3D() : GetPossibleInteractables2D();
            IInteractable newInteractable = FindNearest(possibleInteractables);

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

        Dictionary<Transform, IInteractable> GetPossibleInteractables3D()
        {
            //find interactables in area
            Dictionary<Transform, IInteractable> possibleInteractables = new Dictionary<Transform, IInteractable>();
            foreach (Collider col in Physics.OverlapSphere(Owner.transform.position, radiusInteract, interactLayer))
            {
                IInteractable interactable = col.GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.CanInteract(Owner))
                {
                    //add to dictionary
                    possibleInteractables.Add(col.transform, interactable);
                }
            }
            return possibleInteractables;
        }

        Dictionary<Transform, IInteractable> GetPossibleInteractables2D()
        {
            //find interactables in area
            Dictionary<Transform, IInteractable> possibleInteractables = new Dictionary<Transform, IInteractable>();
            foreach (Collider2D col in Physics2D.OverlapCircleAll(Owner.transform.position, radiusInteract, interactLayer))
            {
                IInteractable interactable = col.GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.CanInteract(Owner))
                {
                    //add to dictionary
                    possibleInteractables.Add(col.transform, interactable);
                }
            }
            return possibleInteractables;
        }

        IInteractable FindNearest(Dictionary<Transform, IInteractable> possibleInteractables)
        {
            IInteractable nearest = null;
            float distance = Mathf.Infinity;

            //find nearest interactable
            foreach (Transform t in possibleInteractables.Keys)
            {
                float newDistance = (t.position - Owner.transform.position).sqrMagnitude;
                if (newDistance < distance)
                {
                    distance = newDistance;
                    nearest = possibleInteractables[t];
                }
            }

            //return its interactable
            return nearest;
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