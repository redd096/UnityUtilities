using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Weapon Component")]
    public class WeaponComponent : MonoBehaviour
    {
        public enum EWeaponOnDeath { None, OnlyEquippedOne, EveryWeapon }

        [Header("Instantiate default weapons")]
        [SerializeField] WeaponBASE[] weaponsPrefabs = default;

        [Header("Number of Weapons")]
        [Min(1)] [SerializeField] int maxWeapons = 2;

        [Header("Destroy Weapon On Death (necessary HealthComponent - default get from this gameObject)")]
        [SerializeField] EWeaponOnDeath dropWeaponOnDeath = EWeaponOnDeath.None;
        [SerializeField] EWeaponOnDeath destroyWeaponOnDeath = EWeaponOnDeath.EveryWeapon;
        [SerializeField] HealthComponent healthComponent = default;

        [Header("DEBUG")]
        [ReadOnly] public WeaponBASE[] CurrentWeapons = default;    //it will be always the same size of Max Weapons
        /*[ShowNonSerializedField]*/ int indexEquippedWeapon = 0;   //it will be always the correct index, or zero

        //the equipped weapon
        public WeaponBASE CurrentWeapon => CurrentWeapons != null && indexEquippedWeapon < CurrentWeapons.Length ? CurrentWeapons[indexEquippedWeapon] : null;

        //events
        public System.Action onPickWeapon { get; set; }         //called at every pick
        public System.Action onDropWeapon { get; set; }         //called at every drop
        public System.Action onSwitchWeapon { get; set; }       //called when use Switch Weapon
        public System.Action onChangeWeapon { get; set; }       //called at every pick and every drop. Also when switch weapon

        Character owner;
        Transform _currentWeaponsParent;
        Transform CurrentWeaponsParent { get { if (_currentWeaponsParent == null) _currentWeaponsParent = new GameObject(name + "'s Weapons").transform; return _currentWeaponsParent; } }

        void Awake()
        {
            //set vars
            CurrentWeapons = new WeaponBASE[maxWeapons];

            //get references
            owner = GetComponent<Character>();

            //instantiate default weapons
            SetDefaultWeapons();
        }

        void OnEnable()
        {
            //get references
            if (healthComponent == null)
                healthComponent = GetComponent<HealthComponent>();

            //add events
            if (healthComponent)
            {
                healthComponent.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            //remove events
            if (healthComponent)
            {
                healthComponent.onDie -= OnDie;
            }
        }

        #region private API

        void SetDefaultWeapons()
        {
            //instantiate and equip default weapons
            if (weaponsPrefabs != null && weaponsPrefabs.Length > 0)
            {
                for (int i = 0; i < CurrentWeapons.Length; i++)
                {
                    if (i < weaponsPrefabs.Length)
                    {
                        if (weaponsPrefabs[i])
                            PickWeapon(Instantiate(weaponsPrefabs[i]));
                    }
                    else
                        break;
                }
            }
        }

        void OnDie(HealthComponent whoDied)
        {
            //clone weapons to destroy also if dropped
            WeaponBASE[] tempWeapons = CurrentWeapons.Clone() as WeaponBASE[];

            //drop equipped weapon on death
            if (dropWeaponOnDeath == EWeaponOnDeath.OnlyEquippedOne)
            {
                DropWeapon(indexEquippedWeapon);
            }
            //or drop every weapon
            else if (dropWeaponOnDeath == EWeaponOnDeath.EveryWeapon)
            {
                if (CurrentWeapons != null)
                    for (int i = 0; i < CurrentWeapons.Length; i++)
                        DropWeapon(i);
            }

            //destroy equipped weapon on death
            if (destroyWeaponOnDeath == EWeaponOnDeath.OnlyEquippedOne)
            {
                if (tempWeapons != null && indexEquippedWeapon < tempWeapons.Length && tempWeapons[indexEquippedWeapon])
                    Destroy(tempWeapons[indexEquippedWeapon]);
            }
            //or destroy every weapon
            else if (destroyWeaponOnDeath == EWeaponOnDeath.EveryWeapon)
            {
                if (tempWeapons != null)
                    for (int i = 0; i < tempWeapons.Length; i++)
                        if (tempWeapons[i])
                            Destroy(tempWeapons[i]);
            }
        }

        bool UpdateIndexEquippedWeapon()
        {
            //if there are not weapons, set index to 0
            if (CurrentWeapons == null || CurrentWeapons.Length <= 0)
            {
                //return changed weapon
                indexEquippedWeapon = 0;
                return true;
            }

            //if current weapon is not null, keep it
            if (indexEquippedWeapon < CurrentWeapons.Length && CurrentWeapons[indexEquippedWeapon])
                return false;

            //else move to previous weapons - be sure to start from array length (if index is greater). Start from length instead of length -1 because for cycle start substracting 1
            if (indexEquippedWeapon > CurrentWeapons.Length)
                indexEquippedWeapon = CurrentWeapons.Length;

            //be sure to cycle every weapon in array
            for (int i = 0; i < CurrentWeapons.Length; i++)
            {
                indexEquippedWeapon--;

                //if reach array limit, restart
                if (indexEquippedWeapon < 0)
                    indexEquippedWeapon = CurrentWeapons.Length - 1;

                //if found weapon not null, set it
                if (indexEquippedWeapon < CurrentWeapons.Length)
                {
                    if (CurrentWeapons[indexEquippedWeapon])
                    {
                        indexEquippedWeapon = i;

                        //and active
                        CurrentWeapons[indexEquippedWeapon].transform.position = transform.position;
                        CurrentWeapons[indexEquippedWeapon].gameObject.SetActive(true);

                        //return is changing weapon
                        return true;
                    }
                }
            }

            //if not found weapon, set at 0
            indexEquippedWeapon = 0;
            return true;
        }

        #endregion

        #region public API

        /// <summary>
        /// Pick Weapon and set at index
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="index"></param>
        public void PickWeapon(WeaponBASE weapon, int index)
        {
            if (CurrentWeapons == null || CurrentWeapons.Length <= 0)
                return;

            //if there is already a weapon equipped, drop it
            if (CurrentWeapons[index] != null)
                DropWeapon(index);

            //pick weapon
            CurrentWeapons[index] = weapon;

            //set owner and parent
            if (weapon)
            {
                weapon.PickWeapon(owner);
                weapon.transform.SetParent(CurrentWeaponsParent);
                foreach (Collider2D col in weapon.GetComponentsInChildren<Collider2D>()) col.enabled = false;   //deactive colliders (necessary to not pick again when press interact)

                //if not equipped, deactive
                if (index != indexEquippedWeapon) weapon.gameObject.SetActive(false);
            }

            //set index equipped weapon
            UpdateIndexEquippedWeapon();

            //call events
            onPickWeapon?.Invoke();
            onChangeWeapon?.Invoke();
        }

        /// <summary>
        /// Pick Weapon (add in an empty slot or replace equipped one with it)
        /// </summary>
        /// <param name="weapon"></param>
        public void PickWeapon(WeaponBASE weapon)
        {
            //find empty slot (or equipped one)
            int index = indexEquippedWeapon;
            for (int i = 0; i < CurrentWeapons.Length; i++)
            {
                if (CurrentWeapons[i] == null)
                {
                    index = i;
                    break;
                }
            }

            PickWeapon(weapon, index);
        }

        /// <summary>
        /// Drop Weapon at index
        /// </summary>
        public void DropWeapon(int index)
        {
            if (CurrentWeapons == null || index >= CurrentWeapons.Length)
                return;

            //remove owner and parent
            if (CurrentWeapons[index])
            {
                CurrentWeapons[index].DropWeapon();
                CurrentWeapons[index].transform.SetParent(null);
                foreach (Collider2D col in CurrentWeapons[index].GetComponentsInChildren<Collider2D>()) col.enabled = true;   //reactive colliders

                //if not equipped, reactive
                if (index != indexEquippedWeapon) CurrentWeapons[index].gameObject.SetActive(true);
            }

            //drop weapon
            CurrentWeapons[index] = null;

            //set index equipped weapon
            UpdateIndexEquippedWeapon();

            //call event
            onDropWeapon?.Invoke();
            onChangeWeapon?.Invoke();
        }

        /// <summary>
        /// Drop equipped weapon
        /// </summary>
        public void DropWeapon()
        {
            DropWeapon(indexEquippedWeapon);
        }

        /// <summary>
        /// Set max number of weapons, and update array
        /// </summary>
        /// <param name="maxWeapons">Min number is 1</param>
        public void SetMaxWeapons(int maxWeapons)
        {
            //set max weapons
            this.maxWeapons = maxWeapons;

            //copy weapons
            WeaponBASE[] weapons = new WeaponBASE[maxWeapons];
            if (CurrentWeapons != null)
            {
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (i < CurrentWeapons.Length)
                        weapons[i] = CurrentWeapons[i];
                    else
                        break;
                }
            }
            CurrentWeapons = weapons;

            //set index equipped weapon
            if (UpdateIndexEquippedWeapon())
            {
                //if changed weapon, call event
                onChangeWeapon?.Invoke();
            }
        }

        /// <summary>
        /// Switch Weapon
        /// </summary>
        /// <param name="nextWeapon">move to next in array or previous?</param>
        public void SwitchWeapon(bool nextWeapon = true)
        {
            if (CurrentWeapons == null || CurrentWeapons.Length <= 0)
                return;

            //move to next or previous weapons
            int i = indexEquippedWeapon + (nextWeapon ? 1 : -1);
            while (i != indexEquippedWeapon)
            {
                //if reach array limit, restart
                if (nextWeapon && i >= CurrentWeapons.Length)
                    i = 0;
                else if (nextWeapon == false && i < 0)
                    i = CurrentWeapons.Length - 1;

                //if found weapon not null, set it
                if (i >= 0 && i < CurrentWeapons.Length)
                {
                    if (CurrentWeapons[i])
                    {
                        //deactive previous weapon
                        if (indexEquippedWeapon < CurrentWeapons.Length && CurrentWeapons[indexEquippedWeapon])
                            CurrentWeapons[indexEquippedWeapon].gameObject.SetActive(false);

                        //and active new one
                        CurrentWeapons[i].transform.position = transform.position;
                        CurrentWeapons[i].gameObject.SetActive(true);

                        indexEquippedWeapon = i;

                        //call event
                        onChangeWeapon?.Invoke();
                        break;
                    }
                }
            }
        }

        #endregion
    }
}