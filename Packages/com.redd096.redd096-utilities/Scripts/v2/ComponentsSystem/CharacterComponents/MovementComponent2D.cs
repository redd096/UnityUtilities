using redd096.Attributes;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This component uses FixedUpdate to set rigidbody.velocity by InputSpeed + PushForce. Everything works with Vector2
    /// </summary>
    [System.Serializable]
    public class MovementComponent2D : ICharacterComponent
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] protected Rigidbody2D rb = default;
        [Tooltip("Speed when call Move functions")][SerializeField] protected float inputSpeed = 5;
        [Tooltip("Max Speed, calculating velocity by input + push (-1 = no limit)")][SerializeField] protected float maxSpeed = 50;

        [Header("When pushed")]
        [Tooltip("Drag based on velocity * drag or normalized velocity * drag?")][SerializeField] protected bool dragBasedOnVelocity = true;
        [Tooltip("Use rigidbody drag or custom drag?")][SerializeField] protected bool useCustomDrag = false;
        [EnableIf("useCustomDrag")][SerializeField] protected float customDrag = 5;

        public ICharacter Owner { get; set; }

        public bool IsMovingRight { get; set; }                 //check if moving right
        public Vector2 MoveDirectionInput { get; set; }         //when moves, set it with only input direction (used to know last movement direction)
        public Vector2 LastDesiredVelocity { get; set; }        //when moves, set it as input direction * speed
        public Vector2 CurrentPushForce { get; set; }           //used to push this object (push by recoil, knockback, dash, etc...), will be decreased by drag in every frame
        public Vector2 CurrentVelocity => rb ? rb.velocity : Vector2.zero;
        public float CurrentSpeed => rb ? rb.velocity.magnitude : 0;
        public float InputSpeed { get => inputSpeed; set => inputSpeed = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        /// <summary>
        /// If using custom drag, get and set customDrag. Else get and set rigidbody.drag
        /// </summary>
        public float Drag
        {
            get => useCustomDrag ? customDrag : (rb ? rb.drag : 1);
            set { if (useCustomDrag) customDrag = value; else if (rb) rb.drag = value; }
        }
        /// <summary>
        /// Return IsMovingRight as a direction. Can also set it passing a Vector2 with X greater or lower than 0
        /// </summary>
        public Vector2 IsMovingRightDirection
        {
            get => IsMovingRight ? Vector2.right : Vector2.left;
            set { if (Mathf.Approximately(value.x, 0) == false) IsMovingRight = value.x > 0; }
        }

        //events
        public System.Action<bool> onChangeMovementDirection;

        //private
        protected Vector2 desiredVelocity;                //when moves, set it as input direction * speed (used to move this object, will be reset in every frame)
        protected Vector2 calculatedVelocity;             //desiredVelocity + DesiredPushForce
        protected Vector2 newPushForce;                   //new push force when Drag

        public virtual void Awake()
        {
            //be sure to have components
            if (rb == null && Owner.transform.TryGetComponent(out rb) == false)
                Debug.LogError("Miss Rigidbody on " + GetType().Name);
        }

        public virtual void FixedUpdate()
        {
            //set velocity (input + push)
            CalculateVelocity();
            CheckIsMovingRight();
            DoMovement();

            //reset movement input (cause if nobody call an update, this object must to be still in next frame), but save in LastDesiredVelocity (to read in inspector or if some script need it)
            LastDesiredVelocity = desiredVelocity;
            desiredVelocity = Vector2.zero;

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
                calculatedVelocity = Vector2.ClampMagnitude(calculatedVelocity, maxSpeed);
        }

        protected virtual void CheckIsMovingRight()
        {
            //set previous direction (necessary in case this object stay still)
            bool newMovingRight = IsMovingRight;

            //check if change direction
            if (IsMovingRight && calculatedVelocity.x < 0)
                newMovingRight = false;
            else if (IsMovingRight == false && calculatedVelocity.x > 0)
                newMovingRight = true;

            //check change direction
            if (IsMovingRight != newMovingRight)
            {
                IsMovingRight = newMovingRight;

                //call event
                onChangeMovementDirection?.Invoke(IsMovingRight);
            }
        }

        protected virtual void DoMovement()
        {
            if (rb)
                rb.velocity = calculatedVelocity;
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
            //if (CurrentPushForce.z >= 0 && newPushForce.z < 0 || CurrentPushForce.z <= 0 && newPushForce.z > 0)
            //    newPushForce.z = 0;
        }

        #endregion

        #region public API

        /// <summary>
        /// Set movement in direction
        /// </summary>
        /// <param name="direction"></param>
        public void MoveInDirection(Vector2 direction)
        {
            //save last input direction + set movement
            MoveDirectionInput = direction.normalized;
            desiredVelocity = MoveDirectionInput * inputSpeed;
        }

        /// <summary>
        /// Set movement direction to position
        /// </summary>
        /// <param name="positionToReach"></param>
        public void MoveTo(Vector2 positionToReach)
        {
            //save last input direction + set movement
            MoveDirectionInput = (positionToReach - (Vector2)Owner.transform.position).normalized;
            desiredVelocity = MoveDirectionInput * inputSpeed;
        }

        /// <summary>
        /// Set movement in direction using custom speed
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="customSpeed"></param>
        public void MoveInDirection(Vector2 direction, float customSpeed)
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
        public void MoveTo(Vector2 positionToReach, float customSpeed)
        {
            //save last input direction + set movement
            MoveDirectionInput = (positionToReach - (Vector2)Owner.transform.position).normalized;
            desiredVelocity = MoveDirectionInput * customSpeed;
        }

        /// <summary>
        /// Push in direction
        /// </summary>
        /// <param name="pushDirection"></param>
        /// <param name="pushForce"></param>
        /// <param name="resetPreviousPush"></param>
        public void PushInDirection(Vector2 pushDirection, float pushForce, bool resetPreviousPush = false)
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
        public Vector2 CalculateNextPosition()
        {
            CalculateVelocity();
            return (Vector2)Owner.transform.position + calculatedVelocity * Time.fixedDeltaTime;
        }

        #endregion
    }
}