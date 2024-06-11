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

        private void FindFile(string guid)
        {
            //find file ID
            Object resultGo = GetGameObjectFromGuid(guid);
            if (resultGo == null)
            {
                Debug.LogError("Guid not found for guid = " + guid);
                return;
            }

            //select it
            Debug.Log("GameObject for guid " + guid + " is " + resultGo, resultGo);
            Object[] newSelection = new Object[] { resultGo };
            Selection.objects = newSelection;
        }

        private Object GetGameObjectFromGuid(string guid)
        {
            Object resultGo = null;
            string path = AssetDatabase.GUIDToAssetPath(guid);

            //file not found
            if (string.IsNullOrEmpty(path))
                return null;

            GUI.FocusControl(null);
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;

            resultGo = asset;

            return resultGo;
        }
    }
}
#endif