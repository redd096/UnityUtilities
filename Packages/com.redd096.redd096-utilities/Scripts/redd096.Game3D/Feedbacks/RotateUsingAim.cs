using redd096.Attributes;
using UnityEngine;

namespace redd096.Game3D
{
    [AddComponentMenu("redd096/.Game3D/Feedbacks/Rotate Using Aim")]
    public class RotateUsingAim : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] AimComponentTopDown aimComponent = default;
        [SerializeField] bool useRigidbody = true;
        [EnableIf("useRigidbody")][SerializeField] Rigidbody rigidBodyToRotate;
        [DisableIf("useRigidbody")][SerializeField] Transform characterToRotate;
        [SerializeField] float rotationSpeed = 20;

        protected virtual void Awake()
        {
            //be sure to have a components
            if (aimComponent == null)
            {
                aimComponent = GetComponentInParent<AimComponentTopDown>();
                if (aimComponent == null) Debug.LogError("Miss AimComponentTopDown on " + name);
            }
            if (useRigidbody && rigidBodyToRotate == null && TryGetComponent(out rigidBodyToRotate) == false)
                Debug.LogError("Miss Rigidbody on " + name);
            if (useRigidbody == false && characterToRotate == null)
                characterToRotate = transform;
        }

        protected virtual void FixedUpdate()
        {
            if (useRigidbody)
                Rotate();
        }

        protected virtual void Update()
        {
            if (useRigidbody == false)
                Rotate();
        }

        protected virtual void Rotate()
        {
            Quaternion lookRotation = Quaternion.LookRotation(aimComponent.AimDirectionInput, Vector3.up);

            if (useRigidbody)
                rigidBodyToRotate.rotation = Quaternion.Lerp(rigidBodyToRotate.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}