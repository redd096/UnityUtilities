using redd096.Attributes;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This component uses FixedUpdate to set rigidbody.velocity by InputSpeed + PushForce. 
    /// It has also a check on Y axis, to keep rigidbody gravity and prevent sliding on a slope (tested only on 3d games)
    /// </summary>
    [System.Serializable]
    public class MovementComponentRigidbody : IComponentRD
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [Tooltip("Speed when call Move functions")][SerializeField] protected float inputSpeed = 5;
        [Tooltip("Max Speed, calculating velocity by input + push (-1 = no limit)")][SerializeField] protected float maxSpeed = 50;
        [SerializeField] protected FRigidbodyWrapper rb;
        [Tooltip("Tested only in 3d. With rigidbody 2d you want to add a check on Y axis to keep rigidbody gravity and prevent sliding on a slope?")][SerializeField] protected bool useAlsoIn2dGravityAndPreventSlide = false;
        [Tooltip("Prevent sliding on this angle slope")][SerializeField] protected float maxSlopeAngle = 45;

        [Header("When pushed")]
        [Tooltip("Drag based on velocity * drag or normalized velocity * drag?")][SerializeField] protected bool dragBasedOnVelocity = true;
        [Tooltip("Use rigidbody drag or custom drag?")][SerializeField] protected bool useCustomDrag = false;
        [EnableIf("useCustomDrag")][SerializeField] protected float customDrag = 5;

        public IGameObjectRD Owner { get; set; }

        public bool IsMovingRight { get; set; }                     //check if moving right (this is used in 2d games where you can look only left or right)
        public Vector3 MoveDirectionInput { get; set; }             //when moves, set it with only input direction (used to know last movement direction)
        public Vector3 LastDesiredVelocity { get; private set; }    //when moves, set it as input direction * speed
        public Vector3 CurrentPushForce { get; set; }               //used to push this object (push by recoil, knockback, dash, etc...), will be decreased by drag in every frame
        public Vector3 CurrentVelocity => rb.IsValid() ? rb.velocity : Vector3.zero;
        public float CurrentSpeed => rb.IsValid() ? rb.velocity.magnitude : 0;
        public float InputSpeed { get => inputSpeed; set => inputSpeed = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        /// <summary>
        /// If using custom drag, get and set customDrag. Else get and set rigidbody.drag
        /// </summary>
        public float Drag
        {
            get => useCustomDrag ? customDrag : (rb.IsValid() ? rb.drag : 1);
            set { if (useCustomDrag) customDrag = value; else if (rb.IsValid()) rb.drag = value; }
        }
        /// <summary>
        /// Return IsMovingRight as a direction. Can also set it passing a Vector3 with X greater or lower than 0
        /// </summary>
        public Vector3 IsMovingRightDirection
        {
            get => IsMovingRight ? Vector3.right : Vector3.left;
            set { if (Mathf.Approximately(value.x, 0) == false) IsMovingRight = value.x > 0; }
        }

        //events
        public System.Action<bool> onChangeMovementDirection;

        //private
        protected Vector3 desiredVelocity;          //when moves, set it as input direction * speed (used to move this object, will be reset in every frame)
        protected Vector3 calculatedVelocity;       //desiredVelocity + DesiredPushForce
        protected Vector3 newPushForce;             //new push force when Drag

        public virtual void AwakeRD()
        {
            //be sure to have components
            if (rb.IsValid() == false && rb.TryGetComponent(Owner.transform) == false)
                Debug.LogError("Miss Rigidbody on " + GetType().Name, Owner.transform.gameObject);
        }

        /// <summary>
        /// It's better to call this in FixedUpdate because rigidbody simulation happens in FixedUpdate
        /// </summary>
        public virtual void UpdatePosition()
        {
            //set velocity (input + push)
            CalculateVelocity();
            CheckIsMovingRight();
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

            //add gravity to calculated velocity
            if (rb.use3d || useAlsoIn2dGravityAndPreventSlide)
                ApplyGravity();

            //clamp at max speed
            if (maxSpeed >= 0)
                calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, maxSpeed);
        }

        protected virtual void ApplyGravity()
        {
            //if falling, keep Y (rigidbody gravity)
            if (rb.IsValid() && rb.velocity.y < 0)
                calculatedVelocity.y += rb.velocity.y;
        }

        protected virtual void CheckIsMovingRight()
        {
            //set previous direction (necessary in case this object stay still)
            bool newRight = IsMovingRight;

            //update new direction
            if (IsMovingRight && calculatedVelocity.x < 0)
                newRight = false;
            else if (IsMovingRight == false && calculatedVelocity.x > 0)
                newRight = true;

            //check change direction
            if (IsMovingRight != newRight)
            {
                IsMovingRight = newRight;

                //call event
                onChangeMovementDirection?.Invoke(IsMovingRight);
            }
        }

        protected virtual void DoMovement()
        {
            //set velocity
            if (rb.IsValid())
            {
                rb.velocity = calculatedVelocity;

                //prevent sliding on a slope
                if (rb.use3d || useAlsoIn2dGravityAndPreventSlide)
                    PreventSliding();
            }
        }

        protected virtual void PreventSliding()
        {
            //prevent sliding when on a slope
            if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, 3f))
            {
                Vector3 surfaceNormal = hit.normal;
                float slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);

                if (slopeAngle < maxSlopeAngle)
                {
                    Vector3 gravity = Physics.gravity;
                    Vector3 slopeParallelGravity = Vector3.ProjectOnPlane(gravity, surfaceNormal);
                    rb.AddForce(-slopeParallelGravity, ForceMode.Acceleration, ForceMode2D.Force);
                }
            }
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
            MoveDirectionInput = (positionToReach - Owner.transform.position).normalized;
            desiredVelocity = MoveDirectionInput * inputSpeed;
        }

        /// <summary>
        /// Set movement in direction, but use X as right and Y as forward
        /// </summary>
        /// <param name="input"></param>
        /// <param name="castToLocal">If false, use global direction Vector3(x, 0, y). If true, rotate input to use X as transform.right and Y as transform.forward</param>
        public void MoveByInput3D(Vector2 input, bool castToLocal = true)
        {
            MoveDirectionInput = castToLocal ? Owner.transform.TransformDirection(input.x, 0, input.y) : new Vector3(input.x, 0, input.y);
            MoveDirectionInput = MoveDirectionInput.normalized;
            desiredVelocity = MoveDirectionInput * inputSpeed;
        }

        /// <summary>
        /// Set movement in direction using custom speed
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
        /// Set movement to position using custom speed
        /// </summary>
        /// <param name="positionToReach"></param>
        /// <param name="customSpeed"></param>
        public void MoveTo(Vector3 positionToReach, float customSpeed)
        {
            //save last input direction + set movement
            MoveDirectionInput = (positionToReach - Owner.transform.position).normalized;
            desiredVelocity = MoveDirectionInput * customSpeed;
        }

        /// <summary>
        /// Set movement in direction, but use X as right and Y as forward
        /// </summary>
        /// <param name="input"></param>
        /// <param name="castToLocal">If false, use global direction Vector3(x, 0, y). If true, rotate input to use X as transform.right and Y as transform.forward</param>
        public void MoveByInput3D(Vector2 input, float customSpeed, bool castToLocal = true)
        {
            MoveDirectionInput = castToLocal ? Owner.transform.TransformDirection(input.x, 0, input.y) : new Vector3(input.x, 0, input.y);
            MoveDirectionInput = MoveDirectionInput.normalized;
            desiredVelocity = MoveDirectionInput * customSpeed;
        }

        /// <summary>
        /// Push in direction
        /// </summary>
        /// <param name="pushDirection"></param>
        /// <param name="pushForce"></param>
        /// <param name="resetPreviousPush"></param>
        public void PushInDirection(Vector3 pushDirection, float pushForce, bool resetPreviousPush = false)
        {
            //reset previous push or add new one to it
            if (resetPreviousPush)
                CurrentPushForce = pushDirection.normalized * pushForce;
            else
                CurrentPushForce += pushDirection.normalized * pushForce;
        }

        /// <summary>
        /// Push in direction
        /// </summary>
        /// <param name="force"></param>
        /// <param name="resetPreviousPush"></param>
        public void PushInDirection(Vector3 force, bool resetPreviousPush = false)
        {
            //reset previous push or add new one to it
            if (resetPreviousPush)
                CurrentPushForce = force;
            else
                CurrentPushForce += force;
        }

        /// <summary>
        /// Calculate which position we will have next frame. 
        /// NB that this use current push and speed, doesn't know if this frame will receive some push or other, so it's not 100% correct
        /// </summary>
        /// <returns></returns>
        public Vector3 CalculateNextPosition()
        {
            CalculateVelocity();
            return Owner.transform.position + calculatedVelocity * Time.fixedDeltaTime;
        }

        #endregion
    }

    #region rigidbody wrapper

    [System.Serializable]
    public struct FRigidbodyWrapper
    {
        [Tooltip("Use rigidbody 3d or 2d")] public bool use3d;
        public Rigidbody rb3d;
        public Rigidbody2D rb2d;

        /// <summary>
        /// Is rigidbody != null 
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (use3d)
                return rb3d != null;
            else
                return rb2d != null;
        }

        /// <summary>
        /// Try get rigidbody on transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public bool TryGetComponent(Transform transform)
        {
            if (use3d)
                return transform.TryGetComponent(out rb3d);
            else
                return transform.TryGetComponent(out rb2d);
        }

        /// <summary>
        /// Apply a force to the rigidbody
        /// </summary>
        /// <param name="force"></param>
        /// <param name="mode3D"></param>
        /// <param name="mode2D"></param>
        public void AddForce(Vector3 force, ForceMode mode3D, ForceMode2D mode2D)
        {
            if (use3d)
                rb3d.AddForce(force, mode3D);
            else
                rb2d.AddForce(force, mode2D);
        }

        /// <summary>
        /// The velocity vector of the rigidbody. It represents the rate of change of Rigidbody position.
        /// </summary>
        public Vector3 velocity
        {
            get
            {
                if (use3d)
                    return rb3d.velocity;
                else
                    return rb2d.velocity;
            }
            set
            {
                if (use3d)
                    rb3d.velocity = value;
                else
                    rb2d.velocity = value;
            }
        }

        /// <summary>
        /// The drag of the rigidbody
        /// </summary>
        public float drag
        {
            get
            {
                if (use3d)
                    return rb3d.drag;
                else
                    return rb2d.drag;
            }
            set
            {
                if (use3d)
                    rb3d.drag = value;
                else
                    rb2d.drag = value;
            }
        }

        /// <summary>
        /// The position of the rigidbody
        /// </summary>
        public Vector3 position
        {
            get
            {
                if (use3d)
                    return rb3d.position;
                else 
                    return rb2d.position;
            }
            set
            {
                if (use3d)
                    rb3d.position = value;
                else
                    rb2d.position = value;
            }
        }
    }

    #endregion
}