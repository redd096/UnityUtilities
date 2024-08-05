using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using redd096.Attributes.AttributesEditorUtility;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show an enum with multiple selection
    /// </summary>
    public class EnumFlagsAttribute : PropertyAttribute
    {

    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            System.Enum targetEnum = property.GetValue(property.name) as System.Enum;

            //show enum flags
            if (targetEnum != null)
            {
                System.Enum result = EditorGUI.EnumFlagsField(position, label, targetEnum);
                property.intValue = System.Convert.ToInt32(result);
            }
            //else show warning
            else
            {
                Debug.LogWarning(property.serializedObject.targetObject + " - " + GetType().Name + " can't be used on '" + property.name + "'. It can be used only on enums", property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }
    }

#endif

    #endregion
}