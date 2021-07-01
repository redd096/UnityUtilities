namespace redd096
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.Networking;
    using System.IO;

    public class WindowCSV : EditorWindow
    {
        Vector2 scrollPosition = Vector2.zero;
        WindowCSVData data;

        /// <summary>
        /// Open Window from Editor
        /// </summary>
        [MenuItem("redd096/Open Window CSV")]
        static void OpenWindowCSV()
        {
            //open window (and set title)
            GetWindow<WindowCSV>("Window CSV");
        }

        void OnEnable()
        {
            //load data
            data = WindowCSVData.LoadData();
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space(15);

            //set link to CSV
            SetLinkCSV();

            EditorGUILayout.Space(50);

            //GUI download path
            GUIDownloadPath();

            EditorGUILayout.Space(30);

            //button abort and percentage download CSV (only if there is a download)
            if (ManageDownloadCSV.www != null)
            {
                ButtonAbortAndPercentageDownloadCSV();
            }

            EditorGUILayout.Space(60);

            //buttons delete file and delete directory
            ButtonDeleteFileAndDirectory();

            EditorGUILayout.Space(15);
            EditorGUILayout.EndScrollView();
        }

        #region window GUI API

        void SetLinkCSV()
        {
            //set link to CSV
            EditorGUILayout.LabelField("Link to CSV", new GUIStyle(EditorStyles.boldLabel));
            data.LinkCSV = EditorGUILayout.TextArea(data.LinkCSV, new GUIStyle(EditorStyles.textArea));
        }

        void GUIDownloadPath()
        {
            //set path download
            SetPathDownload();

            EditorGUILayout.Space();

            //set folder and file name
            SetFolderAndFileName();

            EditorGUILayout.Space();

            //button to download CSV
            ButtonDownloadCSV();
        }

        void SetPathDownload()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //set directory path
            data.IndexPath = EditorGUILayout.Popup(data.IndexPath, data.OptionsPath);
            if (GUILayout.Button("Set Path"))
            {
                if (data.IndexPath <= 0)
                    data.PathDownload = Application.dataPath;
                else
                    data.PathDownload = Application.persistentDataPath;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //set path download
            EditorGUILayout.LabelField("Path Download", new GUIStyle(EditorStyles.boldLabel));
            data.PathDownload = EditorGUILayout.TextArea(data.PathDownload, new GUIStyle(EditorStyles.textArea));
        }

        void SetFolderAndFileName()
        {
            //set folder name
            data.FolderName = EditorGUILayout.TextField("Folder:", data.FolderName, new GUIStyle(EditorStyles.textField));

            //set file name
            data.FileName = EditorGUILayout.TextField("File:", data.FileName, new GUIStyle(EditorStyles.textField));
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

        void ButtonDeleteFileAndDirectory()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //button to delete file
            if (GUILayout.Button("Delete File"))
            {
                ManageCSV.DeleteFile();
            }

            EditorGUILayout.Space();

            //button to delete directory
            if (GUILayout.Button("Delete Directory"))
            {
                ManageCSV.DeleteDirectory();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }

    public static class ManageDownloadCSV
    {
        public static UnityWebRequest www { get; set; }

        /// <summary>
        /// Start Download from link
        /// </summary>
        public static void DownloadCSV()
        {
            //load data
            WindowCSVData data = WindowCSVData.LoadData();

            //UnityWebRequest replace old WWW
            www = UnityWebRequest.Get(data.LinkCSV);
            UnityWebRequestAsyncOperation request = www.SendWebRequest();

            //wait download
            request.completed += OnCompleteDownload;

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
        /// <summary>
        /// Save downloaded File in folder
        /// </summary>
        /// <param name="value"></param>
        public static void SaveFile(string value)
        {
            //load data
            WindowCSVData data = WindowCSVData.LoadData();

            //if there is no directory, create it
            if (Directory.Exists(data.PathFolder) == false)
            {
                Directory.CreateDirectory(data.PathFolder);
            }

            //create stream to file position
            StreamWriter writer = new StreamWriter(data.PathFile);

            //then save value to file position, and close stream
            writer.Write(value);
            writer.Close();
        }

        /// <summary>
        /// Load File from folder
        /// </summary>
        public static string LoadFile()
        {
            //load data
            WindowCSVData data = WindowCSVData.LoadData();

            //if there is no file, return null
            if (File.Exists(data.PathFile) == false)
            {
                Debug.Log("File not found: " + data.PathFile);
                return null;
            }

            //create stream at file position
            StreamReader reader = new StreamReader(data.PathFile);

            //then load from file position as value, and close stream
            string value = reader.ReadToEnd();
            reader.Close();

            return value;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        public static void DeleteFile()
        {
            //load data
            WindowCSVData data = WindowCSVData.LoadData();

            //check there is a file
            if (File.Exists(data.PathFile) == false)
            {
                Debug.Log("File not found: " + data.PathFile);
                return;
            }

            //delete file
            File.Delete(data.PathFile);
            Debug.Log("File deleted successfully: " + data.PathFile);
        }

        /// <summary>
        /// Delete directory with every file
        /// </summary>
        public static void DeleteDirectory()
        {
            //load data
            WindowCSVData data = WindowCSVData.LoadData();

            //check there is a directory
            if (Directory.Exists(data.PathFolder) == false)
            {
                Debug.Log("Directory not found: " + data.PathFolder);
                return;
            }

            //delete directory
            Directory.Delete(data.PathFolder, true);
            Debug.Log("Directory deleted successfully: " + data.PathFolder);
        }
    }
}