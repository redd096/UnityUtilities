using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// This is a struct to use when apply damage
    /// </summary>
    public struct FDamageInfoExample
    {
        /// <summary>
        /// Who will be damaged
        /// </summary>
        public IObject DamagedObject;

        /// <summary>
        /// Base damage to apply
        /// </summary>
        public float Damage;

        /// <summary>
        /// Who was responsible for causing this damage (e.g. player who shoot)
        /// </summary>
        public IObject Instigator;

        /// <summary>
        /// Weapon used to do damage
        /// </summary>
        public IWeapon Weapon;

        /// <summary>
        /// The origin the hit came from
        /// </summary>
        public Vector3 HitOrigin;

        /// <summary>
        /// The info of the hit
        /// </summary>
        public RaycastHit HitInfo;

        /// <summary>
        /// This is a struct to use when apply damage
        /// </summary>
        /// <param name="damagedObject">Who will be damaged</param>
        /// <param name="damage">Base damage to apply</param>
        /// <param name="instigator">Who was responsible for causing this damage (e.g. player who shoot)</param>
        /// <param name="weapon">Weapon used to do damage</param>
        /// <param name="hitOrigin">The origin the hit came from</param>
        /// <param name="hitInfo">The info of the hit</param>
        public FDamageInfoExample(IObject damagedObject, float damage, IObject instigator, IWeapon weapon, Vector3 hitOrigin, RaycastHit hitInfo)
        {
            DamagedObject = damagedObject;
            Damage = damage;
            Instigator = instigator;
            Weapon = weapon;
            HitOrigin = hitOrigin;
            HitInfo = hitInfo;
        }
    }
}