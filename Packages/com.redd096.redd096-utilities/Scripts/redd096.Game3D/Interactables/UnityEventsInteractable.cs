using UnityEngine;
using UnityEngine.Events;

namespace redd096.Game3D
{
    /// <summary>
    /// Just an helper to do fast tests. Call UnityEvents to set in inspector when interact or dismiss
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/Interactables/UnityEvents Interactable")]
    public class UnityEventsInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] float delayBetweenInteracts;
        [SerializeField] bool onInteractReturnTrue = false;
        [SerializeField] bool onDismissReturnFalse = false;

        [SerializeField] UnityEvent<InteractComponent, RaycastHit, object[]> onInteract;
        [SerializeField] UnityEvent<InteractComponent, object[]> onDismiss;

        float nextInteractTime;

        public virtual bool OnInteract(InteractComponent interactor, RaycastHit hit, params object[] args)
        {
            //if interact after delay, call event
            if (Time.time > nextInteractTime)
            {
                nextInteractTime = Time.time + delayBetweenInteracts;
                onInteract?.Invoke(interactor, hit, args);
                return onInteractReturnTrue;
            }

            return false;
        }

        public virtual bool OnDismiss(InteractComponent interactor, params object[] args)
        {
            //call dismiss event
            onDismiss?.Invoke(interactor, args);
            return onDismissReturnFalse == false;
        }
    }
}