using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Use mouse position or analog inclination to set aim direction. Use this for example to rotate player or shoot bullets
    /// </summary>
    public class TopDownAimComponent : ICharacterComponent
    {
        [Header("When set at zero (e.g. release analog), keep last rotation")]
        [SerializeField] bool ignoreDirectionZero = true;
        [SerializeField] ShowDebugRedd096 showPositionNotNormalized = Color.red;
        [SerializeField] ShowDebugRedd096 showDirectionInput = Color.cyan;

        public ICharacter Owner { get; set; }

        public bool IsAimingRight { get; set; } = true;                         //check if aiming to the right (this is used in 2d games where you can look only left or right)
        public Vector3 AimDirectionInput { get; set; } = Vector3.right;         //when aim, set it with only direction (used to know where this object is aiming)
        public Vector3 AimPositionNotNormalized { get; set; } = Vector3.right;  //when aim, set it without normalize (used to set crosshair on screen - to know mouse position or analog inclination)

        //events
        public System.Action<bool> onChangeAimDirection;

        public void OnDrawGizmosSelected()
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
                Gizmos.DrawWireSphere(Owner.transform.position + AimDirectionInput, 1);
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
            AimPositionNotNormalized = Owner.transform.position + aimDirection;
            AimDirectionInput = aimDirection.normalized;

            CheckIsAimingRight();
        }

        /// <summary>
        /// Set aim at position
        /// </summary>
        /// <param name="aimPosition"></param>
        public void AimAt(Vector3 aimPosition)
        {
            Vector3 dir = (aimPosition - Owner.transform.position).normalized;
            if (ignoreDirectionZero && dir == Vector3.zero)
                return;

            //set direction aim
            AimPositionNotNormalized = aimPosition;
            AimDirectionInput = dir;

            CheckIsAimingRight();
        }

        #endregion

        protected virtual void CheckIsAimingRight()
        {
            //set previous direction (necessary in case this object stay still)
            bool newRight = IsAimingRight;

            //update new direction
            if (IsAimingRight && AimDirectionInput.x < 0)
                newRight = false;
            else if (IsAimingRight == false && AimDirectionInput.x > 0)
                newRight = true;

            //check change direction
            if (IsAimingRight != newRight)
            {
                IsAimingRight = newRight;

                //call event
                onChangeAimDirection?.Invoke(IsAimingRight);
            }
        }
    }
}