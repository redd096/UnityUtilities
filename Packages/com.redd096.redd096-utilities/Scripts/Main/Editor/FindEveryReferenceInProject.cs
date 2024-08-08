#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

namespace redd096
{
    public class FindEveryReferenceInProject
    {
        private const string MenuItemText = "Assets/Find EVERY Reference In Project";

        [MenuItem(MenuItemText, false, 25)]
        public static async void Find()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //save every asset and its dependencies in the project
            var referenceCache = new Dictionary<string, List<string>>();

            string[] guids = AssetDatabase.FindAssets("");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);

                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (referenceCache.ContainsKey(dependencies[i]))
                    {
                        if (!referenceCache[dependencies[i]].Contains(assetPath))
                        {
                            referenceCache[dependencies[i]].Add(assetPath);
                        }
                    }
                    else
                    {
                        referenceCache[dependencies[i]] = new List<string>() { assetPath };
                    }

                    //show progress bar and delay of one frame
                    EditorUtility.DisplayProgressBar($"Saving every dependency for asset at path: {assetPath}", $"Saving... {i}/{dependencies.Length}", (float)i / dependencies.Length);
                    if (i % 20 == 0)
                        await Task.Delay((int)(Time.deltaTime * 1000));
                }
            }

            Debug.Log($"Build index takes {sw.ElapsedMilliseconds} milliseconds");

            //find every reference for the selected object
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            Debug.Log($"Find references for asset at path: {path}", Selection.activeObject);

            if (referenceCache.ContainsKey(path))
            {
                List<string> references = referenceCache[path];
                for (int i = 0; i < references.Count; i++)
                {
                    //stamp reference file
                    Debug.Log(references[i], AssetDatabase.LoadMainAssetAtPath(references[i]));

                    //show progress bar and delay of one frame
                    EditorUtility.DisplayProgressBar($"Finding every reference", $"Finding... {i}/{references.Count}", (float)i / references.Count);
                    if (i % 20 == 0)
                        await Task.Delay((int)(Time.deltaTime * 1000));
                }
            }
            else
            {
                Debug.LogWarning("No references");
            }

            referenceCache.Clear();
        }

        [MenuItem(MenuItemText, true)]
        public static bool Validate()
        {
            if (Selection.activeObject)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                return !AssetDatabase.IsValidFolder(path);
            }

            return false;
        }
    }
}
#endif