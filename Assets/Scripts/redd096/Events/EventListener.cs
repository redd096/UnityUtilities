namespace redd096
{
    using UnityEngine;
    using UnityEngine.Events;

    public class EventListener : MonoBehaviour
    {
        [Header("Collisions")]
        [SerializeField] bool onCollisionEnter = false;
        [SerializeField] bool onTriggerEnter = false;

        [Header("Event to set")]
        [SerializeField] UnityEvent response = new UnityEvent();

        /// <summary>
        /// Call this function to invoke event
        /// </summary>
        public void InvokeEvent()
        {
            response.Invoke();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (onCollisionEnter)
            {
                InvokeEvent();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if(onTriggerEnter)
            {
                InvokeEvent();
            }
        }
    }
}