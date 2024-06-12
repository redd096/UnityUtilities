#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace redd096.CsvImporter
{
    public static class LoadAssetsUtilities
    {
        /// <summary>
        /// Get a Scriptable Object. If the file doesn't exists, create and return it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPathRelativeToProject">The path to the file, but the path must starts with Assets</param>
        /// <returns></returns>
        public static T GetAsset<T>(string assetPathRelativeToProject) where T : ScriptableObject
        {
            //try get scriptable object (data)
            T data = AssetDatabase.LoadAssetAtPath<T>(assetPathRelativeToProject);

            //if there is no data, create it
            if (data == null)
            {
                //create also folders
                string pathToProject = Application.dataPath.Replace("Assets", string.Empty);    //remove Assets because it should already be in assetPath
                string projectDirectories = Path.GetDirectoryName(assetPathRelativeToProject);  //remove file from path and keep only directories
                string path = Path.Combine(pathToProject, projectDirectories);
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                data = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(data, assetPathRelativeToProject);
            }

            //return data
            return data;
        }

        /// <summary>
        /// Get Path to this script in project. NB the path is relative to the project, doesn't contain the path to the Assets folder
        /// </summary>
        /// <returns></returns>
        public static string GetScriptPath<T>()
        {
            //find every scripts of this type
            string[] guids = AssetDatabase.FindAssets(typeof(T).Name);
            foreach (string guid in guids)
            {
                //get path to the script, then remove everything from last slash (script name) to get only directory path
                string pathToScript = AssetDatabase.GUIDToAssetPath(guid);
                return pathToScript.Remove(pathToScript.LastIndexOf('/'), pathToScript.Length - pathToScript.LastIndexOf('/'));
            }

            return null;
        }
    }
}
#endif