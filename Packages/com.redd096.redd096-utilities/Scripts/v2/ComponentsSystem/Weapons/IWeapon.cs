using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This is the interface for every weapon. Use a WeaponsComponent to manage it
    /// </summary>
    public interface IWeapon
    {
        void PickWeapon(IGameObjectRD owner);
        void DropWeapon();

        void EquipWeapon();
        void UnequipWeapon();

        void StartAttack(Vector3 attackDirection);
        void UpdateAttack(Vector3 attackDirection);
        void StopAttack();
    }
}