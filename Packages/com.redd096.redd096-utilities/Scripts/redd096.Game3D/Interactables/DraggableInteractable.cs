using redd096.Attributes;
using System.Collections;
using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Script to interact and start drag objects in scene
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/Interactables/Draggable Interactable")]
    public class DraggableInteractable : MonoBehaviour, IInteractable
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] Rigidbody rb;
        [Tooltip("When stop drag: press to toggle drag or keep pressed?")][SerializeField] EDismissType dismissType = EDismissType.InteractAgain;
        [Tooltip("When dragged, save offset on interact or use fixed offset setted in inspector")][SerializeField] bool useFixedOffset = true;
        [EnableIf("useFixedOffset")][Tooltip("When dragged, this is the offset to keep from interactor")][SerializeField] Vector3 offsetFromInteractor = Vector3.forward * 2;
        [SerializeField] EDraggableConstraints draggableConstraints = EDraggableConstraints.None;
        [EnableIf("draggableConstraints", EDraggableConstraints.EditRigidbodyConstraintsOnDrag)][EnumFlags][SerializeField] RigidbodyConstraints constraintsOnDrag;
        [SerializeField] float lerpMovement = 10f;

        //call OnDismiss when release interact button, press again interact or other
        public EDismissType DismissType => dismissType;

        //events
        public System.Action onInteract;
        public System.Action onDismiss;

        public Rigidbody Rb => rb;

        InteractComponent interactor;
        Transform camTransform;
        Vector3 offset;

        //previous rigidbody values
        bool previousUseGravity;
        RigidbodyConstraints previousConstraints;

        protected virtual void Awake()
        {
            //be sure to have a components
            if (rb == null && TryGetComponent(out rb) == false)
                Debug.LogError("Miss Rigidbody on " + name);
        }

        protected virtual void OnEnable()
        {
            //when disabled, coroutine are stopped. So restart movement coroutine if necessary
            if (camTransform)
                StartCoroutine(MovementCoroutine());
        }

        protected virtual IEnumerator MovementCoroutine()
        {
            //follow interactor
            while (camTransform)
            {
                Vector3 grabOffsetRotated = camTransform.position + camTransform.TransformDirection(offset);
                rb.MovePosition(Vector3.Lerp(rb.position, grabOffsetRotated, Time.fixedDeltaTime * lerpMovement));

                //rotate if necessary
                if (draggableConstraints == EDraggableConstraints.AlwaysLookInUserDirection && interactor)
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

            //save previous rigidbody values - and save offset or use fixed offset
            previousUseGravity = rb.useGravity;
            previousConstraints = rb.constraints;
            offset = useFixedOffset ? offsetFromInteractor : camTransform.InverseTransformDirection(rb.position - camTransform.position);

            //edit rigidbody values
            rb.useGravity = false;
            if (draggableConstraints == EDraggableConstraints.EditRigidbodyConstraintsOnDrag) rb.constraints = constraintsOnDrag;

            //start movement coroutine
            StartCoroutine(MovementCoroutine());

            onInteract?.Invoke();

            return true;
        }

        public virtual bool OnDismiss(InteractComponent interactor, params object[] args)
        {
            this.interactor = null;
            camTransform = null;

            //reset rigidbody
            rb.useGravity = previousUseGravity;
            if (draggableConstraints == EDraggableConstraints.EditRigidbodyConstraintsOnDrag) rb.constraints = previousConstraints;

            onDismiss?.Invoke();

            return true;
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