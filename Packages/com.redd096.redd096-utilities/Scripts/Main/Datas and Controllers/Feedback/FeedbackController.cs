using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Used to manage Feedbacks
    /// </summary>
    public abstract class FeedbackController : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            AddEvents();
        }

        protected virtual void OnDisable()
        {
            RemoveEvents();
        }

        protected virtual void AddEvents() { }
        protected virtual void RemoveEvents() { }

        /// <summary>
        /// Instantiate feedbacks using parent or self
        /// </summary>
        protected void InstantiateFeedback(FeedbackClass feedback, Transform parent = null, bool worldPositionStays = true)
        {
            feedback.Element.InstantiateFeedback(parent ? parent : transform, worldPositionStays);
        }

        /// <summary>
        /// Instantiate feedbacks using parent or self. And return first instantiated object for every fx
        /// </summary>
        protected void InstantiateFeedback(FeedbackClass feedback, out GameObject outInstantiatedGameObject, out ParticleSystem outInstantiatedParticle, out AudioSource outInstantiatedAudio, Transform parent = null, bool worldPositionStays = true)
        {
            feedback.Element.InstantiateFeedback(out outInstantiatedGameObject, out outInstantiatedParticle, out outInstantiatedAudio, parent ? parent : transform, worldPositionStays);
        }

        /// <summary>
        /// Instantiate feedbacks using parent or self. And return every instantiated fx
        /// </summary>
        protected void InstantiateFeedback(FeedbackClass feedback, out GameObject[] outInstantiatedGameObjects, out ParticleSystem[] outInstantiatedParticles, out AudioSource[] outInstantiatedAudios, Transform parent = null, bool worldPositionStays = true)
        {
            feedback.Element.InstantiateFeedback(out outInstantiatedGameObjects, out outInstantiatedParticles, out outInstantiatedAudios, parent ? parent : transform, worldPositionStays);
        }
    }

    public abstract class FeedbackController<T> : FeedbackController
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] protected T owner;

        protected override void OnEnable()
        {
            //get owner
            if (owner == null)
                owner = GetComponentInParent<T>();

            //add events
            if (owner != null)
                AddEvents();
        }

        protected override void OnDisable()
        {
            //remove events
            if (owner != null)
                RemoveEvents();
        }
    }
}