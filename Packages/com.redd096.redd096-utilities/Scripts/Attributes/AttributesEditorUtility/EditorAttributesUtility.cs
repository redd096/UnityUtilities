using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes.AttributesEditorUtility
{
    public static class EditorAttributesUtility
    {
#if UNITY_EDITOR

        public static Color GetColor(SerializedProperty property, string colorValue, AttributesUtility.EColor color)
        {
            //return color if string is null
            if (string.IsNullOrEmpty(colorValue))
            {
                return AttributesUtility.GetColor(color);
            }
            else
            {
                //property can be EColor or Color
                object obj = property.GetValue(colorValue, typeof(Color), typeof(Color32), typeof(AttributesUtility.EColor));
                if (obj is AttributesUtility.EColor eColor)
                {
                    return AttributesUtility.GetColor(eColor);
                }
                else if (obj is Color32 color32)
                {
                    return color32;
                }
                else if (obj is Color newColor)
                {
                    return newColor;
                }
                else
                {
                    Debug.LogWarning(property.serializedObject.targetObject + " - color error on property: '" + property.name + "'. It can be used only with Color, Color32 and AttributesUtility.EColor variables", property.serializedObject.targetObject);
                }
            }

            return default;
        }

        public static object GetObjectValue(SerializedProperty property, string valueName, object defaultValue, params System.Type[] types)
        {
            //return default is string is null
            if (string.IsNullOrEmpty(valueName))
            {
                return defaultValue;
            }
            else
            {
                //property can be these types
                object obj = property.GetValue(valueName, types);

                foreach (var type in types)
                {
                    if (obj.GetType() == type)
                        return obj;
                }

                Debug.LogWarning(property.serializedObject.targetObject + " - error on property: '" + property.name + "'. It can be used only these variables: " + ReflectionUtility.SystemTypesToString(false, types), property.serializedObject.targetObject);
                return obj;
            }
        }

#endif
    }
}