using redd096.Attributes;
using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Return delta position or rotation for this rigidbody
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/Other/Get Delta Position")]
    public class GetDeltaPosition : MonoBehaviour
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] Rigidbody rb;
        [SerializeField] EType type;
        [ShowIf("type", EType.Position)][SerializeField] Vector3 startLocalPosition;
        [ShowIf("type", EType.Position)][SerializeField] Vector3 endLocalPosition;
        [ShowIf("type", EType.Rotation)][SerializeField] Vector3 startLocalRotation;
        [ShowIf("type", EType.Rotation)][SerializeField] Vector3 endLocalRotation;

        [Button] void StampDelta() => Debug.Log(GetDelta());

        public Rigidbody Rb => rb;
        public EType Type => type;

        void Awake()
        {
            //be sure to have a components
            if (rb == null && TryGetComponent(out rb) == false)
                Debug.LogError("Miss Rigidbody on " + name, gameObject);
        }

        /// <summary>
        /// Get delta position or rotation for rigidbody
        /// </summary>
        /// <returns></returns>
        public float GetDelta()
        {
            if (type == EType.Position)
            {
                float total = Vector3.SqrMagnitude(endLocalPosition - startLocalPosition);
                float current = Vector3.SqrMagnitude(rb.transform.localPosition - startLocalPosition);
                //Debug.Log($"total {total} and current {current} result {current / total}");
                return current / total;
            }
            else
            {
                Quaternion start = Quaternion.Euler(startLocalRotation);
                Quaternion end = Quaternion.Euler(endLocalRotation);

                float total = Quaternion.Angle(start, end);
                float current = Quaternion.Angle(start, rb.transform.localRotation);

                //if angle is greater than total, than we are moving in the opposite direction
                int isNegative = Quaternion.Angle(end, rb.transform.localRotation) > total ? -1 : 1;
                //Debug.Log($"total {total} and current {current} isNegative {Quaternion.Angle(end, rb.transform.localRotation)} result {current / total * isNegative}");

                return current / total * isNegative;
            }
        }

        /// <summary>
        /// Get delta position or rotation for rigidbody. Clamped between -1 and 1
        /// </summary>
        /// <returns></returns>
        public float GetDeltaClamped()
        {
            return Mathf.Clamp(GetDelta(), -1, 1);
        }

        public enum EType
        {
            Position, Rotation
        }
    }
}