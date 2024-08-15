using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// This data is for every bullet of a range weapon
    /// </summary>
    [System.Serializable]
    public class ExampleBulletData
    {
        [Tooltip("Damage for every bullet")] public float Damage = 10;
        [Tooltip("Push back who hit")] public float KnockBack = 1;
        [Tooltip("Can hit only in this distance")] public float MaxDistance = 30;

        //[Header("Layers")]
        //[Tooltip("Layers to hit but not destroy bullet")] public LayerMask LayersPenetrable = default;
        //[Tooltip("Layers to not hit")] public LayerMask LayersToIgnore = default;
        //[Tooltip("Layers where bullet bounce")] public LayerMask LayersToBounce = default;
        //[Tooltip("Number of bounces before destroy bullet")] public int MaxNumberOfBounce = 2;

        //[Header("Area Damage")]
        //public bool DoAreaDamage = false;
        //[Tooltip("Damage others in radius area")][EnableIf("doAreaDamage")][Min(0)] public float RadiusAreaDamage = 0;
        //[Tooltip("Damage who is in area")][EnableIf("doAreaDamage")] public float DamageInArea = 10;
        //[Tooltip("Do knockback to who hit in area?")][EnableIf("doAreaDamage")] public float KnockBackInArea = 1;
        //[Tooltip("Is possible to damage and knockback owner with area damage?")][EnableIf("doAreaDamage")] public bool AreaCanHitWhoShoot = false;
        //[Tooltip("Is possible to damage and knockback again who hit this bullet?")][EnableIf("doAreaDamage")] public bool AreaCanHitWhoBulletHit = false;

        //[Header("Distance and AutoDestruction")]
        //[Tooltip("Can hit only in this distance")] public float MaxDistance = 100;
        //[Tooltip("If don't hit anything, auto destroy when reach max distance")][EnableIf("doAreaDamage")] public bool DoAreaDamangeOnMaxDistance = true;
    }
}