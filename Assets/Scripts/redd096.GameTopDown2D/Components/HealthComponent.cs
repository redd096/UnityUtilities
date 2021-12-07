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

        [Header("DEBUG")]
        /*[ProgressBar("Health", "MaxHealth", EColor.Red)]*/ public float CurrentHealth = 100;
        /*[ShowNonSerializedField]*/ [ReadOnly] bool alreadyDead = false;

        //events
        public System.Action onGetDamage { get; set; }
        public System.Action<Redd096Main> onDie { get; set; }
        public System.Action onGetHealth { get; set; }

        Redd096Main owner;

        void OnValidate()
        {
            //set default health
            CurrentHealth = MaxHealth;
        }

        void Awake()
        {
            //get references
            owner = GetComponent<Redd096Main>();
        }

        /// <summary>
        /// Get damage and check if dead
        /// </summary>
        /// <param name="damage"></param>
        public void GetDamage(float damage)
        {
            if (alreadyDead)
                return;

            //set health, only if not invincible
            if (Invincible == false)
                CurrentHealth -= damage;

            //call event
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
            onDie?.Invoke(owner);

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
    }
}