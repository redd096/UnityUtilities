using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Weapons/Bullet")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [Header("Necessary Components - default get from this gameObject")]
        [Tooltip("Set drag to 0 to not stop bullet")] [SerializeField] MovementComponent movementComponent;

        [Header("Layer Penetrable")]
        [Tooltip("Layers to hit but not destroy bullet")] [SerializeField] LayerMask layersPenetrable = default;
        [Tooltip("Layers to ignore (no hit and no destroy bullet)")] [SerializeField] LayerMask layersToIgnore = default;
        [Tooltip("Ignore trigger colliders")] [SerializeField] bool ignoreTriggers = true;

        [Header("Bullet")]
        [Tooltip("When a character shoot, can hit also other characters of same type?")] [SerializeField] bool friendlyFire = true;
        [SerializeField] bool ignoreShield = false;
        [Tooltip("Knockback who hit")] [SerializeField] float knockBack = 1;

        [Header("Area Damage")]
        [SerializeField] bool doAreaDamage = false;
        [Tooltip("Damage others in radius area")] [CanEnable("doAreaDamage")] [SerializeField] [Min(0)] float radiusAreaDamage = 0;
        [CanEnable("doAreaDamage")] [SerializeField] bool ignoreShieldAreaDamage = false;
        [Tooltip("Is possible to damage owner with area damage?")] [CanEnable("doAreaDamage")] [SerializeField] bool areaCanDamageWhoShoot = false;
        [Tooltip("Is possible to damage again who hit this bullet?")] [CanEnable("doAreaDamage")] [SerializeField] bool areaCanDamageWhoHit = false;
        [Tooltip("Do knockback also to who hit in area?")] [CanEnable("doAreaDamage")] [SerializeField] bool knockbackAlsoInArea = true;

        [Header("Timer Autodestruction (0 = no autodestruction)")]
        [SerializeField] public float delayAutodestruction = 0;
        [CanEnable("doAreaDamage")] [SerializeField] bool doAreaDamageAlsoOnAutoDestruction = true;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;
        [ReadOnly] [SerializeField] Vector2 direction = Vector2.zero;
        [ReadOnly] [SerializeField] float damage = 0;
        [ReadOnly] [SerializeField] float bulletSpeed = 0;

        Character owner;
        WeaponRange weapon;
        int ownerType;
        bool alreadyDead;
        List<Redd096Main> alreadyHit = new List<Redd096Main>();

        Coroutine autodestructionCoroutine;

        //events
        public System.Action<GameObject> onHit { get; set; }        //also when penetrate something
        public System.Action<GameObject> onLastHit { get; set; }    //when hit something that destroy this bullet
        public System.Action onAutodestruction { get; set; }        //when destroy by timer
        public System.Action onDie { get; set; }                    //both hit something or destroyed by timer

        private void Awake()
        {
            //get references
            if(movementComponent == null)
                movementComponent = GetComponent<MovementComponent>();

            if (movementComponent == null)
                Debug.LogWarning("Missing MovementComponent on " + name);
        }

        void OnDrawGizmos()
        {
            //draw area damage
            if (drawDebug)
            {
                if (doAreaDamage && radiusAreaDamage > 0)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(transform.position, radiusAreaDamage);
                }
            }
        }

        /// <summary>
        /// Initialize bullet
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="owner"></param>
        /// <param name="direction"></param>
        public void Init(WeaponRange weapon, Character owner, Vector2 direction)
        {
            //reset vars
            alreadyDead = false;
            alreadyHit.Clear();

            this.direction = direction;
            this.damage = weapon.Damage;
            this.bulletSpeed = weapon.BulletSpeed;

            this.owner = owner;
            this.weapon = weapon;
            ownerType = owner ? (int)owner.CharacterType : -1;  //if is not a character, set type to -1

            //ignore every collision with owner
            if (owner)
            {
                foreach (Collider2D ownerCol in owner.GetComponentsInChildren<Collider2D>())
                    foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                        Physics2D.IgnoreCollision(bulletCol, ownerCol);
            }

            //ignore every collision with weapon
            if (weapon)
            {
                foreach (Collider2D weaponCol in weapon.GetComponentsInChildren<Collider2D>())
                    foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                        Physics2D.IgnoreCollision(bulletCol, weaponCol);
            }

            //autodestruction coroutine
            if (delayAutodestruction > 0)
            {
                autodestructionCoroutine = StartCoroutine(AutoDestructionCoroutine());
            }

            //push
            if (movementComponent)
                movementComponent.PushInDirection(direction, bulletSpeed, true);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            OnHit(collision);
        }

        #region private API

        void OnHit(Collision2D collision)
        {
            if (alreadyDead)
                return;

            GameObject hit = collision.gameObject;

            //be sure to hit something and be sure to not hit layers to ignore
            if (hit == null || ContainsLayer(layersToIgnore, hit.layer)
                || (ignoreTriggers && collision.collider.isTrigger))        //if ignore triggers, be sure to not hit triggers
                return;

            //be sure to not hit other bullets or weapons or this owner
            if (hit.GetComponentInParent<Bullet>() || hit.GetComponentInParent<WeaponRange>() || (owner && hit.GetComponentInParent<Character>() == owner))
                return;

            //don't hit again same object (for penetrate shots)
            Redd096Main hitMain = hit.GetComponentInParent<Redd096Main>();
            if (hitMain && alreadyHit.Contains(hitMain))
                return;

            //if friendly fire is disabled, be sure to not hit same type of character
            if (friendlyFire == false
                && hitMain is Character && ((int)((Character)hitMain).CharacterType == ownerType))
                return;

            //call event
            onHit?.Invoke(hit);

            if (hitMain)
            {
                alreadyHit.Add(hitMain);

                //if hit something, do damage and push back
                if (hitMain.GetSavedComponent<HealthComponent>()) hitMain.GetSavedComponent<HealthComponent>().GetDamage(damage);
                if (hitMain && hitMain.GetSavedComponent<MovementComponent>()) hitMain.GetSavedComponent<MovementComponent>().PushInDirection(direction, knockBack);
            }

            //if is not a penetrable layer, destroy this object
            if (ContainsLayer(layersPenetrable, hit.layer) == false)
            {
                //call event
                onLastHit?.Invoke(hit);

                //damage in area too
                if (doAreaDamage && radiusAreaDamage > 0)
                    DamageInArea(hitMain);

                //destroy
                Die();
            }
        }

        bool ContainsLayer(LayerMask layerMask, int layerToCompare)
        {
            //if add layer to this layermask, and layermask remain equals, then layermask contains this layer
            return layerMask == (layerMask | (1 << layerToCompare));
        }

        void DamageInArea(Redd096Main hit)
        {
            //be sure to not hit again the same
            List<Redd096Main> hits = new List<Redd096Main>();

            //be sure to not hit owner (if necessary)
            if (areaCanDamageWhoShoot == false && owner)
                hits.Add(owner);

            //be sure to not hit who was already hit by bullet (if necessary)
            if (areaCanDamageWhoHit == false && hit != null)
                hits.Add(hit);

            //find every object damageable in area
            foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusAreaDamage))
            {
                Redd096Main hitMain = col.GetComponentInParent<Redd096Main>();

                if (hitMain != null && hits.Contains(hitMain) == false                      //be sure hit something and is not already hitted
                    && ContainsLayer(layersToIgnore, hitMain.gameObject.layer) == false     //be sure is not in layers to ignore
                    && (ignoreTriggers == false || col.isTrigger == false))                 //be sure is not enabled ignoreTriggers, or is not trigger
                {
                    hits.Add(hitMain);

                    //do damage
                    if (hitMain.GetSavedComponent<HealthComponent>()) hitMain.GetSavedComponent<HealthComponent>().GetDamage(damage);

                    //and knockback if necessary
                    if (knockbackAlsoInArea && hitMain && hitMain.GetSavedComponent<MovementComponent>())
                    {
                        hitMain.GetSavedComponent<MovementComponent>().PushInDirection((col.transform.position - transform.position).normalized, knockBack);
                    }
                }
            }
        }

        IEnumerator AutoDestructionCoroutine()
        {
            //wait
            yield return new WaitForSeconds(delayAutodestruction);

            //only if not already dead
            if (alreadyDead)
                yield break;

            //do damage in area too
            if (doAreaDamageAlsoOnAutoDestruction)
            {
                if (doAreaDamage && radiusAreaDamage > 0)
                    DamageInArea(null);
            }

            //call event
            onAutodestruction?.Invoke();

            //then destroy
            Die();
        }

        #endregion

        void Die()
        {
            alreadyDead = true;

            //if coroutine is running, stop it
            if (autodestructionCoroutine != null)
                StopCoroutine(autodestructionCoroutine);

            //call event
            onDie?.Invoke();

            //destroy bullet
            Pooling.Destroy(gameObject);
        }
    }
}