#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace redd096.CsvImporter
{
    /// <summary>
    /// Show a window to import CSV from url
    /// </summary>
    public class WindowCsvImporter : EditorWindow
    {
        WindowCsvImporterData data;
        Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// Open Window from Editor
        /// </summary>
        [MenuItem("Tools/redd096/CSV Importer/Import CSV")]
        static void OpenWindowCSV()
        {
            //open window (and set title)
            GetWindow<WindowCsvImporter>("CSV Importer");
        }

        private void OnEnable()
        {
            //get path to the script, and add assetName.asset
            string assetPath = Path.Combine(LoadDataUtilities.GetScriptPath<WindowCsvImporterData>(), "WindowCsvImporter Data.asset");
            data = LoadDataUtilities.GetAsset<WindowCsvImporterData>(assetPath);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space(15);

            EditorGUI.BeginChangeCheck();

            SelectItemFromList();
            SetLinkCSV();
            EditorGUILayout.Space(20);
            SetPathDownload();
            EditorGUILayout.Space(30);
            ButtonDownloadCSV();
            EditorGUILayout.Space(30);
            ButtonsDelete();

            //if there are changes, save it
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(data);
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.EndScrollView();
        }

        #region window gui

        void SelectItemFromList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //create options with name of every item in the list
            data.RenameDuplicates();
            string[] optionsList = new string[data.Elements.Count];
            for (int i = 0; i < data.Elements.Count; i++)
            {
                optionsList[i] = data.Elements[i].Name;
            }

            //show every item
            data.Index = EditorGUILayout.Popup(data.Index, optionsList);

            EditorGUILayout.Space();

            //set name link
            data.CurrentElement.Name = EditorGUILayout.TextField(data.CurrentElement.Name, EditorStyles.textField);

            EditorGUILayout.Space();

            //create new element at index
            if (GUILayout.Button("Add Element"))
            {
                ImportCsvElement newElement = new ImportCsvElement();

                //add item to the list and move index to the item created
                data.Elements.Insert(data.Index + 1, newElement);
                data.Index += 1;
                data.RenameDuplicates();
            }

            //remove element at index
            GUI.enabled = data.Elements.Count > 1;
            if (GUILayout.Button("Remove Element"))
            {
                //only if there are others (always keep 1 in the list)
                if (data.Elements.Count > 1)
                {
                    //remove from the list
                    data.Elements.RemoveAt(data.Index);

                    //if index is not at 0, move to previous item in the list
                    if (data.Index > 0)
                        data.Index -= 1;
                }
            }
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void SetLinkCSV()
        {
            //set link to CSV
            EditorGUILayout.LabelField("Links to CSV", EditorStyles.boldLabel);
            data.CurrentElement.LinkCSV = EditorGUILayout.TextArea(data.CurrentElement.LinkCSV, EditorStyles.textArea);
        }

        void SetPathDownload()
        {
            EditorGUILayout.LabelField("Path Download", EditorStyles.boldLabel);

            //set enum
            var prevOptionsPath = data.CurrentElement.OptionsPath;
            data.CurrentElement.OptionsPath = (EOptionsPath)EditorGUILayout.EnumPopup(data.CurrentElement.OptionsPath, GUILayout.Width(200));

            //set base path from enum
            if (prevOptionsPath != data.CurrentElement.OptionsPath)
            {
                data.CurrentElement.BaseDownloadPath = (data.CurrentElement.OptionsPath) switch
                {
                    EOptionsPath.None => "",
                    EOptionsPath.DataPath => Application.dataPath,
                    EOptionsPath.PersistentDataPath => Application.persistentDataPath,
                    _ => "",
                };
            }

            //set base path, folder and file name
            data.CurrentElement.BaseDownloadPath = EditorGUILayout.TextField("Base Path:", data.CurrentElement.BaseDownloadPath, EditorStyles.textArea);
            data.CurrentElement.FolderName = EditorGUILayout.TextField("Folder:", data.CurrentElement.FolderName, EditorStyles.textField);
            data.CurrentElement.FileName = EditorGUILayout.TextField("File:", data.CurrentElement.FileName, EditorStyles.textField);
            EditorGUILayout.SelectableLabel(data.CurrentElement.FilePath);
        }

        void ButtonDownloadCSV()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //button to download CSV
            if (GUILayout.Button("Download CSV"))
            {
                CsvImporter.DownloadCsv(data.CurrentElement.LinkCSV, data.CurrentElement.DirectoryPath, data.CurrentElement.FileName);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        void ButtonsDelete()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //button to delete file
            if (GUILayout.Button("Delete File"))
            {
                ManageFile.DeleteFile(data.CurrentElement.FilePath);
            }

            EditorGUILayout.Space();

            //button to delete directory
            if (GUILayout.Button("Delete Directory"))
            {
                ManageFile.DeleteDirectory(data.CurrentElement.DirectoryPath);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
#endif