using UnityEngine;

namespace redd096.UIControl
{
    /// <summary>
    /// Used with CustomEventTrigger
    /// </summary>
    [CreateAssetMenu(fileName = "Vector3", menuName = "redd096/UI Control/CustomEventTrigger/Vector3")]
    public class Vector3UIControl : ScriptableObject
    {
        [SerializeField] Vector3 vector3 = Vector3.zero;

        public Vector3 Vector3 { get { return vector3; } }
    }
}
