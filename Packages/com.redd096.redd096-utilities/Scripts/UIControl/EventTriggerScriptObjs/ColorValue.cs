using UnityEngine;

namespace redd096.UIControl
{
    /// <summary>
    /// Used with CustomEventTrigger
    /// </summary>
    [CreateAssetMenu(fileName = "Color", menuName = "redd096/UI Control/CustomEventTrigger/Color")]
    public class ColorValue : ScriptableObject
    {
        [SerializeField] Color color = Color.red;

        public Color Color { get { return color; } }
    }
}