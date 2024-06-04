using redd096.Attributes;
using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Set CurrentRotation from input. The same as RotationComponent but for TopDown games
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/CharacterComponents/Aim Component TopDown")]
    public class AimComponentTopDown : MonoBehaviour
    {
        [Header("When set at zero (e.g. release analog), keep last rotation")]
        [SerializeField] bool ignoreDirectionZero = true;

        [ReadOnly] public Vector3 AimDirectionInput = Vector3.right;            //when aim, set it with only direction (used to know where this object is aiming)
        [ReadOnly] public Vector3 AimPositionNotNormalized = Vector3.right;     //when aim, set it without normalize (used to set crosshair on screen - to know mouse position or analog inclination)
        [SerializeField] ShowDebugRedd096 showPositionNotNormalized = Color.red;
        [SerializeField] ShowDebugRedd096 showDirectionInput = Color.cyan;

        protected virtual void OnDrawGizmos()
        {
            //draw sphere to see where is aiming
            if (showPositionNotNormalized)
            {
                Gizmos.color = showPositionNotNormalized.ColorDebug;
                Gizmos.DrawWireSphere(AimPositionNotNormalized, 1);
            }
            if (showDirectionInput)
            {
                Gizmos.color = showDirectionInput.ColorDebug;
                Gizmos.DrawWireSphere(transform.position + AimDirectionInput, 1);
            }
            Gizmos.color = Color.white;
        }

        #region public API

        /// <summary>
        /// Set aim in direction
        /// </summary>
        /// <param name="aimDirection"></param>
        public virtual void AimInDirection(Vector3 aimDirection)
        {
            if (ignoreDirectionZero && aimDirection == Vector3.zero)
                return;

            //set direction aim
            AimPositionNotNormalized = transform.position + aimDirection;
            AimDirectionInput = aimDirection.normalized;
        }

        /// <summary>
        /// Set aim at position
        /// </summary>
        /// <param name="aimPosition"></param>
        public void AimAt(Vector3 aimPosition)
        {
            if (ignoreDirectionZero && (aimPosition - transform.position).normalized == Vector3.zero)
                return;

            //set direction aim
            AimPositionNotNormalized = aimPosition;
            AimDirectionInput = (aimPosition - transform.position).normalized;
        }

        #endregion
    }
}