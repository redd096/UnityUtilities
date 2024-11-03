#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace redd096
{
    public class WindowFindFileID : EditorWindow
    {
        string fileIDString;

        [MenuItem("Tools/redd096/Find Files/Find by fileID (Local Identifier)")]
        static void FindByFileId()
        {
            //open window (and set title)
            GetWindow<WindowFindFileID>("Window Find File ID");
        }

        private void OnGUI()
        {
            //text field
            fileIDString = EditorGUILayout.TextField("File ID (Local identifier): ", fileIDString);

            //button
            if (GUILayout.Button("Find file"))
            {
                if (long.TryParse(fileIDString, out long fileID))
                    FindFile(fileID);
                else
                    Debug.LogError("Text must be a File ID (long value)");
            }
        }

        private void FindFile(long value)
        {
            //find file
            Object result = GetGameObjectFromValue(value);
            if (result == null)
            {
                Debug.LogError("Not found object with value: " + value);
                return;
            }

            //select it
            Debug.Log($"With value [{value}] the result is: {result}", result);
            GUI.FocusControl(null);
            Selection.activeObject = result;
            EditorGUIUtility.PingObject(result);
        }

        private GameObject GetGameObjectFromValue(long fileID)
        {
            //get objects in resources and in scene
            List<GameObject> gameObjects = new List<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>());
            GameObject[] gameObjectsInScene = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            gameObjects.AddRange(gameObjectsInScene);

            ////try find by fileID - for some reason it return always False
            //foreach (var go in gameObjects)
            //{
            //    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(go, out string guid, out long localID);
            //    if (localID == fileID)
            //        return go;
            //}

            // Test every gameobjects
            foreach (var go in gameObjects)
            {
                if (GetFileID(go) == fileID)
                    return go;
            }
            // Test every gameobjects transforms
            foreach (var go in gameObjects)
            {
                if (GetFileID(go.transform) == fileID)
                    return go;
            }

            return null;
        }

        long GetFileID(Object obj)
        {
            PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
            SerializedObject serializedObject = new SerializedObject(obj);
            inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
            SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");

            return localIdProp.longValue;
        }
    }
}
#endif