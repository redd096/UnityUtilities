using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Health Component")]
    public class HealthComponent : MonoBehaviour
    {
        [Header("Health")]
        public bool Invincible = false;
        public float MaxHealth = 100;

        [Header("Friendly Fire")]
        [Range(0f, 1f)] [SerializeField] float percentageDamageWhenHitFromFriend = 0.25f;

        [Header("DEBUG")]
        /*[ProgressBar("Health", "MaxHealth", EColor.Red)]*/ public float CurrentHealth = 100;
        /*[ShowNonSerializedField]*/ [ReadOnly] bool alreadyDead = false;

        //events
        public System.Action onGetDamage { get; set; }
        public System.Action<HealthComponent> onDie { get; set; }
        public System.Action onGetHealth { get; set; }

        Character ownerCharacter;
        Shield shield;

        void OnValidate()
        {
            //set default health
            CurrentHealth = MaxHealth;
        }

        void Awake()
        {
            //get references
            ownerCharacter = GetComponent<Character>();
            shield = GetComponent<Shield>();
        }

        /// <summary>
        /// Get damage and check if dead
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="whoHit"></param>
        /// <param name="hitPoint"></param>
        /// <param name="ignoreShield"></param>
        public void GetDamage(float damage, Character whoHit, Vector2 hitPoint, bool ignoreShield = false)
        {
            if (alreadyDead)
                return;

            //if hitted from same type of character, decrease damage
            if (HittedFromSameCharacterType(whoHit))
            {
                damage *= percentageDamageWhenHitFromFriend;
            }

            //check if hit shield, decrease damage
            if (HittedOnTheShield(hitPoint, ignoreShield))
            {
                damage *= shield.PercentageDamage();
            }

            //set health, only if not invincible
            if (Invincible == false)
                CurrentHealth -= damage;

            //call event only if damage is > 0
            if (damage > Mathf.Epsilon)
                onGetDamage?.Invoke();

            //check if dead
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Call it when health reach 0
        /// </summary>
        public void Die()
        {
            if (alreadyDead)
                return;

            alreadyDead = true;

            //call event
            onDie?.Invoke(this);

            //destroy object
            Destroy(gameObject);
        }

        /// <summary>
        /// Get health and clamp to max health
        /// </summary>
        /// <param name="healthGiven"></param>
        /// <param name="clampMaxHealth">Clamp or can exceed max health?</param>
        public void GetHealth(float healthGiven, bool clampMaxHealth = true)
        {
            if (alreadyDead)
                return;

            //add health
            CurrentHealth += healthGiven;

            //clamp to max health if necessary
            if (clampMaxHealth && CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;

            //call event
            onGetHealth?.Invoke();
        }

        #region private API

        bool HittedFromSameCharacterType(Character whoHit)
        {
            //check if both are character
            if (ownerCharacter && whoHit)
            {
                //check if they are same type of character
                return ownerCharacter.CharacterType == whoHit.CharacterType;
            }

            return false;
        }

        bool HittedOnTheShield(Vector2 hitPoint, bool ignoreShield)
        {
            //return false if this hit ignore shield
            if (ignoreShield)
                return false;

            //else return if we have a shield and hit point is on it
            return shield && shield.CheckHitShield(hitPoint);
        }

        #endregion
    }
}