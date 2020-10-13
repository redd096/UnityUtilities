namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Event", menuName = "redd096/Event redd096")]
    public class EventRedd096 : ScriptableObject
    {
        List<EventListener> eventListeners = new List<EventListener>();

        /// <summary>
        /// Add listener to this event
        /// </summary>
        public void Add(EventListener listener)
        {
            eventListeners.Add(listener);
        }

        /// <summary>
        /// Remove listener from this event
        /// </summary>
        public void Remove(EventListener listener)
        {
            eventListeners.Remove(listener);
        }

        /// <summary>
        /// Invoke event
        /// </summary>
        public void Invoke()
        {
            //invoke on every listener
            for (int i = 0; i < eventListeners.Count; i++)
            {
                eventListeners[i].OnEventOccurs();
            }
        }

    }
}