using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Find interactables in a radius around character. And call function to interact
    /// </summary>
    [System.Serializable]
    public class InteractComponentRadius : IComponentRD
    {
        [Tooltip("Use OverlapSphere to find interactables (3d) or OverlapCircle (2d)")][SerializeField] bool findInteractablesIn3D = false;
        [Tooltip("Area to check for interactables")][SerializeField] float radiusInteract = 1f;
        [Tooltip("Hit only interacts with this layer")][SerializeField] LayerMask interactLayer = -1;
        [Tooltip("This is used if you call ScanInteractablesInDirection")][SerializeField] float maxAngle = 45f;
        [SerializeField] ShowDebugRedd096 showRadiusInteract = Color.cyan;

        public IGameObjectRD Owner { get; set; }

        //events
        public System.Action<ISimpleInteractable> onFoundInteractable;
        public System.Action<ISimpleInteractable> onLostInteractable;
        public System.Action<ISimpleInteractable> onInteract;           //when user interact with Interactable
        public System.Action onFailInteract;                            //when user try to interact but Interactable is null or CanInteract return false

        public ISimpleInteractable CurrentInteractable;

        public void OnDrawGizmosSelectedRD()
        {
            //draw area interactable
            if (showRadiusInteract)
            {
                Gizmos.color = showRadiusInteract.ColorDebug;
                Gizmos.DrawWireSphere(Owner.transform.position, radiusInteract);

                //draw max angle
                Gizmos.color = Color.red;
                Vector3 dir = (findInteractablesIn3D ? Owner.transform.forward : Owner.transform.right) * radiusInteract;
                Vector3 axis = findInteractablesIn3D ? Vector3.up : Vector3.forward;
                Gizmos.DrawLine(Owner.transform.position, Owner.transform.position + Quaternion.AngleAxis(maxAngle, axis) * dir);
                Gizmos.DrawLine(Owner.transform.position, Owner.transform.position + Quaternion.AngleAxis(-maxAngle, axis) * dir);

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
            ISimpleInteractable newInteractable = FindNearest(possibleInteractables);

            //if changed interactable, call events
            CheckChangeInteractable(newInteractable);
        }

        /// <summary>
        /// Find interactables in radius, but only if inside max angle
        /// </summary>
        /// <param name="direction"></param>
        public void ScanInteractablesInDirection(Vector3 direction)
        {
            var possibleInteractables = findInteractablesIn3D ? GetPossibleInteractables3D() : GetPossibleInteractables2D();

            //remove interactables not in angle
            direction = direction.normalized;
            foreach (Transform collider in new List<Transform>(possibleInteractables.Keys))
            {
                Vector3 dir = (collider.position - Owner.transform.position).normalized;
                if (Vector3.Angle(dir, direction) > maxAngle)
                    possibleInteractables.Remove(collider);
            }

            //find nearest interactable
            ISimpleInteractable newInteractable = FindNearest(possibleInteractables);

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

        Dictionary<Transform, ISimpleInteractable> GetPossibleInteractables3D()
        {
            //find interactables in area
            Dictionary<Transform, ISimpleInteractable> possibleInteractables = new Dictionary<Transform, ISimpleInteractable>();
            foreach (Collider col in Physics.OverlapSphere(Owner.transform.position, radiusInteract, interactLayer))
            {
                //add to dictionary if CanInteract is true
                ISimpleInteractable interactable = col.GetComponentInParent<ISimpleInteractable>();
                if (interactable != null && interactable.CanInteract(Owner))
                {
                    possibleInteractables.Add(col.transform, interactable);
                }
            }
            return possibleInteractables;
        }

        Dictionary<Transform, ISimpleInteractable> GetPossibleInteractables2D()
        {
            //find interactables in area
            Dictionary<Transform, ISimpleInteractable> possibleInteractables = new Dictionary<Transform, ISimpleInteractable>();
            foreach (Collider2D col in Physics2D.OverlapCircleAll(Owner.transform.position, radiusInteract, interactLayer))
            {
                //add to dictionary if CanInteract is true
                ISimpleInteractable interactable = col.GetComponentInParent<ISimpleInteractable>();
                if (interactable != null && interactable.CanInteract(Owner))
                {
                    possibleInteractables.Add(col.transform, interactable);
                }
            }
            return possibleInteractables;
        }

        ISimpleInteractable FindNearest(Dictionary<Transform, ISimpleInteractable> possibleInteractables)
        {
            ISimpleInteractable nearest = null;
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

            //return interactable
            return nearest;
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