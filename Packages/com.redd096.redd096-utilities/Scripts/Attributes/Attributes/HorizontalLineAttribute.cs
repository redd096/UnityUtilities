using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show an horizontal line above the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HorizontalLineAttribute : PropertyAttribute
    {
        //default const
        public const float DefaultHeight = 2.0f;
        public const AttributesUtility.EColor DefaultColor = AttributesUtility.EColor.Gray;

        public readonly float height;
        public readonly AttributesUtility.EColor color;

        public HorizontalLineAttribute(float height = DefaultHeight, AttributesUtility.EColor color = DefaultColor)
        {
            this.height = height;
            this.color = color;
        }
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            HorizontalLineAttribute at = attribute as HorizontalLineAttribute;
            return EditorGUIUtility.singleLineHeight + at.height;
        }

        public override void OnGUI(Rect position)
        {
            HorizontalLineAttribute at = attribute as HorizontalLineAttribute;

            Rect rect = EditorGUI.IndentedRect(position);
            rect.y += EditorGUIUtility.singleLineHeight / 3.0f;
            rect.height = at.height;

            EditorGUI.DrawRect(rect, AttributesUtility.GetColor(at.color));
        }
    }

#endif

    #endregion
}