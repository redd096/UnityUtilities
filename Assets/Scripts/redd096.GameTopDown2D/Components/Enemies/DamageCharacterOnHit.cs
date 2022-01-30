using System.Collections.Generic;
using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Enemies/Damage Character On Hit")]
    public class DamageCharacterOnHit : MonoBehaviour
    {
        [Header("Do Damage On Hit?")]
        [SerializeField] bool isActive = true;

        [Header("Necessary Components - default get from this gameObject")]
        [SerializeField] CollisionComponent collisionComponent = default;

        [Header("Damage")]
        [SerializeField] List<Character.ECharacterType> charactersToHit = new List<Character.ECharacterType>() { Character.ECharacterType.Player };
        [SerializeField] float damage = 10;
        [SerializeField] float pushForce = 10;
        [Tooltip("Necessary for on collision stay, to not call damage every frame")] [SerializeField] float delayBetweenAttacks = 1;

        //events
        public System.Action onHit { get; set; }

        //set if do damage or not
        bool canDoDamage = true;

        Character selfCharacter;
        Dictionary<Character, float> hits = new Dictionary<Character, float>();
        Character hitCharacter;

        void Awake()
        {
            //get references
            selfCharacter = GetComponent<Character>();
        }

        void OnEnable()
        {
            //get references
            if (collisionComponent == null)
                collisionComponent = GetComponent<CollisionComponent>();

            //if collision component has update mode to None, set to Coroutine
            if (isActive && collisionComponent && collisionComponent.UpdateMode == CollisionComponent.EUpdateModes.None)
            {
                collisionComponent.UpdateMode = CollisionComponent.EUpdateModes.Coroutine;
            }

            //add events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter += OnRDCollisionEnter;
                collisionComponent.onCollisionStay += OnRDCollisionStay;
                collisionComponent.onCollisionExit += OnRDCollisionExit;
            }
        }

        void OnDisable()
        {
            //remove events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter -= OnRDCollisionEnter;
                collisionComponent.onCollisionStay -= OnRDCollisionStay;
                collisionComponent.onCollisionExit -= OnRDCollisionExit;
            }
        }

        void OnRDCollisionEnter(RaycastHit2D collision)
        {
            //do nothing if deactivated (use boolean because the unity toggle doesn't work with collision enter)
            if (isActive == false)
                return;

            //check if hit character and is not already in the list
            hitCharacter = collision.transform.GetComponentInParent<Character>();
            if (hitCharacter && hits.ContainsKey(hitCharacter) == false)
            {
                //check can damage
                if ((selfCharacter == null || hitCharacter != selfCharacter) &&     //be sure to not hit self
                    charactersToHit.Contains(hitCharacter.CharacterType))           //and be sure is type can hit
                {
                    //damage it and add to the list
                    OnHit(collision, hitCharacter);
                    hits.Add(hitCharacter, Time.time + delayBetweenAttacks);        //set timer
                }
            }
        }

        void OnRDCollisionStay(RaycastHit2D collision)
        {
            //do nothing if deactivated (use boolean because the unity toggle doesn't work with collision enter)
            if (isActive == false)
                return;

            //check if hit character and is in the list
            hitCharacter = collision.transform.GetComponentInParent<Character>();
            if (hitCharacter && hits.ContainsKey(hitCharacter))
            {
                //damage after delay
                if (Time.time > hits[hitCharacter])
                {
                    OnHit(collision, hitCharacter);
                    hits[hitCharacter] = Time.time + delayBetweenAttacks;           //reset timer
                }
            }
        }

        void OnRDCollisionExit(Collider2D collision)
        {
            //do nothing if deactivated (use boolean because the unity toggle doesn't work with collision enter)
            if (isActive == false)
                return;

            //check if exit hit character and is in the list
            hitCharacter = collision.transform.GetComponentInParent<Character>();
            if (hitCharacter && hits.ContainsKey(hitCharacter))
            {
                //remove from the list
                hits.Remove(hitCharacter);
            }
        }

        void OnHit(RaycastHit2D collision, Character hit)
        {
            //if can't do damage, return, but continue to check if enter or exit collision
            if (canDoDamage == false)
                return;

            //if there is no character, do nothing
            if (hit == null)
            {
                //and remove from the list
                if (hits.ContainsKey(hit))
                    hits.Remove(hit);

                return;
            }

            //call event
            onHit?.Invoke();

            //do damage and push back
            if (hit.GetSavedComponent<HealthComponent>())
                hit.GetSavedComponent<HealthComponent>().GetDamage(damage, selfCharacter, collision.point);

            if (hit && hit.GetSavedComponent<MovementComponent>())
                hit.GetSavedComponent<MovementComponent>().PushInDirection(((Vector2)hit.transform.position - collision.point).normalized, pushForce);
        }

        /// <summary>
        /// Set if can do damage or not
        /// </summary>
        /// <param name="canDoDamage"></param>
        public void SetCanDoDamage(bool canDoDamage)
        {
            this.canDoDamage = canDoDamage;
        }
    }
}