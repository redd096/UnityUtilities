using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Use mouse or analog to set aim direction. Use this for example to rotate player or shoot bullets
    /// </summary>
    [System.Serializable]
    public class FirstPersonAimComponent : IComponentRD
    {
        [Tooltip("Used when call RotateByInput")][SerializeField] float horizontalSensitivity = 0.3f;
        [Tooltip("Used when call RotateByInput")][SerializeField] float verticalSensitivity = 0.3f;
        [Tooltip("Limit vertical rotation when look up")][Range(0f, 180f)][SerializeField] float verticalTopRotationLimit = 88f;
        [Tooltip("Limit vertical rotation when look down")][Range(-0f, -180f)][SerializeField] float verticalBottomRotationLimit = -88f;

        public IGameObjectRD Owner { get; set; }

        public Vector3 AimDirectionInput { get; set; } = Vector3.forward;           //when aim, set it with only direction (used to know where this object is aiming)

        public Vector2 CurrentEulerAngles => currentEulerAngles;    //X is Yaw rotation (horizontal) and Y is Pitch rotation (vertical)
        public Quaternion CurrentRotation => currentRotation;       //current rotation as quaternion (Yaw * Pitch)
        public Quaternion Yaw => yaw;                               //Yaw rotation as quaternion
        public Quaternion Pitch => pitch;                           //Pitch rotation as quaternion
        public float HorizontalSensitivity { get => horizontalSensitivity; set => horizontalSensitivity = value; }
        public float VerticalSensitivity { get => verticalSensitivity; set => verticalSensitivity = value; }

        //private
        private Vector2 currentEulerAngles;
        private Quaternion currentRotation;
        private Quaternion yaw = Quaternion.identity;
        private Quaternion pitch = Quaternion.identity;

        #region public API

        /// <summary>
        /// Add input to current rotation
        /// </summary>
        /// <param name="input"></param>
        public virtual void RotateByInput3D(Vector2 input)
        {
            //rotate and clamp vertical rotation
            currentEulerAngles.x += input.x * horizontalSensitivity;
            currentEulerAngles.y += input.y * verticalSensitivity;
            currentEulerAngles.y = Mathf.Clamp(currentEulerAngles.y, verticalBottomRotationLimit, verticalTopRotationLimit);

            //calculate quaternions
            yaw = Quaternion.AngleAxis(currentEulerAngles.x, Vector3.up);
            pitch = Quaternion.AngleAxis(currentEulerAngles.y, Vector3.left);
            currentRotation = Yaw * Pitch;

            //set direction aim
            AimDirectionInput = currentRotation * Vector3.forward;
        }

        /// <summary>
        /// Set aim in direction
        /// </summary>
        /// <param name="aimDirection"></param>
        public void AimInDirection(Vector3 aimDirection)
        {
            Quaternion lookRotation = Quaternion.LookRotation(aimDirection.normalized, Vector3.up);

            //rotate and clamp vertical rotation
            currentEulerAngles.x = lookRotation.eulerAngles.x;
            currentEulerAngles.y = lookRotation.eulerAngles.y;
            currentEulerAngles.y = Mathf.Clamp(currentEulerAngles.y, verticalBottomRotationLimit, verticalTopRotationLimit);

            //calculate quaternions
            yaw = Quaternion.AngleAxis(currentEulerAngles.x, Vector3.up);
            pitch = Quaternion.AngleAxis(currentEulerAngles.y, Vector3.left);
            currentRotation = Yaw * Pitch;

            //set direction aim
            AimDirectionInput = currentRotation * Vector3.forward;
        }

        /// <summary>
        /// Set aim at position (using Owner.transform as FromPosition)
        /// </summary>
        /// <param name="aimPosition"></param>
        public void AimAt(Vector3 aimPosition)
        {
            AimInDirection(aimPosition - Owner.transform.position);
        }

        /// <summary>
        /// Set aim at point
        /// </summary>
        /// <param name="from">camera position or whatever is our current position to calculate rotation</param>
        /// <param name="to">point to look at</param>
        public void AimAt(Vector3 from, Vector3 to)
        {
            AimInDirection(to - from);
        }

        /// <summary>
        /// Set aim in direction, but use X as right and Y as forward
        /// </summary>
        /// <param name="aimDirection"></param>
        public void AimInDirectionByInput3D(Vector2 aimDirection)
        {
            AimInDirection(new Vector3(aimDirection.x, 0, aimDirection.y));
        }

        /// <summary>
        /// Set aim at position, but use X as right and Y as forward
        /// </summary>
        /// <param name="aimPosition"></param>
        public void AimAtByInput3D(Vector2 aimPosition)
        {
            AimAt(new Vector3(aimPosition.x, 0, aimPosition.y));
        }

        #endregion
    }
}