using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Weapons/Weapon BASE")]
    public abstract class WeaponBASE : MonoBehaviour
    {
        [Header("Weapon BASE")]
        public string WeaponName = "Weapon Name";
        public int WeaponPrice = 10;
        /*[ShowAssetPreview]*/ public Sprite WeaponSprite = default;

        [Header("DEBUG")]
        [SerializeField] bool destroyWeaponOnDrop = true;
        [ReadOnly] public Character Owner;

        //events
        public System.Action onPickWeapon { get; set; }
        public System.Action onDropWeapon { get; set; }

        #region public API

        /// <summary>
        /// Set owner to look at
        /// </summary>
        /// <param name="owner"></param>
        public void PickWeapon(Character owner)
        {
            Owner = owner;

            //if is a range weapon and is picked from Player, update ammo UI
            //if (this is WeaponRange weaponRange)
            //    if (Owner && Owner.CharacterType == Character.ECharacterType.Player)
            //        GameManager.instance.uiManager.SetAmmoText(weaponRange.currentAmmo);

            //call event
            onPickWeapon?.Invoke();
        }

        /// <summary>
        /// Remove owner
        /// </summary>
        public void DropWeapon()
        {
            Owner = null;

            //be sure to reset attack vars
            ReleaseAttack();

            //call event
            onDropWeapon?.Invoke();

            //destroy weapon, if setted
            if (destroyWeaponOnDrop)
                Destroy(gameObject);
        }

        #endregion

        #region abstracts

        public abstract void PressAttack();
        public abstract void ReleaseAttack();

        #endregion
    }
}