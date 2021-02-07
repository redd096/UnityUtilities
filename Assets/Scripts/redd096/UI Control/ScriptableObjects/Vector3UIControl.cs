namespace redd096
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Vector3", menuName = "redd096/UIControl/Vector3")]
    public class Vector3UIControl : ScriptableObject
    {
        [SerializeField] Vector3 vector3 = Vector3.zero;

        public Vector3 Vector3 { get { return vector3; } }
    }
}
