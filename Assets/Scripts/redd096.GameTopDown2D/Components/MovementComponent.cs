using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Components/Movement Component")]
    public class MovementComponent : MonoBehaviour
    {
        enum EUpdateModes { Update, FixedUpdate }
        enum EMovementModes { Transform, Rigidbody }

        [Header("Movement")]
        [Tooltip("Move on Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Update;
        [Tooltip("Move using transform or rigidbody? Transform use completely CollisionComponent, rigidbody use unity physics")] [SerializeField] EMovementModes movementMode = EMovementModes.Transform;
        [Tooltip("Max Speed, calculating velocity by input + push (-1 = no limit)")] [SerializeField] float maxSpeed = 50;

        [Header("When pushed")]
        [Tooltip("Drag based on velocity * drag or normalized * drag?")] [SerializeField] bool dragBasedOnVelocity = true;
        [Tooltip("Drag used when pushed from something")] [SerializeField] float drag = 5;
        [Tooltip("When player is pushed for example to the right, and hit a wall. Set Push to right at 0?")] [SerializeField] bool removePushForceWhenHit = true;

        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] CollisionComponent collisionComponent = default;
        [Tooltip("When collide in every direction, can move everywhere")] [SerializeField] bool tempFixCollisionComponent = true;
        [ShowIf("movementMode", EMovementModes.Rigidbody)] [SerializeField] Rigidbody2D rb = default;

        [Header("DEBUG")]
        [ReadOnly] public bool IsMovingRight = true;            //check if moving right
        [ReadOnly] public Vector2 MoveDirectionInput;           //when moves, set it with only input direction (used to know last movement direction)
        [ReadOnly] public Vector2 LastDesiredVelocity;          //when moves, set it as input direction * speed
        [ReadOnly] public Vector2 DesiredPushForce;             //used to push this object (push by recoil, knockback, dash, etc...), will be decreased by drag in every frame
        [ReadOnly] public Vector2 CurrentVelocity;              //velocity calculate for this frame
        [ReadOnly] public float CurrentSpeed;                   //CurrentVelocity.magnitude

        //events
        public System.Action<bool> onChangeMovementDirection { get; set; }

        //private
        Vector2 desiredVelocity;              //when moves, set it as input direction * speed (used to move this object, will be reset in every frame)

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

            //set velocity (input + push + check collisions)
            CurrentVelocity = CalculateVelocity();
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
            DesiredPushForce = CalculateNewPushForce();
        }

        #region private API

        bool CheckComponents()
        {
            //check if have a collision component
            if (collisionComponent == null)
                collisionComponent = GetComponent<CollisionComponent>();

            //if movement mode is rigidbody, be sure to have a rigidbody
            if (movementMode == EMovementModes.Rigidbody && rb == null)
            {
                rb = GetComponent<Rigidbody2D>();

                if (rb == null)
                {
                    Debug.LogWarning("Miss Rigidbody on " + name);
                    return false;
                }
            }

            return true;
        }

        Vector2 CalculateVelocity()
        {
            //input + push
            Vector2 velocity = desiredVelocity + DesiredPushForce;

            if (collisionComponent)
            {
                //be sure fix is not enabled, or is not hitting every 4 directions
                if (tempFixCollisionComponent == false
                    || collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.right) == false || collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.left) == false
                    || collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.up) == false || collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.down) == false)
                {
                    //check if hit horizontal
                    if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.right) && velocity.x > 0)
                        velocity.x = 0;
                    else if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.left) && velocity.x < 0)
                        velocity.x = 0;

                    //check if hit vertical
                    if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.up) && velocity.y > 0)
                        velocity.y = 0;
                    else if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.down) && velocity.y < 0)
                        velocity.y = 0;
                }
            }

            //clamp at max speed
            if (maxSpeed >= 0)
                velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

            return velocity;
        }

        bool CheckIsMovingRight()
        {
            //check if change direction
            if (IsMovingRight && CurrentVelocity.x < 0)
                return false;
            else if (IsMovingRight == false && CurrentVelocity.x > 0)
                return true;

            //else return previous direction (necessary in case this object stay still)
            return IsMovingRight;
        }

        void DoMovement()
        {
            //do movement with rigidbody (let unity calculate reachable position)
            if (movementMode == EMovementModes.Rigidbody)
            {
                rb.velocity = CurrentVelocity;
            }
            //or move with transform (if there is collision component, calculate reachable position, else just move to new position)
            else if (movementMode == EMovementModes.Transform)
            {
                Vector2 newPosition = transform.position + (Vector3)CurrentVelocity * (updateMode == EUpdateModes.Update ? Time.deltaTime : Time.fixedDeltaTime);

                //calculate reachable position
                if (collisionComponent)
                {
                    if (CurrentVelocity.x > 0)
                        newPosition = collisionComponent.CalculateReachablePosition(CollisionComponent.EDirectionEnum.right, newPosition);
                    else if (CurrentVelocity.x < 0)
                        newPosition = collisionComponent.CalculateReachablePosition(CollisionComponent.EDirectionEnum.left, newPosition);

                    if (CurrentVelocity.y > 0)
                        newPosition = collisionComponent.CalculateReachablePosition(CollisionComponent.EDirectionEnum.up, newPosition);
                    else if (CurrentVelocity.y < 0)
                        newPosition = collisionComponent.CalculateReachablePosition(CollisionComponent.EDirectionEnum.down, newPosition);
                }

                transform.position = newPosition;
            }
        }

        Vector2 CalculateNewPushForce()
        {
            //remove push force (direction * drag * delta)
            Vector2 newPushForce = DesiredPushForce - ((dragBasedOnVelocity ? DesiredPushForce : DesiredPushForce.normalized) * drag * (updateMode == EUpdateModes.Update ? Time.deltaTime : Time.fixedDeltaTime));

            //clamp it
            if (DesiredPushForce.x >= 0 && newPushForce.x < 0 || DesiredPushForce.x <= 0 && newPushForce.x > 0)
                newPushForce.x = 0;
            if (DesiredPushForce.y >= 0 && newPushForce.y < 0 || DesiredPushForce.y <= 0 && newPushForce.y > 0)
                newPushForce.y = 0;

            //check if hit walls
            if (collisionComponent && removePushForceWhenHit)
            {
                //check if hit horizontal
                if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.right) && newPushForce.x > 0)
                    newPushForce.x = 0;
                else if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.left) && newPushForce.x < 0)
                    newPushForce.x = 0;

                //check if hit vertical
                if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.up) && newPushForce.y > 0)
                    newPushForce.y = 0;
                else if (collisionComponent.IsHitting(CollisionComponent.EDirectionEnum.down) && newPushForce.y < 0)
                    newPushForce.y = 0;
            }

            return newPushForce;
        }

        #endregion

        #region public API

        /// <summary>
        /// Set movement in direction using custom speed
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="newSpeed"></param>
        public void MoveInDirection(Vector2 direction, float newSpeed)
        {
            //save last input direction + set movement
            MoveDirectionInput = direction.normalized;
            desiredVelocity = MoveDirectionInput * newSpeed;
        }

        /// <summary>
        /// Set movement to position using custom speed
        /// </summary>
        /// <param name="positionToReach"></param>
        /// <param name="newSpeed"></param>
        public void MoveTo(Vector2 positionToReach, float newSpeed)
        {
            //save last input direction + set movement
            MoveDirectionInput = (positionToReach - (Vector2)transform.position).normalized;
            desiredVelocity = MoveDirectionInput * newSpeed;
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
                DesiredPushForce = pushDirection * pushForce;
            else
                DesiredPushForce += pushDirection * pushForce;
        }

        #endregion
    }
}