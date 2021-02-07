namespace redd096
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Color", menuName = "redd096/UIControl/Color")]
    public class ColorValue : ScriptableObject
    {
        [SerializeField] Color color = Color.red;

        public Color Color { get { return color; } }
    }
}