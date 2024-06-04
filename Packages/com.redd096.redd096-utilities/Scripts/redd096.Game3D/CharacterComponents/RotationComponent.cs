using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Set CurrentRotation from input. Can also rotate rigidbody
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/CharacterComponents/Rotation Component")]
    public class RotationComponent : MonoBehaviour
    {
        [Tooltip("Used when call RotateByInput")][SerializeField] float xSensitivity = 0.3f;
        [Tooltip("Used when call RotateByInput")][SerializeField] float ySensitivity = 0.3f;
        [Range(0f, 180f)][SerializeField] float yTopRotationLimit = 88f;
        [Range(-0f, -180f)][SerializeField] float yBottomRotationLimit = -88f;

        [Header("Rotate rigidbody if setted")]
        [SerializeField] Rigidbody rb;

        public Vector2 CurrentRotation => currentRotation;  //X is Yaw rotation (horizontal) and Y is Pitch rotation (vertical)
        public Quaternion XQuat => xQuat;                   //Yaw rotation as quaternion
        public Quaternion YQuat => yQuat;                   //Pitch rotation as quaternion
        public float XSensitivity { get => xSensitivity; set => xSensitivity = value; }
        public float YSensitivity { get => ySensitivity; set => ySensitivity = value; }

        //private
        private Vector2 currentRotation;
        private Quaternion xQuat = Quaternion.identity;
        private Quaternion yQuat = Quaternion.identity;

        protected virtual void FixedUpdate()
        {
            //rotate rb
            if (rb) rb.rotation = xQuat;
        }

        #region public API

        /// <summary>
        /// Add input to current rotation
        /// </summary>
        /// <param name="input"></param>
        public virtual void RotateByInput3D(Vector2 input)
        {
            //rotate and clamp vertical rotation
            currentRotation.x += input.x * xSensitivity;
            currentRotation.y += input.y * ySensitivity;
            currentRotation.y = Mathf.Clamp(currentRotation.y, yBottomRotationLimit, yTopRotationLimit);

            //calculate quaternions
            xQuat = Quaternion.AngleAxis(currentRotation.x, Vector3.up);
            yQuat = Quaternion.AngleAxis(currentRotation.y, Vector3.left);
        }

        /// <summary>
        /// Set rotation to look in direction
        /// </summary>
        /// <param name="direction"></param>
        public virtual void LookInDirection(Vector3 direction)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            //rotate and clamp vertical rotation
            currentRotation.x = lookRotation.eulerAngles.x;
            currentRotation.y = lookRotation.eulerAngles.y;
            currentRotation.y = Mathf.Clamp(currentRotation.y, yBottomRotationLimit, yTopRotationLimit);

            //calculate quaternions
            xQuat = Quaternion.AngleAxis(currentRotation.x, Vector3.up);
            yQuat = Quaternion.AngleAxis(currentRotation.y, Vector3.left);
        }

        /// <summary>
        /// Look at point
        /// </summary>
        /// <param name="from">camera position or whatever is our current position to calculate rotation</param>
        /// <param name="to">point to look at</param>
        public virtual void LookAt(Vector3 from, Vector3 to)
        {
            LookInDirection(to - from);
        }

        #endregion
    }
}