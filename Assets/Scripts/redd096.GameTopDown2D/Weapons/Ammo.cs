using System.Collections;
using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Weapons/Ammo")]
    public class Ammo : MonoBehaviour
    {
        [Header("Necessary Components - default get from this gameObject")]
        [SerializeField] CollisionComponent collisionComponent = default;

        [Header("Ammo")]
        [SerializeField] string ammoType = "GunAmmo";
        [SerializeField] int quantity = 1;
        [Tooltip("Can pick when full of this type of ammo? If true, this object will be destroyed, but no ammo will be added")] [SerializeField] bool canPickAlsoIfFull = false;

        [Header("Destroy when instantiated - 0 = no destroy")]
        [SerializeField] float timeBeforeDestroy = 0;

        public string AmmoType => ammoType;

        //events
        public System.Action<Ammo> onPickAmmo { get; set; }
        public System.Action<Ammo> onFailPickAmmo { get; set; }

        Character whoHit;
        AdvancedWeaponComponent whoHitWeaponComponent;
        bool alreadyUsed;

        void OnEnable()
        {
            //reset vars
            alreadyUsed = false;

            //if there is, start auto destruction timer
            if (timeBeforeDestroy > 0)
                StartCoroutine(AutoDestruction());

            //get references
            if (collisionComponent == null) collisionComponent = GetComponent<CollisionComponent>();

            //add events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter += OnRDCollisionEvent;
                collisionComponent.onTriggerEnter += OnRDCollisionEvent;
            }
        }

        void OnDisable()
        {
            //remove events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter -= OnRDCollisionEvent;
                collisionComponent.onTriggerEnter -= OnRDCollisionEvent;
            }
        }

        void OnRDCollisionEvent(RaycastHit2D collision)
        {
            if (alreadyUsed)
                return;

            //if hitted by player
            whoHit = collision.transform.GetComponentInParent<Character>();
            if (whoHit && whoHit.CharacterType == Character.ECharacterType.Player)
            {
                //and player has weapon component
                whoHitWeaponComponent = whoHit.GetSavedComponent<AdvancedWeaponComponent>();
                if (whoHitWeaponComponent)
                {
                    //if full of ammo, can't pick, call fail event
                    if (whoHitWeaponComponent.IsFullOfAmmo(ammoType) && canPickAlsoIfFull == false)
                    {
                        onFailPickAmmo?.Invoke(this);
                    }
                    //else, pick and add quantity
                    else
                    {
                        whoHitWeaponComponent.AddAmmo(ammoType, quantity);

                        //call event
                        onPickAmmo?.Invoke(this);

                        //destroy this gameObject
                        alreadyUsed = true;
                        Destroy(gameObject);
                    }
                }
            }
        }

        IEnumerator AutoDestruction()
        {
            //wait, then destroy
            yield return new WaitForSeconds(timeBeforeDestroy);
            alreadyUsed = true;
            Destroy(gameObject);
        }
    }
}