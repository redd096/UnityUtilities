using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Call UpdatePosition() in Update to move CharacterController by InputSpeed + PushForce. 
    /// Or call it in FixedUpdate to set rigidbody.velocity by InputSpeed + PushForce. 
    /// With rigidbody it has also a check on Y axis, to keep rigidbody gravity and prevent sliding on a slope (tested only on 3d games)
    /// </summary>
    [System.Serializable]
    public class SimpleMovementComponent : IComponentRD
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] protected FComponentWrapper wrap;
        [Tooltip("Speed when call Move functions")][SerializeField] protected float inputSpeed = 5;
        [Tooltip("Max Speed, calculating velocity by input + push (-1 = no limit)")][SerializeField] protected float maxSpeed = 50;
        [Tooltip("Tested only in 3d. With rigidbody you want to add a check on Y axis to keep rigidbody gravity and prevent sliding on a slope?")][SerializeField] protected bool rigidbodyUseGravityAndPreventSlide = false;
        [Tooltip("With rigidbody, prevent sliding on this angle slope - with CharacterController is setted on CharacterControlelr component")][SerializeField] protected float rigidbodyMaxSlopeAngle = 45;

        [Header("When pushed")]
        [Tooltip("Drag based on velocity * drag or normalized velocity * drag?")][SerializeField] protected bool dragBasedOnVelocity = true;
        [Tooltip("Use rigidbody drag or custom drag? CharacterController use always custom drag")][SerializeField] protected bool rigidbodyUseCustomDrag = false;
        [Tooltip("CharacterController use this drag. You can use it also on Rigidbody instead of Rigidbody.drag")][SerializeField] protected float customDrag = 5;

        public IGameObjectRD Owner { get; set; }

        public bool IsMovingRight { get; set; }                     //check if moving right (this is used in 2d games where you can look only left or right)
        public Vector3 MoveDirectionInput { get; set; }             //when moves, set it with only input direction (used to know last movement direction)
        public Vector3 LastDesiredVelocity { get; private set; }    //when moves, set it as input direction * speed
        public Vector3 CurrentPushForce { get; set; }               //used to push this object (push by recoil, knockback, dash, etc...), will be decreased by drag in every frame
        public Vector3 CurrentVelocity => wrap.IsValid() ? wrap.velocity : Vector3.zero;
        public float CurrentSpeed => wrap.IsValid() ? wrap.velocity.magnitude : 0;
        public float InputSpeed { get => inputSpeed; set => inputSpeed = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        /// <summary>
        /// If using CharacterController or is enabled custom drag, get and set customDrag. Else get and set rigidbody.drag
        /// </summary>
        public float Drag
        {
            get
            {
                return wrap.componentToWrap == FComponentWrapper.EComponentToWrap.CharacterController ? customDrag
                : (rigidbodyUseCustomDrag ? customDrag : (wrap.IsValid() ? wrap.drag : 1));
            }
            set
            {
                if (wrap.componentToWrap == FComponentWrapper.EComponentToWrap.CharacterController) { customDrag = value; }
                else { if (rigidbodyUseCustomDrag) customDrag = value; else if (wrap.IsValid()) wrap.drag = value; }
            }
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
        protected float gravity;                    //applied gravity to characterController

        public virtual void AwakeRD()
        {
            //be sure to have components
            if (wrap.IsValid() == false && wrap.TryGetComponent(Owner.transform) == false)
                Debug.LogError("Miss CharacterController or Rigidbody on " + GetType().Name, Owner.transform.gameObject);
        }

        /// <summary>
        /// If you are using a CharacterController, call this in Update(). 
        /// If you are using a Rigidbody, call this in FixedUpdate()
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
            if (wrap.componentToWrap == FComponentWrapper.EComponentToWrap.CharacterController || rigidbodyUseGravityAndPreventSlide)
                ApplyGravity();

            //clamp at max speed
            if (maxSpeed >= 0)
                calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, maxSpeed);
        }

        protected virtual void ApplyGravity()
        {
            if (wrap.componentToWrap == FComponentWrapper.EComponentToWrap.CharacterController)
            {
                //if CharacterController is grounded, remove gravity - else, add gravity
                if (wrap.IsValid() && wrap.isGrounded && wrap.velocity.y < 0)
                    gravity = 0f;
                else
                    gravity += Physics.gravity.y * Time.deltaTime;

                calculatedVelocity.y += gravity;
            }
            else
            {
                //if falling, keep Y (rigidbody gravity)
                if (wrap.IsValid() && wrap.velocity.y < 0)
                    calculatedVelocity.y += wrap.velocity.y;
            }
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
            if (wrap.IsValid())
            {
                wrap.Move(calculatedVelocity);

                //prevent sliding on a slope - only for rigidbody (character controller does it by itself)
                if (wrap.componentToWrap != FComponentWrapper.EComponentToWrap.CharacterController && rigidbodyUseGravityAndPreventSlide)
                    PreventSliding();
            }
        }

        protected virtual void PreventSliding()
        {
            //prevent sliding when on a slope
            if (Physics.Raycast(wrap.position, Vector3.down, out RaycastHit hit, 3f))
            {
                Vector3 surfaceNormal = hit.normal;
                float slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);

                if (slopeAngle < rigidbodyMaxSlopeAngle)
                {
                    Vector3 gravity = Physics.gravity;
                    Vector3 slopeParallelGravity = Vector3.ProjectOnPlane(gravity, surfaceNormal);
                    wrap.AddForce(-slopeParallelGravity, ForceMode.Acceleration, ForceMode2D.Force);
                }
            }
        }

        protected virtual void RemovePushForce()
        {
            //remove push force (direction * drag * deltaTime)
            newPushForce = CurrentPushForce - (
                (dragBasedOnVelocity ? CurrentPushForce : CurrentPushForce.normalized) *
                Drag * wrap.deltaTime);

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
        /// Set movement in direction, but use X as right and Y as forward
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rotation">Rotate input to use it from local to world space</param>
        public void MoveByInput3D(Vector2 input, Quaternion rotation)
        {
            MoveDirectionInput = rotation * new Vector3(input.x, 0, input.y);
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
        /// Set movement in direction, but use X as right and Y as forward
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rotation">Rotate input to use it from local to world space</param>
        public void MoveByInput3D(Vector2 input, float customSpeed, Quaternion rotation)
        {
            MoveDirectionInput = rotation * new Vector3(input.x, 0, input.y);
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
            return Owner.transform.position + calculatedVelocity * wrap.deltaTime;
        }

        #endregion
    }

    #region rigidbody wrapper

    [System.Serializable]
    public struct FComponentWrapper
    {
        public enum EComponentToWrap { CharacterController, Rigidbody3D, Rigidbody2D }

        [Tooltip("Use CharacterController, Rigidbody 3d or Rigidbody 2d")] public EComponentToWrap componentToWrap;
        public CharacterController ch;
        public Rigidbody rb3d;
        public Rigidbody2D rb2d;

        /// <summary>
        /// Is component != null 
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (componentToWrap == EComponentToWrap.CharacterController)
                return ch != null;
            else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                return rb3d != null;
            else
                return rb2d != null;
        }

        /// <summary>
        /// Try get CharacterController or rigidbody on transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public bool TryGetComponent(Transform transform)
        {
            if (componentToWrap == EComponentToWrap.CharacterController)
                return transform.TryGetComponent(out ch);
            else if (componentToWrap == EComponentToWrap.Rigidbody3D)
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
            if (componentToWrap == EComponentToWrap.CharacterController)
                return;
            else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                rb3d.AddForce(force, mode3D);
            else
                rb2d.AddForce(force, mode2D);
        }

        /// <summary>
        /// Call CharacterController.Move or set Rigidbody.velocity
        /// </summary>
        /// <param name="velocity"></param>
        public void Move(Vector3 velocity)
        {
            if (componentToWrap == EComponentToWrap.CharacterController)
                ch.Move(velocity * Time.deltaTime);
            else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                rb3d.velocity = velocity;
            else
                rb2d.velocity = velocity;
        }

        /// <summary>
        /// The velocity vector of the component. It represents the rate of change of CharacterController or Rigidbody position.
        /// </summary>
        public Vector3 velocity
        {
            get
            {
                if (componentToWrap == EComponentToWrap.CharacterController)
                    return ch.velocity;
                else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                    return rb3d.velocity;
                else
                    return rb2d.velocity;
            }
        }

        /// <summary>
        /// The drag of the rigidbody
        /// </summary>
        public float drag
        {
            get
            {
                if (componentToWrap == EComponentToWrap.CharacterController)
                    return default;
                else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                    return rb3d.drag;
                else
                    return rb2d.drag;
            }
            set
            {
                if (componentToWrap == EComponentToWrap.CharacterController)
                    return;
                else if (componentToWrap == EComponentToWrap.Rigidbody3D)
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
                if (componentToWrap == EComponentToWrap.CharacterController)
                    return default;
                else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                    return rb3d.position;
                else 
                    return rb2d.position;
            }
        }

        /// <summary>
        /// Was the CharacterController touching the ground during the last move?
        /// </summary>
        public bool isGrounded
        {
            get
            {
                if (componentToWrap == EComponentToWrap.CharacterController)
                    return ch.isGrounded;
                else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                    return false;
                else
                    return false;
            }
        }

        /// <summary>
        /// If using CharacterController, return deltaTime. If using Rigidbody, return fixedDeltaTime
        /// </summary>
        public float deltaTime
        {
            get
            {
                if (componentToWrap == EComponentToWrap.CharacterController)
                    return Time.deltaTime;
                else if (componentToWrap == EComponentToWrap.Rigidbody3D)
                    return Time.fixedDeltaTime;
                else
                    return Time.fixedDeltaTime;
            }
        }
    }

    #endregion
}