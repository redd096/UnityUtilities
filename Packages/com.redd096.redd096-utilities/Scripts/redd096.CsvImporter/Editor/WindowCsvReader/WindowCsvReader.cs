#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace redd096.CsvImporter
{
    /// <summary>
    /// Show a window to read a file .csv
    /// </summary>
    public class WindowCsvReader : EditorWindow
    {
        Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// Open Window from Editor
        /// </summary>
        [MenuItem("Tools/redd096/CSV Importer/CSV Reader")]
        static void OpenWindowCSV()
        {
            //open window (and set title)
            GetWindow<WindowCsvReader>("CSV Reader");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space(15);



            EditorGUILayout.Space(15);
            EditorGUILayout.EndScrollView();
        }

        #region editor gui

        #endregion
    }
}
#endif