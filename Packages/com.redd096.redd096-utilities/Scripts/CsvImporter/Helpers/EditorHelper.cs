#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace redd096.CsvImporter
{
    public static class EditorHelper
    {
        /// <summary>
        /// Cycle every row, and for every row find the asset in project, then call event to set its variables
        /// </summary>
        /// <typeparam name="T">Assets type</typeparam>
        /// <param name="result">Result from parsed file</param>
        /// <param name="skipFirstRow">Skip first row if for example it's used to set Columns names</param>
        /// <param name="assetsPath">Directory where we want to create the assets</param>
        /// <param name="setFileNameAndExtension">Use ParseResult and current Row, to return a file name and its extension</param>
        /// <param name="setAssetVariables">Use loaded Asset, ParseResult, current Row and filePath, to set asset variables</param>
        /// <returns></returns>
        public static async Task LoadOneAssetForEveryRow<T>(
            FParseResult result, bool skipFirstRow, string assetsPath,
            System.Func<FParseResult, int, string> setFileNameAndExtension, 
            System.Action<T, FParseResult, int, string> setAssetVariables) where T : Object
        {
            await LoadOneAssetForEveryRow_Internal(
                result, skipFirstRow, assetsPath, 
                setFileNameAndExtension, setAssetVariables, 
                createAssetIfNotFound: false);
        }

        /// <summary>
        /// Cycle every row, and for every row find the asset in project or create it, then call event to set its variables
        /// </summary>
        /// <typeparam name="T">Assets type</typeparam>
        /// <param name="result">Result from parsed file</param>
        /// <param name="skipFirstRow">Skip first row if for example it's used to set Columns names</param>
        /// <param name="assetsPath">Directory where we want to create the assets</param>
        /// <param name="setFileNameAndExtension">Use ParseResult and current Row, to return a file name and its extension</param>
        /// <param name="setAssetVariables">Use loaded Asset, ParseResult, current Row and filePath, to set asset variables</param>
        public static async Task LoadOrCreateOneAssetForEveryRow<T>(
            FParseResult result, bool skipFirstRow, string assetsPath,
            System.Func<FParseResult, int, string> setFileNameAndExtension,
            System.Action<T, FParseResult, int, string> setAssetVariables) where T : ScriptableObject
        {
            await LoadOneAssetForEveryRow_Internal(
                result, skipFirstRow, assetsPath,
                setFileNameAndExtension, setAssetVariables,
                createAssetIfNotFound: true);
        }

        /// <summary>
        /// Return the column with "Name" and scriptable object's extension
        /// </summary>
        public static string DefaultGetScriptableObjectNameAndExtension(FParseResult result, int row)
        {
            return result.GetCellContent("Name", row) + ".asset";
        }

        /// <summary>
        /// Return the column with "Name" and prefab's extension
        /// </summary>
        public static string DefaultGetPrefabNameAndExtension(FParseResult result, int row)
        {
            return result.GetCellContent("Name", row) + ".prefab";
        }

        #region private API

        private static async Task LoadOneAssetForEveryRow_Internal<T>(
            FParseResult result, bool skipFirstRow, string assetsPath,
            System.Func<FParseResult, int, string> setFileNameAndExtension,
            System.Action<T, FParseResult, int, string> setAssetVariables,
            bool createAssetIfNotFound) where T : Object
        {
            for (int row = 0; row < result.Rows.Count; row++)
            {
                //ignore first row, is used only to give a name to every column
                if (skipFirstRow && row == 0)
                    continue;

                //use callback to set file name and get file path
                string fileNameAndExtension = setFileNameAndExtension?.Invoke(result, row);
                string filePath = Path.Combine(assetsPath, fileNameAndExtension);

                //get or create asset
                T data;
                if (createAssetIfNotFound)
                    data = LoadAssetsUtilities.GetOrCreateAsset(filePath, typeof(T)) as T;
                else
                    data = LoadAssetsUtilities.GetAsset<T>(filePath);

                //and set variables
                setAssetVariables?.Invoke(data, result, row, filePath);

                //set dirty to save
                if (data)
                {
                    EditorUtility.SetDirty(data);
                }

                //show progress bar and delay if created too much files
                EditorUtility.DisplayProgressBar("Creating Assets", $"Creating... {row}/{result.Rows.Count}", (float)row / result.Rows.Count);
                if (row % 20 == 0)
                    await Task.Delay((int)(Time.deltaTime * 1000));
            }

            EditorUtility.ClearProgressBar();
        }

        #endregion
    }
}
#endif