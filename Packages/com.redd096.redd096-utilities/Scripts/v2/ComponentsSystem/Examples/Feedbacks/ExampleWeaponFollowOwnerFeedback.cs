using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Use this component to move weapon with its owner when equipped
    /// </summary>
    [System.Serializable]
    public class ExampleWeaponFollowOwnerFeedback : IComponentRD
    {
        [SerializeField] Vector3 offsetFromOwner;

        public IGameObjectRD Owner { get; set; }

        ExampleWeaponRange weapon;

        public void AwakeRD()
        {
            //add events
            weapon = Owner.transform.GetComponent<ExampleWeaponRange>();
            if (weapon)
            {
                weapon.onEquipWeapon += OnEquipWeapon;
                weapon.onUnequipWeapon += OnUnequipWeapon;
            }
            else
            {
                Debug.LogError($"Missing weapon on {GetType().Name}", Owner.transform.gameObject);
            }
        }

        public void OnDestroyRD()
        {
            //remove events
            if (weapon)
            {
                weapon.onEquipWeapon -= OnEquipWeapon;
                weapon.onUnequipWeapon -= OnUnequipWeapon;
            }
        }

        void OnEquipWeapon()
        {
            //set parent of weapon's Owner
            weapon.transform.SetParent(weapon.Owner.transform);
            weapon.transform.localPosition = offsetFromOwner;
            weapon.transform.localRotation = Quaternion.identity;
        }

        void OnUnequipWeapon()
        {
            //remove parent
            weapon.transform.SetParent(null);
        }
    }
}