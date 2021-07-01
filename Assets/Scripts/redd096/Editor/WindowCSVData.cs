namespace redd096
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    public class WindowCSVData : ScriptableObject
    {
        const string DATANAME = "Window CSV Data.asset";


        [Header("Download")]
        [TextArea(2, 5)]
        public string LinkCSV = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQUwVhCiPn1nvfdwMfOwU1RDmPAmllJ4tQYx7w-30kFyiSomK_GR7ebaFNnBYF6MlJEWuHbgK03kMdb/pub?gid=0&single=true&output=csv";

        [Header("Window")]
        public string[] OptionsPath = new string[] { "Data Path", "Persistent Data Path" };
        public int IndexPath = 0;

        [Header("CSV")]
        [TextArea(2, 5)]
        public string PathDownload = "";
        public string FolderName = "Commons/CSV";
        public string FileName = "Songs.csv";

        public string PathFolder => Path.Combine(PathDownload, FolderName);
        public string PathFile => Path.Combine(PathDownload, FolderName, FileName);

        #region static functions

        /// <summary>
        /// Get Path to this scriptable object saved in project folder
        /// </summary>
        /// <returns></returns>
        public static string GetDataPath()
        {
            //find script folder and create scriptable object there
            string[] guids = AssetDatabase.FindAssets(typeof(WindowCSVData).ToString());
            foreach (string guid in guids)
            {
                string pathToScript = AssetDatabase.GUIDToAssetPath(guid);                                                                          //get path to the script
                string pathToFolder = pathToScript.Remove(pathToScript.LastIndexOf('/'), pathToScript.Length - pathToScript.LastIndexOf('/'));      //remove everything from last slash (script name) to get only directory path

                return Path.Combine(pathToFolder, DATANAME);
            }

            return null;
        }

        /// <summary>
        /// Load or Create Scriptable Object
        /// </summary>
        /// <returns></returns>
        public static WindowCSVData LoadData()
        {
            //try get scriptable object (data)
            WindowCSVData data = AssetDatabase.LoadAssetAtPath<WindowCSVData>(GetDataPath());

            //if there is no data, create it
            if (data == null)
            {
                data = CreateInstance<WindowCSVData>();
                data.PathDownload = Application.dataPath;           //initialize path because is not possible doing when initialize var :/
                AssetDatabase.CreateAsset(data, GetDataPath());
            }

            //load data
            return data;
        }

        #endregion
    }
}