using System.Collections.Generic;
using UnityEngine;

namespace redd096.ComponentsSystem
{
    /// <summary>
    /// Find interactables in a radius around character. And call function to interact
    /// </summary>
    [System.Serializable]
    public class InteractComponent2D : ICharacterComponent
    {
        [Tooltip("Look for interactables every X seconds")][SerializeField] float updateTime = 0.2f;
        [Tooltip("Area to check for interactables")][SerializeField] float radiusInteract = 1f;
        [Tooltip("Hit only interacts with this layer")][SerializeField] LayerMask interactLayer = -1;
        [SerializeField] ShowDebugRedd096 showRadiusInteract = Color.cyan;

        public ICharacter Owner { get; set; }

        //events
        public System.Action<IInteractable> onFoundInteractable;
        public System.Action<IInteractable> onLostInteractable;

        private float time;
        private IInteractable currentInteractable;

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

        public void Update()
        {
            //delay between updates
            if (Time.time < time)
                return;
            time = Time.time + updateTime;

            //find nearest interactable
            var possibleInteractables = GetPossibleInteractables();
            IInteractable newInteractable = FindNearest(possibleInteractables);

            //if changed interactable, call events
            if (newInteractable != currentInteractable)
            {
                CallEvents(currentInteractable, newInteractable);
                currentInteractable = newInteractable;
            }
        }

        /// <summary>
        /// Interact with nearest interactable
        /// </summary>
        public void Interact()
        {
            if (currentInteractable != null)
                currentInteractable.Interact(Owner);
        }

        #region private API

        Dictionary<Collider2D, IInteractable> GetPossibleInteractables()
        {
            //find interactables in area
            Dictionary<Collider2D, IInteractable> possibleInteractables = new Dictionary<Collider2D, IInteractable>();
            foreach (Collider2D col in Physics2D.OverlapCircleAll(Owner.transform.position, radiusInteract, interactLayer))
            {
                IInteractable interactable = col.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    //add to dictionary
                    possibleInteractables.Add(col, interactable);
                }
            }
            return possibleInteractables;
        }

        IInteractable FindNearest(Dictionary<Collider2D, IInteractable> possibleInteractables)
        {
            IInteractable nearest = null;
            float distance = Mathf.Infinity;

            //find nearest collider
            foreach (Collider2D col in possibleInteractables.Keys)
            {
                float newDistance = Vector2.Distance(col.transform.position, Owner.transform.position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    nearest = possibleInteractables[col];
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