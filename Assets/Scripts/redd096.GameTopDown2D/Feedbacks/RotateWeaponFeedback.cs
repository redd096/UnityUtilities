using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Rotate Weapon Feedback")]
    public class RotateWeaponFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] WeaponBASE weaponBASE;

        [Header("Sprites to flip - default get in children")]
        [SerializeField] SpriteRenderer[] spritesToFlip = default;

        [Header("Pivot - default is this transform")]
        [SerializeField] Transform objectPivot = default;

        void OnEnable()
        {
            //get references
            if (weaponBASE == null) weaponBASE = GetComponentInParent<WeaponBASE>();
            if (spritesToFlip == null || spritesToFlip.Length <= 0) spritesToFlip = GetComponentsInChildren<SpriteRenderer>();
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
                objectPivot.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * aimDirection);

                //when rotate to left, flip Y to not be upside down
                foreach (SpriteRenderer sprite in spritesToFlip)
                    sprite.flipY = aimDirection.x < 0;
            }
        }
    }
}