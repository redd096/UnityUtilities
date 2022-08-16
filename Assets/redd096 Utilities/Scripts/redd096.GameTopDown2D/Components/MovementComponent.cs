using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Movement Component")]
    public class MovementComponent : MonoBehaviour
    {
        enum EUpdateModes { Update, FixedUpdate }
        enum EMovementModes { Transform, Rigidbody }

        [Header("Movement")]
        [Tooltip("Move on Update or FixedUpdate?")][SerializeField] EUpdateModes updateMode = EUpdateModes.FixedUpdate;
        [Tooltip("Move using transform or rigidbody? Transform doesn't see colliders, rigidbody use unity physics")][SerializeField] EMovementModes movementMode = EMovementModes.Rigidbody;
        [Tooltip("Speed when call MoveTo or MoveInDirection")][SerializeField] float inputSpeed = 5;
        [Tooltip("Max Speed, calculating velocity by input + push (-1 = no limit)")][SerializeField] float maxSpeed = 50;

        [Header("When pushed")]
        [Tooltip("Drag based on velocity * drag or normalized velocity * drag?")][SerializeField] bool dragBasedOnVelocity = true;
        [Tooltip("Use rigidbody drag or custom drag?")][SerializeField] bool useCustomDrag = false;
        [Tooltip("Drag used when pushed from something")][EnableIf("useCustomDrag")][SerializeField] float customDrag = 5;

        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] Rigidbody2D rb = default;

        [Header("DEBUG")]
        [ReadOnly] public bool IsMovingRight = true;            //check if moving right
        [ReadOnly] public Vector2 MoveDirectionInput;           //when moves, set it with only input direction (used to know last movement direction)
        [ReadOnly] public Vector2 LastDesiredVelocity;          //when moves, set it as input direction * speed
        [ReadOnly] public Vector2 CurrentPushForce;             //used to push this object (push by recoil, knockback, dash, etc...), will be decreased by drag in every frame
        [ReadOnly] public Vector2 CurrentVelocity;              //calculated velocity for this frame or rigidbody.velocity
        [ReadOnly] public float CurrentSpeed;                   //CurrentVelocity.magnitude or rigidbody.velocity.magnitude
        public float InputSpeed { get => inputSpeed; set => inputSpeed = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        /// <summary>
        /// If using custom drag, get and set customDrag. Else get and set rigidbody.drag
        /// </summary>
        public float Drag { get => useCustomDrag ? customDrag : (rb ? rb.drag : 1);
            set { if (useCustomDrag) customDrag = value; else if (rb) rb.drag = value; } }

        //events
        public System.Action<bool> onChangeMovementDirection { get; set; }

        //private
        Vector2 desiredVelocity;                //when moves, set it as input direction * speed (used to move this object, will be reset in every frame)
        Vector2 calculatedVelocity;             //desiredVelocity + DesiredPushForce
        Vector2 newPushForce;                   //new push force when Drag

        void Update()
        {
            //do only if update mode is Update
            if (updateMode == EUpdateModes.Update)
                Move();
        }

        void FixedUpdate()
        {
            //do only if update mode is FixedUpdate
            if (updateMode == EUpdateModes.FixedUpdate)
                Move();
        }

        void Move()
        {
            //start only if there are all necessary components
            if (CheckComponents() == false)
                return;

            //set velocity (input + push)
            CalculateVelocity();
            CurrentVelocity = movementMode == EMovementModes.Transform ? (calculatedVelocity != Vector2.zero ? calculatedVelocity : Vector2.zero) : (rb ? rb.velocity : Vector2.zero);
            CurrentSpeed = CurrentVelocity != Vector2.zero ? CurrentVelocity.magnitude : 0.0f;

            //set if change movement direction
            if (IsMovingRight != CheckIsMovingRight())
            {
                IsMovingRight = CheckIsMovingRight();

                //call event
                onChangeMovementDirection?.Invoke(IsMovingRight);
            }

            //do movement
            DoMovement();

            //reset movement input (cause if nobody call an update, this object must to be still in next frame), but save in LastDesiredVelocity (to read in inspector or if some script need it)
            LastDesiredVelocity = desiredVelocity;
            desiredVelocity = Vector2.zero;

            //remove push force (direction * drag * delta)
            CalculateNewPushForce();
            CurrentPushForce = newPushForce;
        }

        #region private API

        bool CheckComponents()
        {
            //check if have components
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            //if movement mode is rigidbody, be sure to have a rigidbody
            if (movementMode == EMovementModes.Rigidbody && rb == null)
            {
                Debug.LogWarning("Miss Rigidbody on " + name);
                return false;
            }

            return true;
        }

        void CalculateVelocity()
        {
            //input + push
            calculatedVelocity = desiredVelocity + CurrentPushForce;

            //clamp at max speed
            if (maxSpeed >= 0)
                calculatedVelocity = Vector2.ClampMagnitude(calculatedVelocity, maxSpeed);
        }

        bool CheckIsMovingRight()
        {
            //check if change direction
            if (IsMovingRight && calculatedVelocity.x < 0)
                return false;
            else if (IsMovingRight == false && calculatedVelocity.x > 0)
                return true;

            //else return previous direction (necessary in case this object stay still)
            return IsMovingRight;
        }

        void DoMovement()
        {
            //do movement with rigidbody (let unity calculate reachable position)
            if (movementMode == EMovementModes.Rigidbody)
            {
                rb.velocity = calculatedVelocity;
            }
            //or move with transform
            else if (movementMode == EMovementModes.Transform)
            {
                transform.position += (Vector3)calculatedVelocity *
                    (updateMode == EUpdateModes.Update ? Time.deltaTime : Time.fixedDeltaTime);
            }
        }

        void CalculateNewPushForce()
        {
            //remove push force (direction * drag * delta)
            newPushForce = CurrentPushForce - (
                (dragBasedOnVelocity ? CurrentPushForce : CurrentPushForce.normalized) *
                (useCustomDrag ? customDrag : (rb ? rb.drag : 1)) *
                (updateMode == EUpdateModes.Update ? Time.deltaTime : Time.fixedDeltaTime));

            //clamp it
            if (CurrentPushForce.x >= 0 && newPushForce.x < 0 || CurrentPushForce.x <= 0 && newPushForce.x > 0)
                newPushForce.x = 0;
            if (CurrentPushForce.y >= 0 && newPushForce.y < 0 || CurrentPushForce.y <= 0 && newPushForce.y > 0)
                newPushForce.y = 0;
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
        /// Set movement to position
        /// </summary>
        /// <param name="positionToReach"></param>
        public void MoveTo(Vector2 positionToReach)
        {
            //save last input direction + set movement
            MoveDirectionInput = (positionToReach - (Vector2)transform.position).normalized;
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
            MoveDirectionInput = (positionToReach - (Vector2)transform.position).normalized;
            desiredVelocity = MoveDirectionInput * customSpeed;
        }

        /// <summary>
        /// Push in direction
        /// </summary>
        /// <param name="pushDirection"></param>
        /// <param name="pushForce"></param>
        /// <param name="resetPreviousPush"></param>
        public virtual void PushInDirection(Vector2 pushDirection, float pushForce, bool resetPreviousPush = false)
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

            //do movement with rigidbody (let unity calculate reachable position)
            if (movementMode == EMovementModes.Rigidbody)
            {
                return transform.position + (Vector3)calculatedVelocity * Time.fixedDeltaTime;
            }
            //or move with transform
            else
            {
                return transform.position + (Vector3)calculatedVelocity *
                    (updateMode == EUpdateModes.Update ? Time.deltaTime : Time.fixedDeltaTime);
            }
        }

        #endregion
    }
}