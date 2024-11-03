#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace redd096
{
    public class WindowFindGuid : EditorWindow
    {
        string GuidString;

        [MenuItem("Tools/redd096/Find Files/Find by Guid")]
        static void FindByGuid()
        {
            //open window (and set title)
            GetWindow<WindowFindGuid>("Window Find Guid");
        }

        private void OnGUI()
        {
            //text field
            GuidString = EditorGUILayout.TextField("Guid: ", GuidString);

            //button
            if (GUILayout.Button("Find file"))
            {
                FindFile(GuidString);
            }
        }

        private void FindFile(string value)
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

        private Object GetGameObjectFromValue(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            //find file
            if (string.IsNullOrEmpty(path) == false)
                return AssetDatabase.LoadMainAssetAtPath(path);

            //file not found
            return null;
        }
    }
}
#endif