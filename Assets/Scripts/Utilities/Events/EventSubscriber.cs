namespace redd096
{
    using UnityEngine;

    public class EventSubscriber : MonoBehaviour
    {
        [Header("Put scriptable object on Listener and Subscriber")]
        [SerializeField] EventRedd096 eventToSubscribe = default;

        /// <summary>
        /// Invoke event
        /// </summary>
        public void InvokeEvent()
        {
            eventToSubscribe?.Invoke();
        }
    }
}