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
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);

                foreach (var dependency in dependencies)
                {
                    if (referenceCache.ContainsKey(dependency))
                    {
                        if (!referenceCache[dependency].Contains(assetPath))
                        {
                            referenceCache[dependency].Add(assetPath);
                        }
                    }
                    else
                    {
                        referenceCache[dependency] = new List<string>() { assetPath };
                    }
                }

                //show progress bar and delay of one frame
                EditorUtility.DisplayProgressBar($"Creating cache for every object in the project", $"Cache... {i}/{guids.Length}", (float)i / guids.Length);
                if (i % 500 == 0)
                    await Task.Delay((int)(Time.deltaTime * 1000));
            }

            EditorUtility.ClearProgressBar();
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

                EditorUtility.ClearProgressBar();
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