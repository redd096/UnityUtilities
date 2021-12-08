using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Enemies/Damage Character On Hit")]
    public class DamageCharacterOnHit : MonoBehaviour
    {
        [Header("Do Damage On Hit?")]
        [SerializeField] bool isActive = true;

        [Header("Damage")]
        [CanEnable("isActive")] [SerializeField] List<Character.ECharacterType> charactersToHit = new List<Character.ECharacterType>() { Character.ECharacterType.Player };
        [CanEnable("isActive")] [SerializeField] float damage = 10;
        [CanEnable("isActive")] [SerializeField] float pushForce = 10;
        [Tooltip("Necessary for on collision stay, to not call damage every frame")] [CanEnable("isActive")] [SerializeField] float delayBetweenAttacks = 1;

        //events
        public System.Action onHit { get; set; }

        Character character;
        Dictionary<Character, float> hits = new Dictionary<Character, float>();

        void Awake()
        {
            //get references
            character = GetComponent<Character>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (isActive == false)
                return;

            //check if hit character and is not already in the list
            Character hitCharacter = collision.gameObject.GetComponentInParent<Character>();
            if (hitCharacter && hits.ContainsKey(hitCharacter) == false)
            {
                //check can damage
                if ((character == null || hitCharacter != character) &&     //be sure to not hit self
                    charactersToHit.Contains(hitCharacter.CharacterType))   //and be sure is type can hit
                {
                    //damage it and add to the list
                    OnHit(collision, hitCharacter);
                    hits.Add(hitCharacter, Time.time + delayBetweenAttacks);    //set timer
                }
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (isActive == false)
                return;

            //check if hit character and is in the list
            Character hitCharacter = collision.gameObject.GetComponentInParent<Character>();
            if (hitCharacter && hits.ContainsKey(hitCharacter))
            {
                //damage after delay
                if (Time.time > hits[hitCharacter])
                {
                    OnHit(collision, hitCharacter);
                    hits[hitCharacter] = Time.time + delayBetweenAttacks;       //reset timer
                }
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (isActive == false)
                return;

            //check if hit character and is in the list
            Character hitCharacter = collision.gameObject.GetComponentInParent<Character>();
            if (hitCharacter && hits.ContainsKey(hitCharacter))
            {
                //remove from the list
                hits.Remove(hitCharacter);
            }
        }

        void OnHit(Collision2D collision, Character hitCharacter)
        {
            //if there is no character, do nothing
            if(hitCharacter == null)
            {
                //and remove from the list
                if (hits.ContainsKey(hitCharacter))
                    hits.Remove(hitCharacter);

                return;
            }

            //call event
            onHit?.Invoke();

            //do damage and push back
            if (hitCharacter.GetSavedComponent<HealthComponent>())
                hitCharacter.GetSavedComponent<HealthComponent>().GetDamage(damage);

            if (hitCharacter && hitCharacter.GetSavedComponent<MovementComponent>())
                hitCharacter.GetSavedComponent<MovementComponent>().PushInDirection(((Vector2)hitCharacter.transform.position - collision.GetContact(0).point).normalized, pushForce);
        }
    }
}