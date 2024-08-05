using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show instance of the scene (can drag the scene from project to inspector)
    /// </summary>
    public class SceneInstanceAttribute : PropertyAttribute
    {
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SceneInstanceAttribute))]
    public class SceneInstanceDrawer : PropertyDrawer
    {
        bool isStringProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //if value is int or string
            if (property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.String)
            {
                //find scene in build settings, by index or name
                isStringProperty = property.propertyType == SerializedPropertyType.String;
                SceneAsset scene = isStringProperty ? GetSceneObjectByName(property.stringValue) : GetSceneObjectByIndex(property.intValue);

                //show object property
                Object newScene = EditorGUI.ObjectField(position, label, scene, typeof(SceneAsset), true);

                //if changed scene, try set new value
                if (newScene != null && newScene.Equals(scene) == false)
                {
                    newScene = GetSceneObjectByName(newScene.name, out int index);
                    if (newScene != null)
                    {
                        if (isStringProperty)
                            property.stringValue = newScene.name;
                        else
                            property.intValue = index;
                    }
                }

                //if scene is deleted or not valid, reset property
                if (newScene == null)
                {
                    if (isStringProperty)
                        property.stringValue = "";
                    else
                        property.intValue = -1;

                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            //else show warning
            else
            {
                Debug.LogWarning(property.serializedObject.targetObject + " - " + typeof(SceneInstanceAttribute).Name + " can't be used on '" + property.name + "'. It can be used only on int or string variables", property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        SceneAsset GetSceneObjectByName(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return null;

            //find scene in build settings by name. If not in build settings, show error
            foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes)
            {
                if (editorScene.path.IndexOf(sceneName) != -1)
                {
                    return AssetDatabase.LoadAssetAtPath<SceneAsset>(editorScene.path);
                }
            }

            Debug.LogWarning("Scene [" + sceneName + "] cannot be used. Add this scene to the build settings.");
            return null;
        }

        SceneAsset GetSceneObjectByIndex(int sceneIndex)
        {
            if (sceneIndex < 0)
                return null;

            //find scene in build settings by index. If not in build settings, show error
            if (sceneIndex >= EditorBuildSettings.scenes.Length)
            {
                Debug.LogWarning("Scene at index [" + sceneIndex + "] cannot be used. Add this scene to the build settings.");
                return null;
            }

            EditorBuildSettingsScene editorScene = EditorBuildSettings.scenes[sceneIndex];
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(editorScene.path);
        }

        SceneAsset GetSceneObjectByName(string sceneName, out int indexInBuildSettings)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                indexInBuildSettings = -1;
                return null;
            }

            //find scene in build settings by name. If not in build settings, show error
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene editorScene = EditorBuildSettings.scenes[i];
                if (editorScene.path.IndexOf(sceneName) != -1)
                {
                    indexInBuildSettings = i;
                    return AssetDatabase.LoadAssetAtPath<SceneAsset>(editorScene.path);
                }
            }

            Debug.LogWarning("Scene [" + sceneName + "] cannot be used. Add this scene to the build settings.");
            indexInBuildSettings = -1;
            return null;
        }
    }

#endif

    #endregion
}