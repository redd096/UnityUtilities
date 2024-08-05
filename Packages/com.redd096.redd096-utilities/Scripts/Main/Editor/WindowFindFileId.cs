#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace redd096
{
    public class WindowFindFileId : EditorWindow
    {
        string fileIDString;

        [MenuItem("Tools/redd096/Find Files/Find by fileID")]
        static void FindByFileId()
        {
            //open window (and set title)
            GetWindow<WindowFindFileId>("Window Find File ID");
        }

        private void OnGUI()
        {
            //text field
            fileIDString = EditorGUILayout.TextField("File ID: ", fileIDString);

            //button
            if (GUILayout.Button("Find file"))
            {
                if (long.TryParse(fileIDString, out long fileID))
                    FindFile(fileID);
                else
                    Debug.LogError("Text must be a File ID (long value)");
            }
        }

        private void FindFile(long fileID)
        {
            //find file ID
            GameObject resultGo = GetGameObjectFromFileID(fileID);
            if (resultGo == null)
            {
                Debug.LogError("FileID not found for fileID = " + fileID);
                return;
            }

            //select it
            Debug.Log("GameObject for fileID " + fileID + " is " + resultGo, resultGo);
            GameObject[] newSelection = new GameObject[] { resultGo };
            Selection.objects = newSelection;
        }

        private GameObject GetGameObjectFromFileID(long fileID) // also called local identifier
        {
            GameObject resultGo = null;
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            //List<GameObject> gameObjects = new List<GameObject>(FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID));
            //gameObjects.AddRange(Resources.FindObjectsOfTypeAll<GameObject>());

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

    //public static class FindFileId
    //{
    //    [MenuItem("redd096/Select in scene by fileID")]
    //    public static void SelectInSceneByFileId()
    //    {
    //        long fileID = 804738792;
    //        GameObject resultGo = GetGameObjectFromFileID(fileID);
    //        if (resultGo == null)
    //        {
    //            Debug.LogError("FileID not found for fileID = " + fileID);
    //            return;
    //        }
    //        Debug.Log("GameObject for fileID " + fileID + " is " + resultGo, resultGo);
    //        GameObject[] newSelection = new GameObject[] { resultGo };
    //        Selection.objects = newSelection;
    //    }
    //}
}
#endif