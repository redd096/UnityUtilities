using UnityEngine;

namespace redd096.GameTopDown2D
{
    [RequireComponent(typeof(CollisionComponent))]
    public abstract class MonoBehaviourRD : MonoBehaviour
    {
        CollisionComponent collisionComponent;

        void Awake()
        {
            //get references
            if (collisionComponent == null) 
                collisionComponent = GetComponent<CollisionComponent>();

            //warnings
            if (collisionComponent == null) 
                Debug.LogWarning("Miss CollisionComponent on " + name);
        }

        void OnEnable()
        {
            //get references
            if (collisionComponent == null) collisionComponent = GetComponent<CollisionComponent>();

            //add events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter += OnCollisionEnterRD;
                collisionComponent.onCollisionStay += OnCollisionStayRD;
                collisionComponent.onCollisionExit += OnCollisionExitRD;

                collisionComponent.onTriggerEnter += OnTriggerEnterRD;
                collisionComponent.onTriggerStay += OnTriggerStayRD;
                collisionComponent.onTriggerExit += OnTriggerExitRD;
            }
        }

        void OnDisable()
        {
            //remove events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter -= OnCollisionEnterRD;
                collisionComponent.onCollisionStay -= OnCollisionStayRD;
                collisionComponent.onCollisionExit -= OnCollisionExitRD;

                collisionComponent.onTriggerEnter -= OnTriggerEnterRD;
                collisionComponent.onTriggerStay -= OnTriggerStayRD;
                collisionComponent.onTriggerExit -= OnTriggerExitRD;
            }
        }

        protected virtual void OnCollisionEnterRD(RaycastHit2D collision)
        {

        }

        protected virtual void OnCollisionStayRD(RaycastHit2D collision)
        {

        }

        protected virtual void OnCollisionExitRD(Collider2D collision)
        {

        }

        protected virtual void OnTriggerEnterRD(RaycastHit2D collision)
        {

        }

        protected virtual void OnTriggerStayRD(RaycastHit2D collision)
        {

        }

        protected virtual void OnTriggerExitRD(Collider2D collision)
        {

        }
    }
}