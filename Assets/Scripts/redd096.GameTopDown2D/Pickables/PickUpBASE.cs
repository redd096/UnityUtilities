using System.Collections;
using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Pickables/Pick Up BASE")]
    public abstract class PickUpBASE<T> : MonoBehaviour, IPickable where T : Component
    {
        [Header("Destroy when instantiated - 0 = no destroy")]
        [SerializeField] float timeBeforeDestroy = 0;

        //events
        public System.Action<PickUpBASE<T>> onPick { get; set; }
        public System.Action<PickUpBASE<T>> onFailPick { get; set; }

        protected Character whoHit;
        protected T whoHitComponent;
        bool alreadyUsed;

        protected virtual void OnEnable()
        {
            //reset vars
            whoHit = null;
            whoHitComponent = null;
            alreadyUsed = false;

            //if there is, start auto destruction timer
            if (timeBeforeDestroy > 0)
                StartCoroutine(AutoDestruction());
        }

        protected virtual void FixedUpdate()
        {
            if (alreadyUsed)
                return;

            //if trigger enter and can't pick up, check again. Maybe player stay compenetrate and loose health, so now can pick up
            if (whoHit && CanPickUp())
            {
                PickUp();
                OnPick();
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (alreadyUsed)
                return;

            //if hitted by player
            Character ch = collision.transform.GetComponentInParent<Character>();
            if (ch && ch.CharacterType == Character.ECharacterType.Player)
            {
                whoHit = ch;
                whoHitComponent = whoHit.GetSavedComponent<T>();

                //pick up
                if (CanPickUp())
                {
                    PickUp();
                    OnPick();
                }
                //or fail pick up
                else
                {
                    OnFailPick();
                }
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            //remove who hit on trigger exit
            if (whoHit && collision.transform.GetComponentInParent<Character>() == whoHit)
            {
                whoHit = null;
                whoHitComponent = null;
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

        protected abstract bool CanPickUp();

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