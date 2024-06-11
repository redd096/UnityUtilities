#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace redd096.CsvImporter
{
    /// <summary>
    /// Scriptable object created by window, to save settings
    /// </summary>
    public class WindowCsvImporterData : ScriptableObject
    {
        public int Index = 0;
        public List<ImportCsvElement> Elements = new List<ImportCsvElement>();

        public ImportCsvElement CurrentElement
        {
            get
            {
                //be sure to have at least one element
                if (Elements == null || Elements.Count == 0)
                    Elements = new List<ImportCsvElement>() { new ImportCsvElement() };

                Index = Mathf.Clamp(Index, 0, Elements.Count - 1);
                return Elements[Index];
            }
        }

        /// <summary>
        /// Find every element with same name, and rename them to not have duplicates
        /// </summary>
        public void RenameDuplicates()
        {
            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                //check every prev element in the list
                for (int j = i - 1; j >= 0; j--)
                {
                    //if same name already in the list, change this to not have duplicates
                    while (Elements[i].Name.Equals(Elements[j].Name))
                        Elements[i].Name += "#";
                }
            }
        }
    }

    /// <summary>
    /// Elements to save in scriptable object
    /// </summary>
    [System.Serializable]
    public class ImportCsvElement
    {
        [Header("Name")]
        public string Name = "Element";

        [Header("Download")]
        [TextArea(2, 5)]
        public string LinkCSV = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSkP8ujm2voXc1L34U4LTbhWybChPVCfSUk3_Z-74Lpvr8h-ZFoQ8DwVIvGXA2IQmPf9Tychxt6RGbi/pub?output=csv";

        [Header("CSV")]
        public EOptionsPath OptionsPath = EOptionsPath.None;
        [TextArea(2, 5)]
        public string BaseDownloadPath = "";
        public string FolderName = "Commons/CSV";
        public string FileName = "File.csv";

        public string DirectoryPath => Path.Combine(BaseDownloadPath, FolderName);
        public string FilePath => Path.Combine(BaseDownloadPath, FolderName, FileName);
    }

    public enum EOptionsPath { None, DataPath, PersistentDataPath }
}
#endif