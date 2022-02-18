using System.Collections;
using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Pickables/Pick Up BASE")]
    public abstract class PickUpBASE : MonoBehaviour, IPickable
    {
        [Header("Necessary Components - default get from this gameObject")]
        [SerializeField] CollisionComponent collisionComponent = default;

        [Header("Destroy when instantiated - 0 = no destroy")]
        [SerializeField] float timeBeforeDestroy = 0;

        //events
        public System.Action<PickUpBASE> onPick { get; set; }
        public System.Action<PickUpBASE> onFailPick { get; set; }

        protected Character whoHit;
        bool alreadyUsed;

        void OnEnable()
        {
            //reset vars
            alreadyUsed = false;

            //if there is, start auto destruction timer
            if (timeBeforeDestroy > 0)
                StartCoroutine(AutoDestruction());

            //get references
            if (collisionComponent == null) 
                collisionComponent = GetComponent<CollisionComponent>();

            //warnings
            if (collisionComponent == null)
                Debug.LogWarning("Missing CollisionComponent on " + name);

            //add events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter += OnRDCollisionEvent;
                collisionComponent.onTriggerEnter += OnRDCollisionEvent;
            }
        }

        void OnDisable()
        {
            //remove events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter -= OnRDCollisionEvent;
                collisionComponent.onTriggerEnter -= OnRDCollisionEvent;
            }
        }

        void OnRDCollisionEvent(RaycastHit2D collision)
        {
            if (alreadyUsed)
                return;

            //if hitted by player
            whoHit = collision.transform.GetComponentInParent<Character>();
            if (whoHit && whoHit.CharacterType == Character.ECharacterType.Player)
            {
                //pick up
                PickUp();
            }
        }

        IEnumerator AutoDestruction()
        {
            //wait, then destroy
            yield return new WaitForSeconds(timeBeforeDestroy);
            alreadyUsed = true;
            Destroy(gameObject);
        }

        #region protected API

        public abstract void PickUp();

        protected virtual void OnPick()
        {
            //call event
            onPick?.Invoke(this);

            //destroy this gameObject
            alreadyUsed = true;
            Destroy(gameObject);
        }

        protected virtual void OnFailPick()
        {
            //call event
            onFailPick?.Invoke(this);
        }

        #endregion
    }
}