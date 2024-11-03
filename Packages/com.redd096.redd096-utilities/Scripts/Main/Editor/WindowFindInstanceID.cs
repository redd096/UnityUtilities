#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace redd096
{
    public class WindowFindInstanceID : EditorWindow
    {
        string InstanceIDString;

        [MenuItem("Tools/redd096/Find Files/Find by Instance ID")]
        static void FindByInstanceID()
        {
            //open window (and set title)
            GetWindow<WindowFindInstanceID>("Window Find Instance ID");
        }

        private void OnGUI()
        {
            //text field
            InstanceIDString = EditorGUILayout.TextField("Instance ID: ", InstanceIDString);

            //button
            if (GUILayout.Button("Find file"))
            {
                if (int.TryParse(InstanceIDString, out int value))
                    FindFile(value);
                else
                    Debug.LogError("Text must be a Instance ID (int value)");
            }
        }

        private void FindFile(int value)
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

        private Object GetGameObjectFromValue(int instanceID)
        {
            GameObject[] gameObjectsInScene = FindObjectsOfType<GameObject>();

            //find file
            foreach (GameObject go in gameObjectsInScene)
            {
                int id = go.GetInstanceID();
                if (id == instanceID)
                    return go;
            }
            //for some reason in Inspector Debug Mode, the showed Instance ID is wrong. It has +2 at its value, so try decrease to find the correct object
            foreach (GameObject go in gameObjectsInScene)
            {
                int id = go.GetInstanceID() - 2;
                if (id == instanceID)
                    return go;
            }

            //file not found
            return null;
        }
    }
}
#endif