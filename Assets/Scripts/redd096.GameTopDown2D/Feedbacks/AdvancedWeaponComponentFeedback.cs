using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Advanced Weapon Component Feedback")]
    public class AdvancedWeaponComponentFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] AdvancedWeaponComponent advancedWeaponComponent = default;

        [Header("Ammo")]
        [SerializeField] bool updateAmmoOnEquip = true;
        [SerializeField] bool updateAmmoOnShoot = true;

        WeaponRange equippedRangeWeapon;

        void OnEnable()
        {
            //get references
            if (advancedWeaponComponent == null) advancedWeaponComponent = GetComponentInParent<AdvancedWeaponComponent>();

            //add events
            if (advancedWeaponComponent)
            {
                advancedWeaponComponent.onChangeWeapon += OnChangeWeapon;
                advancedWeaponComponent.onAddAmmo += OnAddAmmo;

                //set default weapon and ammo
                OnChangeWeapon();
            }
        }

        void OnDisable()
        {
            //remove events
            if (advancedWeaponComponent)
            {
                advancedWeaponComponent.onChangeWeapon -= OnChangeWeapon;
                advancedWeaponComponent.onAddAmmo -= OnAddAmmo;
            }
        }

        #region private API

        void OnChangeWeapon()
        {
            //save equipped weapon
            equippedRangeWeapon = advancedWeaponComponent.CurrentWeapon as WeaponRange;

            if (updateAmmoOnEquip)
            {
                //update ammo text
                //if (GameManager.instance && GameManager.instance.uiManager)
                //    GameManager.instance.uiManager.SetAmmoText(equippedRangeWeapon ? advancedWeaponComponent.GetCurrentAmmo(equippedRangeWeapon.AmmoType) : 0);
            }
        }

        void OnAddAmmo()
        {
            //if has equipped weapon (this event is called also when pick ammo of other weapons)
            if (equippedRangeWeapon && updateAmmoOnShoot)
            {
                //update ammo text
                //if (GameManager.instance && GameManager.instance.uiManager)
                //    GameManager.instance.uiManager.SetAmmoText(equippedRangeWeapon ? advancedWeaponComponent.GetCurrentAmmo(equippedRangeWeapon.AmmoType) : 0);
            }
        }

        #endregion
    }
}