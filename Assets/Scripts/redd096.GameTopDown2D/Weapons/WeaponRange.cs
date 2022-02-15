using System.Collections;
using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Weapons/Weapon Range")]
    public class WeaponRange : WeaponBASE
    {
        [Header("Range Weapon")]
        [Tooltip("Keep pressed or click?")] public bool Automatic = true;
        [Tooltip("Delay between shots")] public float RateOfFire = 0.2f;
        [Tooltip("Push back when shoot")] public float Recoil = 1;
        [Tooltip("Rotate random the shot when instantiated")] public float NoiseAccuracy = 10;

        [Header("Barrel")]
        [Tooltip("Spawn bullets")] public Transform[] Barrels = default;
        [Tooltip("When more than one barrel, shoot every bullet simultaneously or from one random?")] public bool BarrelSimultaneously = true;

        [Header("Bullet")]
        public Bullet BulletPrefab = default;
        public float Damage = 10;
        public float BulletSpeed = 10;

        [Header("Ammo - NONE = always full")]
        /*[Dropdown("GetAllAmmoTypes")]*/ public string AmmoType = "NONE";
        [Tooltip("When pick this weapon for the first time, pick also ammo")] public int AmmoOnPick = 12;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        //private
        float timeForNextShot;
        Coroutine automaticShootCoroutine;

        //bullets
        Pooling<Bullet> bulletsPooling = new Pooling<Bullet>();
        Transform _bulletsParent;
        Transform BulletsParent { get { if (_bulletsParent == null) _bulletsParent = new GameObject(name + "'s Bullets Parent").transform; return _bulletsParent; } }

        //events
        public System.Action<Transform> onInstantiateBullet { get; set; }
        public System.Action onShoot { get; set; }
        public System.Action onFailShoot { get; set; }
        public System.Action onPressAttack { get; set; }
        public System.Action onReleaseAttack { get; set; }

#if UNITY_EDITOR

        string[] GetAllAmmoTypes()
        {
            //get guid to every ammo in project
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            System.Collections.Generic.List<string> values = new System.Collections.Generic.List<string>();

            //default is no type of ammo
            values.Add("NONE");

            //return array with loaded assets
            Ammo ammo;
            for (int i = 0; i < guids.Length; i++)
            {
                ammo = UnityEditor.AssetDatabase.LoadAssetAtPath<Ammo>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]));
                if (ammo) values.Add(ammo.AmmoType);
            }

            return values.ToArray();
        }

#endif

        public override void PressAttack()
        {
            //check rate of fire
            if (Time.time > timeForNextShot)
            {
                //shoot
                Shoot();

                //start coroutine if automatic
                if (Automatic)
                {
                    if (automaticShootCoroutine != null)
                        StopCoroutine(automaticShootCoroutine);

                    automaticShootCoroutine = StartCoroutine(AutomaticShootCoroutine());
                }

                //call event
                onPressAttack?.Invoke();
            }
        }

        public override void ReleaseAttack()
        {
            //stop coroutine if running (automatic shoot)
            if (automaticShootCoroutine != null)
                StopCoroutine(automaticShootCoroutine);

            //call event
            onReleaseAttack?.Invoke();
        }

        #region private API

        /// <summary>
        /// Create bullet from every barrel or one random barrel
        /// </summary>
        void Shoot()
        {
            //if this weapon need ammos
            if (AmmoType != "NONE" && Owner && Owner.GetSavedComponent<AdvancedWeaponComponent>())
            {
                //if there are not enough ammos, fail shoot
                if (Owner.GetSavedComponent<AdvancedWeaponComponent>().GetCurrentAmmo(AmmoType) <= 0)
                {
                    onFailShoot?.Invoke();
                    return;
                }
                //else remove ammos
                else
                {
                    Owner.GetSavedComponent<AdvancedWeaponComponent>().AddAmmo(AmmoType, -1);
                }
            }

            //update rate of fire
            timeForNextShot = Time.time + RateOfFire;

            //shoot every bullet
            if (BarrelSimultaneously)
            {
                foreach (Transform barrel in Barrels)
                {
                    InstantiateBullet(barrel);
                }
            }
            //or shoot one bullet from random barrel
            else
            {
                InstantiateBullet(Barrels[Random.Range(0, Barrels.Length)]);
            }

            //pushback owner
            if (Owner && Owner.GetSavedComponent<MovementComponent>() && Owner.GetSavedComponent<AimComponent>())
                Owner.GetSavedComponent<MovementComponent>().PushInDirection(-Owner.GetSavedComponent<AimComponent>().AimDirectionInput, Recoil);

            //call event
            onShoot?.Invoke();
        }

        /// <summary>
        /// Instantiate bullet and set it
        /// </summary>
        /// <param name="barrel"></param>
        void InstantiateBullet(Transform barrel)
        {
            if (BulletPrefab == null)
                return;

            //create random noise in accuracy
            float randomNoiseAccuracy = Random.Range(-NoiseAccuracy, NoiseAccuracy);
            Vector2 direction = Quaternion.AngleAxis(randomNoiseAccuracy, Vector3.forward) * barrel.right;                                  //direction with noise

            //draw debug
            if (drawDebug)
                Debug.DrawLine(barrel.position, (Vector2)barrel.position + direction, Color.red, 1);

            //instantiate bullet
            Bullet bullet = bulletsPooling.Instantiate(BulletPrefab, BulletsParent);
            bullet.transform.position = barrel.position;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * direction);    //rotate direction to left, to use right as forward

            //and set it
            bullet.Init(this, Owner, direction);

            //call event
            onInstantiateBullet?.Invoke(barrel);
        }

        /// <summary>
        /// Continue shooting
        /// </summary>
        /// <returns></returns>
        IEnumerator AutomaticShootCoroutine()
        {
            while (true)
            {
                //stop if lose owner
                if (Owner == null)
                    break;

                //check rate of fire
                if (Time.time > timeForNextShot)
                {
                    //shoot
                    Shoot();
                }

                yield return null;
            }
        }

        #endregion
    }
}