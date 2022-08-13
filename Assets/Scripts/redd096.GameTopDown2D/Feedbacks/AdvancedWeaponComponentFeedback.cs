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

        [Header("Update UI")]
        [SerializeField] bool updateBulletImage = true;

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

            if (updateBulletImage)
            {
                //update bullet image
                //if (GameManager.instance && GameManager.instance.uiManager)
                //{
                //    //if range weapon, with ammo sprite setted then use it, else set sprite empty
                //    GameManager.instance.uiManager.SetBulletImage(equippedRangeWeapon && equippedRangeWeapon.AmmoSprite ? equippedRangeWeapon.AmmoSprite : null);
                //}
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