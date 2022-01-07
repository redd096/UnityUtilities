using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Move Weapon Feedback")]
    public class MoveWeaponFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] WeaponBASE weaponBASE;

        [Header("Pivot - default is this transform")]
        [SerializeField] Transform objectPivot = default;
        [SerializeField] float offsetFromCharacter = 0.5f;

        void OnEnable()
        {
            //get references
            if (weaponBASE == null) weaponBASE = GetComponentInParent<WeaponBASE>();
            if (objectPivot == null) objectPivot = transform;
        }

        void Update()
        {
            //set position using Aim
            MoveWeapon();
        }

        void MoveWeapon()
        {
            //move to owner + offset
            if (weaponBASE && weaponBASE.Owner && weaponBASE.Owner.GetSavedComponent<AimComponent>())
            {
                objectPivot.position = (Vector2)weaponBASE.Owner.transform.position + (weaponBASE.Owner.GetSavedComponent<AimComponent>().AimDirectionInput * offsetFromCharacter);
            }
        }
    }
}