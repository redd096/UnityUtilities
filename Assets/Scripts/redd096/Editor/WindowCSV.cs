namespace redd096
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.Networking;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class WindowCSV : EditorWindow
    {
        string[] optionsPath = new string[] { "Data Path", "Persistent Data Path" };
        int indexPath;

        /// <summary>
        /// Open Window from Editor
        /// </summary>
        [MenuItem("redd096/Open Window CSV")]
        static void OpenWindowCSV()
        {
            //open window (and set title)
            GetWindow<WindowCSV>("Window CSV");
        }

        void OnGUI()
        {
            EditorGUILayout.Space();

            //set link to CSV
            SetLinkCSV();

            EditorGUILayout.Space(50);

            //set path download
            SetPathDownload();

            EditorGUILayout.Space();

            //set folder and file name
            SetFolderAndFileName();

            EditorGUILayout.Space();

            //button to download CSV
            ButtonDownloadCSV();

            EditorGUILayout.Space(30);

            //button abort and percentage download CSV (only if there is a download)
            if (ManageDownloadCSV.www != null)
            {
                ButtonAbortAndPercentageDownloadCSV();
            }
        }

        void SetLinkCSV()
        {
            //set link to CSV
            EditorGUILayout.LabelField("Link to CSV:", new GUIStyle(EditorStyles.boldLabel));
            ManageDownloadCSV.linkCSV = EditorGUILayout.TextArea(ManageDownloadCSV.linkCSV, new GUIStyle(EditorStyles.textArea));
        }

        void SetPathDownload()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //set data path
            indexPath = EditorGUILayout.Popup(indexPath, optionsPath);
            if (GUILayout.Button("Set Path"))
            {
                if (indexPath <= 0)
                    ManageCSV.pathDownload = Application.dataPath;
                else
                    ManageCSV.pathDownload = Application.persistentDataPath;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            //text area file path
            EditorGUILayout.LabelField("File Path:", new GUIStyle(EditorStyles.boldLabel));
            ManageCSV.pathDownload = EditorGUILayout.TextArea(ManageCSV.pathDownload, new GUIStyle(EditorStyles.textArea));
        }

        void SetFolderAndFileName()
        {
            //set folder name
            ManageCSV.folderName = EditorGUILayout.TextField("Folder:", ManageCSV.folderName, new GUIStyle(EditorStyles.textField));

            //set file name
            ManageCSV.fileName = EditorGUILayout.TextField("File:", ManageCSV.fileName, new GUIStyle(EditorStyles.textField));
        }

        void ButtonDownloadCSV()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //button to download CSV
            if (GUILayout.Button("Download CSV"))
            {
                ManageDownloadCSV.DownloadCSV();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        void ButtonAbortAndPercentageDownloadCSV()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //button to abort
            if (GUILayout.Button("Abort Download"))
            {
                ManageDownloadCSV.AbortDownloadCSV();
                return;     //return to avoid error, because there will be no download to show percentage
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            //show percentage
            EditorGUILayout.Slider("Percentage:", ManageDownloadCSV.www.downloadProgress, 0f, 1f);

            //when complete, show error or Download Completed!
            if (ManageDownloadCSV.www.isDone)
            {
                if (ManageDownloadCSV.www.isHttpError || ManageDownloadCSV.www.isNetworkError)
                    EditorGUILayout.LabelField("Error:", ManageDownloadCSV.www.error, new GUIStyle(EditorStyles.boldLabel));
                else
                    EditorGUILayout.LabelField("Download Completed!", new GUIStyle(EditorStyles.boldLabel));
            }
        }
    }

    public static class ManageDownloadCSV
    {
        public static string linkCSV { get; set; } = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQUwVhCiPn1nvfdwMfOwU1RDmPAmllJ4tQYx7w-30kFyiSomK_GR7ebaFNnBYF6MlJEWuHbgK03kMdb/pub?gid=0&single=true&output=csv";
        public static UnityWebRequest www { get; set; }

        /// <summary>
        /// Start Download from link
        /// </summary>
        public static void DownloadCSV()
        {
            //UnityWebRequest replace old WWW
            www = UnityWebRequest.Get(linkCSV);
            UnityWebRequestAsyncOperation request = www.SendWebRequest();
            request.completed += OnCompleteDownload;

            ////wait download
            //while (!request.isDone) { }
        }

        /// <summary>
        /// If downloading, Abort it
        /// </summary>
        public static void AbortDownloadCSV()
        {
            //if there is a download, abort it
            if (www != null)
            {
                www.Abort();
                www = null;
            }
        }

        /// <summary>
        /// On complete download, Save CSV
        /// </summary>
        /// <param name="asyncOperation"></param>
        static void OnCompleteDownload(AsyncOperation asyncOperation)
        {
            //do only if there are not errors
            if (www.isHttpError || www.isNetworkError)
                return;

            //save CSV
            ManageCSV.SaveFile(www.downloadHandler.text);
        }
    }

    public static class ManageCSV
    {
        public static string pathDownload { get; set; } = Application.dataPath;
        public static string folderName { get; set; } = "Commons/CSV";
        public static string fileName { get; set; } = "Options.csv";

        static string pathFolder => Path.Combine(pathDownload, folderName);
        static string pathFile => Path.Combine(pathDownload, folderName, fileName);

        /// <summary>
        /// Save downloaded File in folder
        /// </summary>
        /// <param name="value"></param>
        public static void SaveFile(string value)
        {
            //if there is no directory, create it
            if (Directory.Exists(pathFolder) == false)
            {
                Directory.CreateDirectory(pathFolder);
            }

            //create stream at file position
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(pathFile, FileMode.Create);

            //then save value to file position, and close stream
            formatter.Serialize(stream, value);
            stream.Close();
        }

        /// <summary>
        /// Load File from folder
        /// </summary>
        public static string LoadFile()
        {
            //if there is no file, return null
            if (File.Exists(pathFile) == false)
            {
                Debug.Log("File not found: " + pathFile);
                return null;
            }

            //create stream at file position
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(pathFile, FileMode.Open);

            //then load from file position as value, and close stream
            string value = formatter.Deserialize(stream) as string;
            stream.Close();

            return value;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        public static void DeleteFile()
        {
            //check there is a file
            if (File.Exists(pathFile) == false)
            {
                Debug.Log("File not found: " + pathFile);
                return;
            }

            //delete file
            File.Delete(pathFile);
        }

        /// <summary>
        /// Delete directory with every file
        /// </summary>
        public static void DeleteDirectory()
        {
            //check there is a directory
            if (Directory.Exists(pathFolder) == false)
            {
                Debug.Log("Directory not found: " + pathFolder);
                return;
            }

            //delete directory
            Directory.Delete(pathFolder, true);
        }
    }
}