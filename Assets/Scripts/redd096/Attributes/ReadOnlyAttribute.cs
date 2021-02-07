﻿namespace redd096
{
    using UnityEngine;

#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //update height when open a list/array/struct in inspector
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //disable GUI for this property
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label, true);

            //re-enable GUI
            GUI.enabled = true;
        }
    }

#endif

    /// <summary>
    /// Attribute to show this variable in inspector as read only
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}