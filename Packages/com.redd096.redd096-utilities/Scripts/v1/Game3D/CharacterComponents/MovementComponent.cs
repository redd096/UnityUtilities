using redd096.Attributes;
using UnityEngine;

namespace redd096.v1.Game3D
{
    /// <summary>
    /// Move the rigidbody by using the input from public API
    /// </summary>
    [AddComponentMenu("redd096/v1/Game3D/CharacterComponents/Movement Component")]
    public class MovementComponent : MonoBehaviour
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] Rigidbody rb = default;
        [Tooltip("Speed when call Move functions")][SerializeField] float inputSpeed = 5;
        [Tooltip("Max Speed, calculating velocity by input + push (-1 = no limit)")][SerializeField] float maxSpeed = 50;

        [Header("When pushed")]
        [Tooltip("Drag based on velocity * drag or normalized velocity * drag?")][SerializeField] bool dragBasedOnVelocity = true;
        [Tooltip("Use rigidbody drag or custom drag?")][SerializeField] bool useCustomDrag = false;
        [EnableIf("useCustomDrag")][SerializeField] float customDrag = 5;

        public Vector3 MoveDirectionInput { get; set; }         //when moves, set it with only input direction (used to know last movement direction)
        public Vector3 LastDesiredVelocity { get; set; }        //when moves, set it as input direction * speed
        public Vector3 CurrentPushForce { get; set; }           //used to push this object (push by recoil, knockback, dash, etc...), will be decreased by drag in every frame
        public Vector3 CurrentVelocity => rb ? rb.linearVelocity : Vector3.zero;
        public float CurrentSpeed => rb ? rb.linearVelocity.magnitude : 0;
        public float InputSpeed { get => inputSpeed; set => inputSpeed = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        /// <summary>
        /// If using custom drag, get and set customDrag. Else get and set rigidbody.drag
        /// </summary>
        public float Drag
        {
            get => useCustomDrag ? customDrag : (rb ? rb.linearDamping : 1);
            set { if (useCustomDrag) customDrag = value; else if (rb) rb.linearDamping = value; }
        }

        //private
        Vector3 desiredVelocity;                //when moves, set it as input direction * speed (used to move this object, will be reset in every frame)
        Vector3 calculatedVelocity;             //desiredVelocity + DesiredPushForce
        Vector3 newPushForce;                   //new push force when Drag

        protected virtual void Awake()
        {
            //be sure to have a components
            if (rb == null && TryGetComponent(out rb) == false)
                Debug.LogError("Miss Rigidbody on " + name);
        }

        protected virtual void FixedUpdate()
        {
            //set velocity (input + push)
            CalculateVelocity();
            DoMovement();

            //reset movement input (cause if nobody call an update, this object must to be still in next frame), but save in LastDesiredVelocity (to read in inspector or if some script need it)
            LastDesiredVelocity = desiredVelocity;
            desiredVelocity = Vector3.zero;

            //remove push force (direction * drag * delta)
            RemovePushForce();
            CurrentPushForce = newPushForce;
        }

        #region protected API

        protected virtual void CalculateVelocity()
        {
            //input + push
            calculatedVelocity = desiredVelocity + CurrentPushForce;

            //clamp at max speed
            if (maxSpeed >= 0)
                calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, maxSpeed);
        }

        protected virtual void DoMovement()
        {
            if (rb)
                rb.linearVelocity = calculatedVelocity;
        }

        protected virtual void RemovePushForce()
        {
            //remove push force (direction * drag * delta)
            newPushForce = CurrentPushForce - (
                (dragBasedOnVelocity ? CurrentPushForce : CurrentPushForce.normalized) *
                Drag * Time.fixedDeltaTime);

            //clamp it
            if (CurrentPushForce.x >= 0 && newPushForce.x < 0 || CurrentPushForce.x <= 0 && newPushForce.x > 0)
                newPushForce.x = 0;
            if (CurrentPushForce.y >= 0 && newPushForce.y < 0 || CurrentPushForce.y <= 0 && newPushForce.y > 0)
                newPushForce.y = 0;
            if (CurrentPushForce.z >= 0 && newPushForce.z < 0 || CurrentPushForce.z <= 0 && newPushForce.z > 0)
                newPushForce.z = 0;
        }

        #endregion

        #region public API

        /// <summary>
        /// Set movement in direction
        /// </summary>
        /// <param name="direction"></param>
        public void MoveInDirection(Vector3 direction)
        {
            //save last input direction + set movement
            MoveDirectionInput = direction.normalized;
            desiredVelocity = MoveDirectionInput * inputSpeed;
        }

        /// <summary>
        /// Set movement direction to position
        /// </summary>
        /// <param name="positionToReach"></param>
        public void MoveTo(Vector3 positionToReach)
        {
            //save last input direction + set movement
            MoveDirectionInput = (positionToReach - transform.position).normalized;
            desiredVelocity = MoveDirectionInput * inputSpeed;
        }

        /// <summary>
        /// Use X as right and Y as forward
        /// </summary>
        /// <param name="input"></param>
        /// <param name="castToLocal">If false, use global direction Vector3(x, 0, y). If true, rotate input to use X as transform.right and Y as transform.forward</param>
        public void MoveByInput3D(Vector2 input, bool castToLocal = true)
        {
            MoveDirectionInput = castToLocal ? transform.TransformDirection(input.x, 0, input.y) : new Vector3(input.x, 0, input.y);
            MoveDirectionInput = MoveDirectionInput.normalized;
            desiredVelocity = MoveDirectionInput * inputSpeed;
        }

        /// <summary>
        /// Set movement in direction
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="customSpeed"></param>
        public void MoveInDirection(Vector3 direction, float customSpeed)
        {
            //save last input direction + set movement
            MoveDirectionInput = direction.normalized;
            desiredVelocity = MoveDirectionInput * customSpeed;
        }

        /// <summary>
        /// Set movement direction to position
        /// </summary>
        /// <param name="positionToReach"></param>
        /// <param name="customSpeed"></param>
        public void MoveTo(Vector3 positionToReach, float customSpeed)
        {
            //save last input direction + set movement
            MoveDirectionInput = (positionToReach - transform.position).normalized;
            desiredVelocity = MoveDirectionInput * customSpeed;
        }

        /// <summary>
        /// Use X as right and Y as forward
        /// </summary>
        /// <param name="input"></param>
        /// <param name="customSpeed"></param>
        /// <param name="castToLocal">If false, use global direction Vector3(x, 0, y). If true, rotate input to use X as transform.right and Y as transform.forward</param>
        public void MoveByInput3D(Vector2 input, float customSpeed, bool castToLocal = true)
        {
            MoveDirectionInput = castToLocal ? transform.TransformDirection(input.x, 0, input.y) : new Vector3(input.x, 0, input.y);
            MoveDirectionInput = MoveDirectionInput.normalized;
            desiredVelocity = MoveDirectionInput * customSpeed;
        }

        /// <summary>
        /// Push in direction
        /// </summary>
        /// <param name="pushDirection"></param>
        /// <param name="pushForce"></param>
        /// <param name="resetPreviousPush"></param>
        public virtual void PushInDirection(Vector3 pushDirection, float pushForce, bool resetPreviousPush = false)
        {
            //reset previous push or add new one to it
            if (resetPreviousPush)
                CurrentPushForce = pushDirection.normalized * pushForce;
            else
                CurrentPushForce += pushDirection.normalized * pushForce;
        }

        /// <summary>
        /// Calculate which position we will have next frame. 
        /// NB that this use current push and speed, doesn't know if this frame will receive some push or other, so it's not 100% correct
        /// </summary>
        /// <returns></returns>
        public Vector3 CalculateNextPosition()
        {
            CalculateVelocity();
            return transform.position + calculatedVelocity * Time.fixedDeltaTime;
        }

        #endregion
    }
}