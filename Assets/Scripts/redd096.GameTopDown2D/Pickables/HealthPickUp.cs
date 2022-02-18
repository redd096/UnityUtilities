using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Pickables/Health Pick Up")]
    public class HealthPickUp : PickUpBASE
    {
        [Header("Health")]
        [SerializeField] float healthToGive = 30;
        [Tooltip("Can pick when full of health? If true, this object will be destroyed, but no health will be added")] [SerializeField] bool canPickAlsoIfFull = false;

        HealthComponent whoHitComponent;

        public override void PickUp()
        {
            //check if hit has component
            whoHitComponent = whoHit.GetSavedComponent<HealthComponent>();
            if (whoHitComponent)
            {
                //if can pick when full, or current health is lower than max health
                if (canPickAlsoIfFull || whoHitComponent.CurrentHealth < whoHitComponent.MaxHealth)
                {
                    //give health
                    whoHitComponent.GetHealth(healthToGive);
                    OnPick();
                }
                //else fail pick
                else
                {
                    OnFailPick();
                }
            }
        }
    }
}