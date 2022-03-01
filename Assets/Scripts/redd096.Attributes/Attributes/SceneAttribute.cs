using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        string[] scenes;
        int index;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //if value is int or string
            if (property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.String)
            {
                //get scenes name
                scenes = GetScenesInBuildIndex();

                //find current selected index, then show dropdown to select
                index = GetCurrentIndex(property);
                index = EditorGUI.Popup(position, label.text, index, scenes);

                //set int or string value
                if (property.propertyType == SerializedPropertyType.Integer)
                    property.intValue = index;                
                else if (property.propertyType == SerializedPropertyType.String)
                    property.stringValue = scenes[index];
            }
            //else show warning
            else
            {
                Debug.LogWarning(typeof(SceneAttribute).Name + " can't be used on '" + property.name + "'. It can be used only on int or string variables", property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        string[] GetScenesInBuildIndex()
        {
            //get name of every scene in build settings
            List<string> scenesList = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                //only if enabled
                if (EditorBuildSettings.scenes[i].enabled)
                    scenesList.Add(GetSceneName(EditorBuildSettings.scenes[i].path));
            }

            return scenesList.ToArray();
        }

        string GetSceneName(string scenePath)
        {
            //path is Assets/ScenesFolder/SceneName.unity
            int slashIndex = scenePath.LastIndexOf('/');
            int pointIndex = scenePath.LastIndexOf('.');

            //get name between last Slash and last Point
            if (slashIndex >= 0 && pointIndex >= 0)
                return scenePath.Substring(slashIndex + 1, pointIndex - (slashIndex + 1));

            return "";
        }

        int GetCurrentIndex(SerializedProperty property)
        {
            //return current index
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (property.intValue < scenes.Length)
                    return property.intValue;
            }
            //or find current index in scenes name
            else if (property.propertyType == SerializedPropertyType.String)
            {
                for (int i = 0; i < scenes.Length; i++)
                    if (scenes[i] == property.stringValue)  //check scene name
                        return i;
            }

            return 0;
        }
    }

#endif

    #endregion

    /// <summary>
    /// Show dropdown of every scene in Build Index
    /// </summary>
    public class SceneAttribute : PropertyAttribute
    {
    }
}