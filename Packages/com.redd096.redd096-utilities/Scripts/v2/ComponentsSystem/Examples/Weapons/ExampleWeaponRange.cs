using System.Collections;
using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Attach this script to your weapon prefab, to have a weapon you can use with WeaponsComponent
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example WeaponRange")]
    public class WeaponRange : MonoBehaviour, IWeapon
    {
        public ExampleWeaponRangeData data;
        public ExampleBulletData bulletData;
        public IObject Owner;

        //private
        bool isAttacking;
        float timeForNextShoot;
        Vector3 attackDirection;

        //events
        public System.Action onPickWeapon;
        public System.Action onDropWeapon;
        public System.Action onEquipWeapon;
        public System.Action onUnequipWeapon;
        public System.Action onStartAttack;
        public System.Action onStopAttack;

        public System.Action onEveryBullet;         //called for every bullet shooted (paramter is selected barrel)
        public System.Action onShoot;               //called for every shoot (also if shoot more bullets)
        public System.Action onLastShoot;           //called when shoot last time (when number of bullets is more than one)

        #region public API

        /// <summary>
        /// Set owner
        /// </summary>
        /// <param name="owner"></param>
        public void PickWeapon(IObject owner)
        {
            Owner = owner;
            onPickWeapon?.Invoke();
        }

        /// <summary>
        /// Remove owner (and stop attack if still attacking)
        /// </summary>
        public void DropWeapon()
        {
            if (isAttacking)
                StopAttack();

            Owner = null;
            onDropWeapon?.Invoke();
        }

        /// <summary>
        /// Call when equip weapon
        /// </summary>
        public void EquipWeapon()
        {
            onEquipWeapon?.Invoke();
        }

        /// <summary>
        /// Call when unequip weapon
        /// </summary>
        public void UnequipWeapon()
        {
            if (isAttacking)
                StopAttack();

            onUnequipWeapon?.Invoke();
        }

        /// <summary>
        /// Start attack in direction
        /// </summary>
        /// <param name="attackDirection"></param>
        public void StartAttack(Vector3 attackDirection)
        {
            isAttacking = true;
            this.attackDirection = attackDirection;
            onStartAttack?.Invoke();

            Attack();
        }

        /// <summary>
        /// Update attack in direction
        /// </summary>
        /// <param name="attackDirection"></param>
        public void UpdateAttack(Vector3 attackDirection)
        {
            //continue shooting if Automatic weapon
            if (data.FireMode == ExampleWeaponRangeData.EFireMode.Automatic)
            {
                this.attackDirection = attackDirection;
                Attack();
            }
        }

        /// <summary>
        /// If still attacking, stop attack
        /// </summary>
        public void StopAttack()
        {
            isAttacking = false;
            onStopAttack?.Invoke();
        }

        #endregion

        #region private API

        void Attack()
        {
            //delay between shoots
            if (Time.time < timeForNextShoot)
                return;

            timeForNextShoot = Time.time + data.RateOfFire;

            //shoot
            Shoot();

            //if multi shoots, start coroutine
            if (data.NumberOfShoots > 1)
                StartCoroutine(MultiShootCoroutine());
        }

        void Shoot()
        {
            //shoot every bullet
            for (int i = 0; i < data.NumberOfBullets; i++)
                ShootBullet();

            //recoil
            if (Owner.TryGetObjectComponent(out IPushableExample ownerPushable))
                ownerPushable.Push(-attackDirection * data.Recoil);

            onShoot?.Invoke();
        }

        IEnumerator MultiShootCoroutine()
        {
            //shoot more times (skip first shoot, because we already shooted it)
            for (int i = 1; i < data.NumberOfShoots; i++)
            {
                yield return new WaitForSeconds(data.DelayBetweenShoots);
                Shoot();
            }

            onLastShoot?.Invoke();
        }

        void ShootBullet()
        {
            Quaternion rotation = Quaternion.LookRotation(attackDirection, Vector3.up);

            //calculate accuracy (topdown randomize only on X axis)
            float accuracy = data.NoiseAccuracy;
            Vector3 rotatedAccuracy = rotation * new Vector3(Random.Range(-accuracy, accuracy), 0, 0);
            Vector3 direction = attackDirection + rotatedAccuracy;

            //calculate origin (topdown randomize only on X axis)
            Vector3 attackOrigin = Owner.transform.position + rotation * data.OriginBulletsOffset;
            float offset = data.NoiseOriginBulletOffset;
            Vector3 rotatedOffset = rotation * new Vector3(Random.Range(-offset, offset), 0, 0);
            Vector3 origin = attackOrigin + rotatedOffset;

            Debug.DrawLine(origin, origin + direction * bulletData.MaxDistance, Color.red, 2);

            //shoot
            if (Physics.Raycast(origin, direction, out RaycastHit hit, bulletData.MaxDistance))
            {
                //apply damage and knockback
                IObject hitObject = hit.transform.GetComponentInParent<IObject>();
                if (hitObject != null)
                {
                    if (hitObject.TryGetObjectComponent(out IDamageableExample damageable))
                        damageable.ApplyDamage(bulletData.Damage);

                    if (hitObject.TryGetObjectComponent(out IPushableExample pushable))
                        pushable.Push(direction * bulletData.KnockBack);
                }

                Debug.DrawLine(origin, hit.point, Color.green, 2);
            }

            onEveryBullet?.Invoke();
        }

        #endregion
    }
}