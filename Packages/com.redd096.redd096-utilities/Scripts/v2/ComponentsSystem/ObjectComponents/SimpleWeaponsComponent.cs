using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This component is used to equip and attack with weapons
    /// </summary>
    [System.Serializable]
    public class SimpleWeaponsComponent : IComponentRD
    {
        [Min(1)][SerializeField] int maxWeapons = 2;

        public IGameObjectRD Owner { get; set; }

        public List<IWeapon> Weapons = new List<IWeapon>();
        public IWeapon CurrentWeapon;

        //events
        public System.Action<IWeapon> onPickWeapon;
        public System.Action<IWeapon> onDropWeapon;
        public System.Action<IWeapon, IWeapon> onChangeCurrentWeapon;

        /// <summary>
        /// Add weapon to the list, and set as current weapon
        /// </summary>
        /// <param name="weapon"></param>
        public void PickWeapon(IWeapon weapon)
        {
            //add to the list
            if (Weapons.Count < maxWeapons)
            {
                Weapons.Add(weapon);
            }
            //or drop current weapon to pick this one
            else if (CurrentWeapon != null)
            {
                int index = GetCurrentWeaponIndex();
                Weapons.Insert(index, weapon);
                DropWeapon(CurrentWeapon, false);
            }

            //call pick event
            weapon.PickWeapon(Owner);
            onPickWeapon?.Invoke(weapon);

            //equip new weapon
            EquipWeapon(weapon);
        }

        /// <summary>
        /// Remove weapon from the list. If this was the CurrentWeapon, equip a new weapon
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="updateEquippedWeapon">If false and drop the CurrentWeapon, the CurrentWeapon isn't updated and onChangeCurrentWeapon isn't invoked</param>
        public void DropWeapon(IWeapon weapon, bool updateEquippedWeapon = true)
        {
            //drop weapon only if in the list
            if (Weapons.Contains(weapon) == false)
            {
                Debug.LogWarning($"Impossible to drop {weapon}, it's not in the list", Owner.transform.gameObject);
                return;
            }

            //save current weapon
            int index = GetCurrentWeaponIndex();
            IWeapon currentWeap = CurrentWeapon;

            //unequip if this was the current weapon
            if (CurrentWeapon == weapon)
            {
                UnequipWeapon();
            }

            //remove from the list
            Weapons.Remove(weapon);

            //call drop event
            weapon.DropWeapon();
            onDropWeapon?.Invoke(weapon);

            //equip new weapon if dropped the current weapon
            if (updateEquippedWeapon && currentWeap == weapon)
            {
                for (int i = 0; i < Weapons.Count; i++)
                {
                    //move to previous weapon, or restart if index < 0
                    index = (index < 0) ? Weapons.Count - 1 : index - 1;
                    if (index > 0)
                    {
                        EquipWeapon(Weapons[index]);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Set current weapon
        /// </summary>
        /// <param name="weapon"></param>
        public void EquipWeapon(IWeapon weapon)
        {
            //equip new weapon
            IWeapon previousWeapon = CurrentWeapon;
            CurrentWeapon = weapon;

            //call equip event
            CurrentWeapon.EquipWeapon();
            onChangeCurrentWeapon?.Invoke(previousWeapon, CurrentWeapon);
        }

        /// <summary>
        /// Set current weapon to null
        /// </summary>
        /// <param name="weapon"></param>
        public void UnequipWeapon()
        {
            //unequip current weapon
            IWeapon previousWeapon = CurrentWeapon;
            CurrentWeapon = null;

            //call unequip event
            previousWeapon.UnequipWeapon();
            onChangeCurrentWeapon?.Invoke(previousWeapon, null);
        }

        /// <summary>
        /// Start attack with CurrentWeapon
        /// </summary>
        public void StartAttack(Vector3 attackDirection)
        {
            if (CurrentWeapon != null)
                CurrentWeapon.StartAttack(attackDirection);
        }

        /// <summary>
        /// Update attack with CurrentWeapon (for example, continue to shoot with an automatic rifle)
        /// </summary>
        public void UpdateAttack(Vector3 attackDirection)
        {
            if (CurrentWeapon != null)
                CurrentWeapon.UpdateAttack(attackDirection);
        }

        /// <summary>
        /// Stop attack with CurrentWeapon
        /// </summary>
        public void StopAttack()
        {
            if (CurrentWeapon != null)
                CurrentWeapon.StopAttack();
        }

        #region private API

        private int GetCurrentWeaponIndex()
        {
            return Weapons.IndexOf(CurrentWeapon);
        }

        #endregion
    }
}