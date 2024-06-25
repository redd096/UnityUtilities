using redd096.Attributes;
using System.Collections;
using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Script to interact and start drag objects in scene, by using a Joint
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/Interactables/Draggable Joint Interactable")]
    public class DraggableJointInteractable : MonoBehaviour
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] Rigidbody rb;
        [Tooltip("If using something like doors, set to false. If dragging a pen that follows player, set to true")][SerializeField] bool canDragAround = false;
        [Tooltip("When stop drag: press to toggle drag or keep pressed?")][SerializeField] EDismissType dismissType = EDismissType.ReleaseInteractInput;
        [Tooltip("If user moves too much away from the dragged object, destroy the joint")][SerializeField] float breakJointRange = 2;
        [SerializeField] EDraggableConstraints draggableConstraints = EDraggableConstraints.None;
        [EnableIf("draggableConstraints", EDraggableConstraints.EditRigidbodyConstraintsOnDrag)][EnumFlags][SerializeField] RigidbodyConstraints constraintsOnDrag;
        [Tooltip("If true, when drag set Interpolate and ContinuousDynamic. Reset when stop drag")][SerializeField] bool setInterpolationAndCollisionDetectionMode = true;
        [Tooltip("If when release the object, it's moving too much, clamp its velocity")][SerializeField] float clampVelocityOnRelease = 10;

        //call OnDismiss when release interact button, press again interact or other
        public EDismissType DismissType => dismissType;

        //events
        public System.Action onInteract;
        public System.Action onDismiss;
        public System.Action onTooMuchDistance;

        public Rigidbody Rb => rb;
        public Transform Handle => handle;
        public Rigidbody HandRb => handRb;

        InteractComponent interactor;
        Transform camTransform;

        //for grab - instantiate and move an hand with a joint to the grabbed object
        Transform handle;   //used to check the distance to break the joint
        Rigidbody handRb;

        //previous rigidbody values
        bool previousUseGravity;
        RigidbodyConstraints previousConstraints;
        RigidbodyInterpolation previousInterpolation;
        CollisionDetectionMode previousCollisionDetectionMode;

        protected virtual void Awake()
        {
            //be sure to have a components
            if (rb == null && TryGetComponent(out rb) == false)
                Debug.LogError("Miss Rigidbody on " + name);
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            //Gizmos.color = Color.red - new Color(0, 0, 0, 0.7f);
            //if (handle)
            //    Gizmos.DrawSphere(handle.position, 0.1f);
            //Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.7f);
            //if (handRb)
            //    Gizmos.DrawSphere(handRb.position, 0.1f);
            //Gizmos.color = Color.white;
        }
#endif

        protected virtual void OnEnable()
        {
            //when disabled, coroutine are stopped. So restart movement coroutine if necessary
            if (handRb)
                StartCoroutine(MovementCoroutine());
        }

        protected virtual IEnumerator MovementCoroutine()
        {
            while (handRb != null && handle != null)
            {
                //if too much distance, destroy joint
                if (Vector3.Distance(handRb.transform.position, handle.transform.position) > breakJointRange)
                {
                    DestroyJoint();
                    onTooMuchDistance?.Invoke();
                    interactor.DismissWithCurrentInteractable();
                    break;
                }

                //move hand
                //bool hitSomething = Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, breakJointRange);
                //handRb.transform.position = hitSomething ? hit.point : camTransform.position + camTransform.forward * breakJointRange;

                //rotate if necessary
                if (draggableConstraints == EDraggableConstraints.AlwaysLookInUserDirection)
                {
                    rb.MoveRotation(interactor.transform.rotation);
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public virtual bool OnInteract(InteractComponent interactor, RaycastHit hit, params object[] args)
        {
            this.interactor = interactor;
            camTransform = args[0] as Transform;

            //save previous rigidbody values
            previousUseGravity = rb.useGravity;
            previousConstraints = rb.constraints;
            previousInterpolation = rb.interpolation;
            previousCollisionDetectionMode = rb.collisionDetectionMode;

            //edit rigidbody values
            rb.useGravity = false;
            if (draggableConstraints == EDraggableConstraints.EditRigidbodyConstraintsOnDrag) rb.constraints = constraintsOnDrag;
            if (setInterpolationAndCollisionDetectionMode) rb.interpolation = RigidbodyInterpolation.Interpolate;
            if (setInterpolationAndCollisionDetectionMode) rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            //create joint
            CreateJoint(hit.point);

            //start coroutine
            StartCoroutine(MovementCoroutine());

            onInteract?.Invoke();

            return true;
        }

        public virtual bool OnDismiss(InteractComponent interactor, params object[] args)
        {
            this.interactor = null;
            camTransform = null;

            //be sure to stop coroutine
            StopCoroutine(nameof(MovementCoroutine));

            //reset rigidbody
            rb.useGravity = previousUseGravity;
            if (draggableConstraints == EDraggableConstraints.EditRigidbodyConstraintsOnDrag) rb.constraints = previousConstraints;
            if (setInterpolationAndCollisionDetectionMode) rb.interpolation = previousInterpolation;
            if (setInterpolationAndCollisionDetectionMode) rb.collisionDetectionMode = previousCollisionDetectionMode;

            //destroy joint
            DestroyJoint();

            onDismiss?.Invoke();

            return true;
        }

        protected virtual void CreateJoint(Vector3 hitPoint)
        {
            //remove if has already joint
            DestroyJoint();

            //create handle, child of the object to drag
            handle = new GameObject("Handle").transform;
            handle.transform.SetParent(rb.transform);
            handle.position = hitPoint;

            //create hand
            handRb = new GameObject("Hand", typeof(Rigidbody)).GetComponent<Rigidbody>();
            handRb.isKinematic = true;
            handRb.transform.SetParent(camTransform);
            handRb.transform.position = handle.position;

            //create joint - old
            //SpringJoint joint = handRb.gameObject.AddComponent<SpringJoint>();
            //joint.spring = 50;
            //joint.damper = 5;
            //joint.maxDistance = 0.2f;
            //joint.connectedBody = rb;
            //joint.anchor = Vector3.zero;

            //create joint
            ConfigurableJoint joint = handRb.gameObject.AddComponent<ConfigurableJoint>();
            JointDrive drive = new()
            {
                positionSpring = 80f,
                positionDamper = 32f,
                maximumForce = Mathf.Infinity
            };
            joint.xDrive = drive;
            joint.yDrive = drive;
            joint.zDrive = drive;
            joint.connectedBody = rb;
            joint.anchor = Vector3.zero;

            //configure joint to take things that follow player
            if (canDragAround)
            {
                JointDrive angularDrive = new()
                {
                    positionSpring = 100f,
                    positionDamper = 40f,
                    maximumForce = Mathf.Infinity
                };
                joint.angularXDrive = angularDrive;
                joint.angularYZDrive = angularDrive;
                joint.xMotion = ConfigurableJointMotion.Limited;
                joint.yMotion = ConfigurableJointMotion.Limited;
                joint.zMotion = ConfigurableJointMotion.Limited;
                SoftJointLimit limit = new()
                {
                    bounciness = 0,
                    limit = 0.15f
                };
                joint.linearLimit = limit;
            }
        }

        protected virtual void DestroyJoint()
        {
            if (handRb == null)
                return;

            //destroy hand and handle
            Destroy(handle.gameObject);
            Destroy(handRb.gameObject);
            handle = null;
            handRb = null;

            //clamp velocity to not throw items too distant
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, clampVelocityOnRelease);
        }

        /// <summary>
        /// Used to decide how this object will move and rotate when dragged
        /// None - normally, just stop gravity
        /// EditRigidbodyConstraintsOnDrag - set new rigidbody constraints. Reset on release
        /// AlwaysLookInUserDirection - while dragged, always rotate to look in the same direction as interactor
        /// </summary>
        public enum EDraggableConstraints
        {
            None, EditRigidbodyConstraintsOnDrag, AlwaysLookInUserDirection
        }
    }
}