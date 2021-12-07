using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Change Order Weapon On Rotate Feedback")]
    public class ChangeOrderWeaponOnRotateFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] WeaponBASE weaponBASE;

        [Header("Sprites - default get in children")]
        [SerializeField] SpriteRenderer[] spritesToUse = default;
        [SerializeField] int layerWhenLookingRight = 1;
        [SerializeField] int layerWhenLookingLeft = -1;

        AimComponent owner;

        void OnEnable()
        {
            //get referemces
            if(weaponBASE == null) weaponBASE = GetComponentInParent<WeaponBASE>();
            if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();

            //add events
            if (weaponBASE)
            {
                weaponBASE.onPickWeapon += OnPickWeapon;
                weaponBASE.onDropWeapon += OnDropWeapon;

                //if there is an owner register to its events too
                OnPickWeapon();
            }
        }

        void OnDisable()
        {
            //remove events
            if (weaponBASE)
            {
                weaponBASE.onPickWeapon -= OnPickWeapon;
                weaponBASE.onDropWeapon -= OnDropWeapon;
            }

            //remove owner events too
            OnDropWeapon();
        }

        void UpdateOrderInLayer(bool isLookingRight)
        {
            //foreach sprite, update order
            foreach (SpriteRenderer sprite in spritesToUse)
            {
                sprite.sortingOrder = isLookingRight ? layerWhenLookingRight : layerWhenLookingLeft;
            }
        }

        #region events

        void OnPickWeapon()
        {
            //register to owner events
            if (weaponBASE.Owner && weaponBASE.Owner.GetSavedComponent<AimComponent>())
            {
                owner = weaponBASE.Owner.GetSavedComponent<AimComponent>();
                owner.onChangeAimDirection += OnChangeAimDirection;

                //update order
                UpdateOrderInLayer(owner.IsLookingRight);
            }
        }

        void OnDropWeapon()
        {
            //remove events from owner
            if (owner)
            {
                owner.onChangeAimDirection -= OnChangeAimDirection;
            }
        }

        void OnChangeAimDirection(bool isLookingRight)
        {
            //update order in layer
            UpdateOrderInLayer(isLookingRight);
        }

        #endregion
    }
}