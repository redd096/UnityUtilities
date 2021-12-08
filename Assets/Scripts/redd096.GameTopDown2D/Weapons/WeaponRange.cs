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
        public float RateOfFire = 0.2f;
        [Tooltip("Push back when shoot")] public float Recoil = 1;
        [Tooltip("Rotate random the shot when instantiated")] public float NoiseAccuracy = 10;

        [Header("Barrel")]
        [Tooltip("Spawn bullets")] public Transform[] Barrels = default;
        [Tooltip("When more than one barrel, shoot every bullet simultaneously or from one random?")] public bool BarrelSimultaneously = true;

        [Header("Bullet")]
        public Bullet BulletPrefab = default;
        public float Damage = 10;
        public float BulletSpeed = 10;

        [Header("Ammo")]
        public bool hasAmmo = true;
        /*[OnValueChanged("ChangedMaxAmmo")]*/ [CanEnable("hasAmmo")] [Min(0)] public int maxAmmo = 32;
        [ReadOnly] public int currentAmmo = 32;
        [CanEnable("hasAmmo")] public float delayReload = 1;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        //private
        float timeForNextShot;
        Coroutine automaticShootCoroutine;
        Coroutine reloadCoroutine;

        //bullets
        Pooling<Bullet> bulletsPooling = new Pooling<Bullet>();
        GameObject bulletsParent;
        GameObject BulletsParent
        {
            get
            {
                if (bulletsParent == null)
                    bulletsParent = new GameObject("Bullets Parent");

                return bulletsParent;
            }
        }

        //events
        public System.Action<Transform> onInstantiateBullet { get; set; }
        public System.Action onShoot { get; set; }
        public System.Action onPressAttack { get; set; }
        public System.Action onReleaseAttack { get; set; }

        private void OnValidate()
        {
            //use instead of attribute OnValueChanged
            if (currentAmmo != maxAmmo)
                ChangedMaxAmmo();
        }

        void ChangedMaxAmmo()
        {
            //when change max ammo, reset current ammo (used by editor)
            currentAmmo = maxAmmo;
        }

        public override void PressAttack()
        {
            //check rate of fire
            if (Time.time > timeForNextShot)
            {
                timeForNextShot = Time.time + RateOfFire;

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

        public void Reload()
        {
            //do only if this weapon has ammo, and is not full
            if (hasAmmo && currentAmmo < maxAmmo)
            {
                //start reload coroutine
                if (reloadCoroutine == null)
                    reloadCoroutine = StartCoroutine(ReloadCoroutine());
            }
        }

        public void AbortReload()
        {
            //stop reload coroutine if running
            if(reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
            }
        }

        #region private API

        /// <summary>
        /// Create bullet from every barrel or one random barrel
        /// </summary>
        void Shoot()
        {
            //if this weapon has ammo
            if(hasAmmo)
            {
                //if there are not enough ammos, start reload instead of shoot
                if(currentAmmo <= 0)
                {
                    Reload();
                    return;
                }
                //else remove ammos
                else
                {
                    currentAmmo--;
                    AbortReload();  //be sure is not reloading
                }
            }

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
                Transform barrel = Barrels[Random.Range(0, Barrels.Length)];
                InstantiateBullet(barrel);
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
            //create random noise in accuracy
            float randomNoiseAccuracy = Random.Range(-NoiseAccuracy, NoiseAccuracy);
            Vector2 direction = Quaternion.AngleAxis(randomNoiseAccuracy, Vector3.forward) * barrel.right;                                  //direction with noise

            //draw debug
            if(drawDebug)
                Debug.DrawLine(barrel.position, (Vector2)barrel.position + direction, Color.red, 1);

            //instantiate bullet
            Bullet bullet = bulletsPooling.Instantiate(BulletPrefab, BulletsParent.transform);
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
                    timeForNextShot = Time.time + RateOfFire;

                    //shoot
                    Shoot();
                }

                yield return null;
            }
        }

        IEnumerator ReloadCoroutine()
        {
            //wait
            yield return new WaitForSeconds(delayReload);

            //then reload ammos
            currentAmmo = maxAmmo;

            reloadCoroutine = null;
        }

        #endregion
    }
}