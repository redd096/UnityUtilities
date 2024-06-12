#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace redd096.CsvImporter
{
    public static class CreateAssetsUtilities
    {
        /// <summary>
        /// Cycle every row and for every row create an asset, then call event to set variables
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="skipFirstRow"></param>
        /// <param name="filesPath">Directory where we want to create the assets</param>
        /// <param name="setFileName">Return a file name</param>
        /// <param name="setAssetVariables">Set object variables in this function</param>
        public static async Task CreateAssets<T>(FParseResult result, bool skipFirstRow, string filesPath, System.Func<FParseResult, int, string> setFileName, System.Action<T, FParseResult, int> setAssetVariables) where T : ScriptableObject
        {
            //create an asset for every row
            for (int row = 0; row < result.Rows.Count; row++)
            {
                //ignore first row, is used only to give a name to every column
                if (skipFirstRow && row == 0)
                    continue;

                //use callback to set file name
                string fileName = setFileName?.Invoke(result, row);
                string filePath = Path.Combine(filesPath, fileName + ".asset");

                //load if already created, or create a new scriptable object with fileName
                var data = LoadAssetsUtilities.GetAsset<T>(filePath);

                //set asset's variables
                setAssetVariables?.Invoke(data, result, row);

                //set dirty to save
                EditorUtility.SetDirty(data);

                //show progress bar and delay if created too much files
                EditorUtility.DisplayProgressBar("Creating Assets", $"Creating... {row}/{result.Rows.Count}", (float)row / result.Rows.Count);
                if (row % 20 == 0)
                    await Task.Delay((int)(Time.deltaTime * 1000));
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
#endif