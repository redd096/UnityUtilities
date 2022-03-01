using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Weapons/Bullet")]
    public class Bullet : MonoBehaviour
    {
        [Header("Necessary Components - default get from this gameObject")]
        [SerializeField] MovementComponent movementComponent = default;
        [SerializeField] CollisionComponent collisionComponent = default;

        [Header("Layer Penetrable")]
        [Tooltip("Layers to hit but not destroy bullet")] [SerializeField] LayerMask layersPenetrable = default;

        [Header("Bullet")]
        [Tooltip("When a character shoot, can hit also other characters of same type?")] [SerializeField] bool friendlyFire = true;
        [SerializeField] bool ignoreShield = false;
        [Tooltip("Knockback who hit")] [SerializeField] float knockBack = 1;

        [Header("Area Damage")]
        [SerializeField] bool doAreaDamage = false;
        [Tooltip("Damage others in radius area")] [EnableIf("doAreaDamage")] [SerializeField] [Min(0)] float radiusAreaDamage = 0;
        [EnableIf("doAreaDamage")] [SerializeField] bool ignoreShieldAreaDamage = false;
        [Tooltip("Is possible to damage owner with area damage?")] [EnableIf("doAreaDamage")] [SerializeField] bool areaCanDamageWhoShoot = false;
        [Tooltip("Is possible to damage again who hit this bullet?")] [EnableIf("doAreaDamage")] [SerializeField] bool areaCanDamageWhoHit = false;
        [Tooltip("Do knockback also to who hit in area?")] [EnableIf("doAreaDamage")] [SerializeField] bool knockbackAlsoInArea = true;

        [Header("Timer Autodestruction (0 = no autodestruction)")]
        [SerializeField] public float delayAutodestruction = 0;
        [EnableIf("doAreaDamage")] [SerializeField] bool doAreaDamageAlsoOnAutoDestruction = true;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;
        [ReadOnly] [SerializeField] Vector2 direction = Vector2.zero;
        [ReadOnly] [SerializeField] float damage = 0;
        [ReadOnly] [SerializeField] float bulletSpeed = 0;

        [HideInInspector] public Character Owner;
        WeaponRange weapon;
        int ownerType;
        bool alreadyDead;
        List<Redd096Main> alreadyHit = new List<Redd096Main>();
        List<Redd096Main> alreadyHitsDamageInArea = new List<Redd096Main>();

        Coroutine autodestructionCoroutine;

        //events
        public System.Action<GameObject> onHit { get; set; }        //also when penetrate something
        public System.Action<GameObject> onLastHit { get; set; }    //when hit something that destroy this bullet
        public System.Action onAutodestruction { get; set; }        //when destroy by timer
        public System.Action onDie { get; set; }                    //both hit something or destroyed by timer

        private void Awake()
        {
            //get references
            if (movementComponent == null) movementComponent = GetComponent<MovementComponent>();
            if (collisionComponent == null) collisionComponent = GetComponent<CollisionComponent>();

            //warnings
            if (movementComponent == null) Debug.LogWarning("Missing MovementComponent on " + name);
            if (collisionComponent == null) Debug.LogWarning("Missing CollisionComponent on " + name);
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

        void OnEnable()
        {
            //get references
            if (collisionComponent == null)
                collisionComponent = GetComponent<CollisionComponent>();

            //add events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter += OnRDCollisionEnter;
                collisionComponent.onTriggerEnter += OnRDCollisionEnter;
            }
        }

        void OnDisable()
        {
            //remove events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter -= OnRDCollisionEnter;
                collisionComponent.onTriggerEnter -= OnRDCollisionEnter;
            }
        }

        void Update()
        {
            //move
            if (movementComponent)
                movementComponent.MoveInDirection(direction, bulletSpeed);
        }

        /// <summary>
        /// Initialize bullet
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="owner"></param>
        /// <param name="direction"></param>
        public void Init(WeaponRange weapon, Character owner, Vector2 direction)
        {
            //set weapon and initialize with it
            this.weapon = weapon;
            Init(owner, direction, weapon.Damage, weapon.BulletSpeed);
        }

        /// <summary>
        /// Initialize bullet without using a WeaponRange
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="direction"></param>
        /// <param name="damage"></param>
        /// <param name="bulletSpeed"></param>
        /// <param name="delayAutoDestruction"></param>
        public void Init(Character owner, Vector2 direction, float damage, float bulletSpeed, float autodestruction = 0)
        {
            //reset vars
            alreadyDead = false;
            alreadyHit.Clear();

            this.direction = direction;
            this.damage = damage;
            this.bulletSpeed = bulletSpeed;

            this.Owner = owner;
            ownerType = Owner ? (int)Owner.CharacterType : -1;  //if is not a character, set type to -1

            //ignore every collision with owner
            if (Owner && collisionComponent)
            {
                foreach (Collider2D ownerCol in Owner.GetComponentsInChildren<Collider2D>())
                    foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                        collisionComponent.IgnoreCollision(bulletCol, ownerCol);
            }

            //ignore every collision with weapon
            if (weapon && collisionComponent)
            {
                foreach (Collider2D weaponCol in weapon.GetComponentsInChildren<Collider2D>())
                    foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                        collisionComponent.IgnoreCollision(bulletCol, weaponCol);
            }

            //if passed autodestruction is greater then 0, use it. Else keep bullet delay
            if (autodestruction > Mathf.Epsilon)
                delayAutodestruction = autodestruction;

            //autodestruction coroutine
            if (delayAutodestruction > 0)
            {
                autodestructionCoroutine = StartCoroutine(AutoDestructionCoroutine());
            }
        }

        void OnRDCollisionEnter(RaycastHit2D collision)
        {
            OnHit(collision);
        }

        #region private API

        void OnHit(RaycastHit2D collision)
        {
            if (alreadyDead)
                return;

            //be sure to hit something
            if (collision == false || collision.transform == false)
                return;

            GameObject hit = collision.transform.gameObject;

            //be sure to not hit owner
            if (Owner && hit.GetComponentInParent<Character>() == Owner)
                return;

            //don't hit again same object (for penetrate shots)
            Redd096Main hitMain = hit.GetComponentInParent<Redd096Main>();
            if (hitMain && alreadyHit.Contains(hitMain))
                return;

            //if friendly fire is disabled, be sure to not hit same type of character
            if (friendlyFire == false
                && hitMain is Character hitCharacter && (int)hitCharacter.CharacterType == ownerType)
                return;

            //call event
            onHit?.Invoke(hit);

            if (hitMain)
            {
                alreadyHit.Add(hitMain);

                //if hit something, do damage and push back
                if (hitMain.GetSavedComponent<HealthComponent>()) hitMain.GetSavedComponent<HealthComponent>().GetDamage(damage, Owner, collision.point, ignoreShield);
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
            alreadyHitsDamageInArea.Clear();

            //be sure to not hit owner (if necessary)
            if (areaCanDamageWhoShoot == false && Owner)
                alreadyHitsDamageInArea.Add(Owner);

            //be sure to not hit who was already hit by bullet (if necessary)
            if (areaCanDamageWhoHit == false && hit != null)
                alreadyHitsDamageInArea.Add(hit);

            //find every object damageable in area
            Redd096Main hitMain;
            foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusAreaDamage))
            {
                hitMain = col.GetComponentInParent<Redd096Main>();

                if (hitMain != null && alreadyHitsDamageInArea.Contains(hitMain) == false                                       //be sure hit something and is not already hitted
                    && collisionComponent && collisionComponent.CanHit(col) != CollisionComponent.ECollisionResponse.Ignore)    //be sure can be hit
                {
                    alreadyHitsDamageInArea.Add(hitMain);

                    //do damage
                    if (hitMain.GetSavedComponent<HealthComponent>()) hitMain.GetSavedComponent<HealthComponent>().GetDamage(damage, Owner, transform.position, ignoreShieldAreaDamage);

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