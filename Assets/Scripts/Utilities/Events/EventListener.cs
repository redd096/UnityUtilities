namespace redd096
{
    using UnityEngine;
    using UnityEngine.Events;

    public class EventListener : MonoBehaviour
    {
        [Header("Put scriptable object on Listener and Subscriber")]
        [SerializeField] EventRedd096 eventToSubscribe = default;

        [Header("Event to set")]
        [SerializeField] UnityEvent response = new UnityEvent();

        void OnEnable()
        {
            eventToSubscribe.Add(this);
        }

        void OnDisable()
        {
            eventToSubscribe.Remove(this);
        }

        /// <summary>
        /// When event occurs, this function is called
        /// </summary>
        public void OnEventOccurs()
        {
            response.Invoke();
        }

    }
}