using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Get Hit Animation Feedback")]
    public class GetHitAnimmationFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in child and parent")]
        [SerializeField] Animator anim = default;
        [SerializeField] HealthComponent healthComponent = default;

        [Header("Animator triggers' name")]
        [SerializeField] string hitTrigger = "Damage";
        [SerializeField] string deathTrigger = "Dead";

        [Header("Checks")]
        [SerializeField] bool setTriggerOnGetDamage = true;
        [SerializeField] bool setTriggerOnDie = true;

        void OnEnable()
        {
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (healthComponent == null) healthComponent = GetComponentInParent<HealthComponent>();

            if (healthComponent)
            {
                healthComponent.onGetDamage += OnGetDamage;
                healthComponent.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            if (healthComponent)
            {
                healthComponent.onGetDamage -= OnGetDamage;
                healthComponent.onDie -= OnDie;
            }
        }

        void OnGetDamage(Vector2 hitPoint)
        {
            //set trigger on get damage
            if (anim && setTriggerOnGetDamage)
                anim.SetTrigger(hitTrigger);
        }

        void OnDie(HealthComponent healthComponent)
        {
            //set trigger on die
            if (anim && setTriggerOnDie)
                anim.SetTrigger(deathTrigger);
        }
    }
}