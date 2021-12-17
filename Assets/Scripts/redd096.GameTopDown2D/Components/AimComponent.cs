﻿using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Aim Component")]
    public class AimComponent : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;
        [ReadOnly] public bool IsLookingRight = true;           //check if looking right
        [ReadOnly] public Vector2 AimDirectionInput;            //when aim, set it with only direction (used to know where this object is aiming)
        [ReadOnly] public Vector2 AimPositionNotNormalized;     //when aim, set it without normalize (used to set crosshair on screen - to know mouse position or analog inclination)

        //events
        public System.Action<bool> onChangeAimDirection { get; set; }

        void OnDrawGizmos()
        {
            //draw sphere to see where is aiming
            if (drawDebug)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere((Vector2)transform.position + AimPositionNotNormalized, 1);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere((Vector2)transform.position + AimDirectionInput, 1);
            }
        }

        bool CheckIsLookingRight()
        {
            //check if change direction
            if (IsLookingRight && AimDirectionInput.x < 0)
                return false;
            else if (IsLookingRight == false && AimDirectionInput.x > 0)
                return true;

            //else return previous direction
            return IsLookingRight;
        }

        #region public API

        /// <summary>
        /// Set aim in direction
        /// </summary>
        /// <param name="aimPosition">aim not normalized</param>
        public void AimAt(Vector2 aimPosition)
        {
            //set direction aim
            AimPositionNotNormalized = aimPosition;
            AimDirectionInput = aimPosition.normalized;

            //set if change aim direction
            if (IsLookingRight != CheckIsLookingRight())
            {
                IsLookingRight = CheckIsLookingRight();

                //call event
                onChangeAimDirection?.Invoke(IsLookingRight);
            }
        }

        #endregion
    }
}