#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace redd096.CsvImporter
{
    public static class LoadDataUtilities
    {
        /// <summary>
        /// Load or Create Scriptable Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static T LoadData<T>(string assetName) where T : ScriptableObject
        {
            //get path to the script, and add assetName.asset
            string assetPath = Path.Combine(GetScriptPath<T>(), assetName + ".asset");

            //try get scriptable object (data)
            T data = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            //if there is no data, create it
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(data, assetPath);
            }

            //load data
            return data;
        }

        /// <summary>
        /// Get Path to this script in project
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