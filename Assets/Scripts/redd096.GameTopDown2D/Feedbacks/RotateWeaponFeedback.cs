using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Rotate Weapon Feedback")]
    public class RotateWeaponFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] WeaponBASE weaponBASE;

        [Header("Pivot - default is this transform")]
        [SerializeField] Transform objectPivot = default;
        [Tooltip("By default this weapon is looking to the right?")] [SerializeField] bool defaultLookRight = true;

        void OnEnable()
        {
            //get references
            if (weaponBASE == null) weaponBASE = GetComponentInParent<WeaponBASE>();
            if (objectPivot == null) objectPivot = transform;
        }

        void Update()
        {
            //rotate weapon with aim
            RotateWeapon();
        }

        void RotateWeapon()
        {
            //rotate weapon with aim (using pivot)
            if (weaponBASE && weaponBASE.Owner && weaponBASE.Owner.GetSavedComponent<AimComponent>())
            {
                Vector2 aimDirection = weaponBASE.Owner.GetSavedComponent<AimComponent>().AimDirectionInput;
                Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * aimDirection);

                //when rotate to opposite direction (from default), rotate 180 updown
                if ((defaultLookRight && aimDirection.x < 0) || (defaultLookRight == false && aimDirection.x > 0))
                {
                    rotation *= Quaternion.AngleAxis(180, Vector3.right);
                }

                //set rotation
                objectPivot.transform.rotation = rotation;
            }
        }
    }
}