using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// This data is for a generic ranged weapon: pistol, shotgun, sniper, throwed knife, etc...
    /// </summary>
    [System.Serializable]
    public class ExampleWeaponRangeData
    {
        /// <summary>
        /// SemiAutomatic - click, release and click again to shoot 2 times
        /// Automatic - keep pressed to continue shoot
        /// </summary>
        public enum EFireMode { SemiAutomatic, Automatic }

        [Tooltip("Keep pressed or click?")] public EFireMode FireMode = EFireMode.SemiAutomatic;
        [Tooltip("Delay between shoots")] public float RateOfFire = 0.4f;
        [Tooltip("Push back when shoot")] public float Recoil = 1;
        [Tooltip("Bullet origin is weapon's owner position + this offset")] public Vector3 OriginBulletsOffset = Vector3.zero;

        [Header("Shoots - shoot one time or three shoots in sequence")]
        [Tooltip("Number of shoots at every click")][Min(1)] public int NumberOfShoots = 1;
        [Tooltip("Delay between shoots (if more than one)")] public float DelayBetweenShoots = 0.1f;

        [Header("Bullets - shoot one (pistol) or more bullets (shotgun)")]
        [Tooltip("Shoot one (pistol) or more bullets (shotgun)")][Min(1)] public int NumberOfBullets = 1;
        [Tooltip("Shoot every bullet from origin or with a random offset")] public float NoiseOriginBulletOffset = 0;
        [Tooltip("Rotate random the bullet direction when instantiated")] public float NoiseAccuracy = 0.01f;
    }
}