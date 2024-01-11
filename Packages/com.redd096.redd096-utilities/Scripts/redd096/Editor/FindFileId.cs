#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace redd096
{
    public static class FindFileId
    {
        [MenuItem("redd096/Select in scene by fileID")]
        public static void SelectInSceneByFileId()
        {
            long fileID = 804738792;
            GameObject resultGo = GetGameObjectFromFileID(fileID);
            if (resultGo == null)
            {
                Debug.LogError("FileID not found for fileID = " + fileID);
                return;
            }
            Debug.Log("GameObject for fileID " + fileID + " is " + resultGo, resultGo);
            GameObject[] newSelection = new GameObject[] { resultGo };
            Selection.objects = newSelection;
        }

        public static GameObject GetGameObjectFromFileID(long fileID) // also called local identifier
        {
            GameObject resultGo = null;
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            // Test every gameobjects
            foreach (var go in gameObjects)
            {
                PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
                SerializedObject serializedObject = new SerializedObject(go);
                inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
                SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");
                if (localIdProp.longValue == fileID) resultGo = go;
            }
            // Test every gameobjects transforms
            foreach (var go in gameObjects)
            {
                PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
                SerializedObject serializedObject = new SerializedObject(go.transform);
                inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
                SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");
                if (localIdProp.longValue == fileID) resultGo = go;
            }

            return resultGo;
        }
    }
}
#endif