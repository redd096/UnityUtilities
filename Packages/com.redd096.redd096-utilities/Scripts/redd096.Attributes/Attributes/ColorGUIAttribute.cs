using UnityEngine;
#if UNITY_EDITOR
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Attribute to change color for this property
    /// </summary>
    public class ColorGUIAttribute : PropertyAttribute
    {
        public readonly AttributesUtility.EColor color;
        public readonly string colorValue;
        public readonly EColorType colorType;

        public enum EColorType { GUI, Background, Content }

        public ColorGUIAttribute(AttributesUtility.EColor color, EColorType colorType = EColorType.Content)
        {
            this.color = color;
            this.colorValue = string.Empty;
            this.colorType = colorType;
        }

        public ColorGUIAttribute(string colorValue, EColorType colorType = EColorType.Content)
        {
            this.colorValue = colorValue;
            this.colorType = colorType;
        }
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ColorGUIAttribute))]
    public class ColorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //update height when open a list/array/struct in inspector
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColorGUIAttribute at = attribute as ColorGUIAttribute;

            //color GUI for this property
            Color previousColor;
            SetColorGUI(at, property, out previousColor);

            EditorGUI.PropertyField(position, property, label, true);

            //reset GUI color
            ResetColorGUI(at, previousColor);
        }

        void SetColorGUI(ColorGUIAttribute at, SerializedProperty property, out Color previousColor)
        {
            //get color from attribute or property
            Color colorToUse = default;
            if (string.IsNullOrEmpty(at.colorValue))
            {
                colorToUse = AttributesUtility.GetColor(at.color);
            }
            else
            {
                //property can be EColor or Color
                object obj = property.GetValue(at.colorValue, typeof(Color), typeof(Color32), typeof(AttributesUtility.EColor));
                if (obj is AttributesUtility.EColor eColor)
                {
                    colorToUse = AttributesUtility.GetColor(eColor);
                }
                else if (obj is Color32 color32)
                {
                    colorToUse = color32;
                }
                else if (obj is Color newColor)
                {
                    colorToUse = newColor;
                }
                else
                {
                    Debug.LogWarning(property.serializedObject.targetObject + " - " + typeof(ColorGUIAttribute).Name + " can't be used on '" + property.name + "'. It can be used only on Color, Color32 and AttributesUtility.EColor variables", property.serializedObject.targetObject);
                }
            }

            //set GUI color
            if (at.colorType == ColorGUIAttribute.EColorType.GUI)
            {
                previousColor = GUI.color;
                GUI.color = colorToUse;
            }
            //or content color
            else if (at.colorType == ColorGUIAttribute.EColorType.Content)
            {
                previousColor = GUI.contentColor;
                GUI.contentColor = colorToUse;
            }
            //or background color
            else
            {
                previousColor = GUI.backgroundColor;
                GUI.backgroundColor = colorToUse;
            }
        }

        void ResetColorGUI(ColorGUIAttribute at, Color previousColor)
        {
            //reset GUI, content or background color
            if (at.colorType == ColorGUIAttribute.EColorType.GUI)
                GUI.color = previousColor;
            else if (at.colorType == ColorGUIAttribute.EColorType.Content)
                GUI.contentColor = previousColor;
            else
                GUI.backgroundColor = previousColor;
        }
    }

#endif

    #endregion
}