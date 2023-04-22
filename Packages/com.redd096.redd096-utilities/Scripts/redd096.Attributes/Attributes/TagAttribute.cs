﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show dropdown of every tag in the project
    /// </summary>
    public class TagAttribute : PropertyAttribute
    {
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagDrawer : PropertyDrawer
    {
        string[] tags;
        int index;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //if value is string
            if (property.propertyType == SerializedPropertyType.String)
            {
                //get tags
                SceneAttribute at = attribute as SceneAttribute;
                tags = GetTags(property);

                //find current selected index, then show dropdown to select
                index = GetCurrentIndex(property);
                index = EditorGUI.Popup(position, label.text, index, tags);

                //set value
                property.stringValue = index < tags.Length ? tags[index] : "";
            }
            //else show warning
            else
            {
                Debug.LogWarning(property.serializedObject.targetObject + " - " + typeof(SceneAttribute).Name + " can't be used on '" + property.name + "'. It can be used only on string variables", property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        string[] GetTags(SerializedProperty property)
        {
            //get tags
            string[] tagsList = new string[UnityEditorInternal.InternalEditorUtility.tags.Length];
            for (int i = 0; i < tagsList.Length; i++)
            {
                tagsList[i] = UnityEditorInternal.InternalEditorUtility.tags[i];
            }

            return tagsList;
        }

        int GetCurrentIndex(SerializedProperty property)
        {
            for (int i = 0; i < tags.Length; i++)
                if (tags[i] == property.stringValue)        //check tag
                    return i;

            return 0;
        }
    }

#endif

    #endregion
}