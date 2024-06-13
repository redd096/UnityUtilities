#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace redd096.CsvImporter.Example
{
    public class ExampleSimpleCsvImporterAndReader
    {
        [MenuItem("Tools/redd096/CSV Importer/Examples/Example Download Csv")]
        static void ExampleImportCsv()
        {
            //download csv in Assets/Example/CSV as DownloadedFile.csv
            CsvImporter.DownloadCsv(
                url: "https://docs.google.com/spreadsheets/d/e/2PACX-1vSkP8ujm2voXc1L34U4LTbhWybChPVCfSUk3_Z-74Lpvr8h-ZFoQ8DwVIvGXA2IQmPf9Tychxt6RGbi/pub?output=csv",
                directoryPath: Path.Combine(Application.dataPath, "Example/CSV"),
                fileName: "DownloadedFile.csv");
        }

        [MenuItem("Tools/redd096/CSV Importer/Examples/Example Create SO with downloaded Csv")]
        static void ExampleCreateScriptableObjects()
        {
            //parse csv file in Assets/Example/CSV/DownloadedFile.csv 
            string filePath = Path.Combine(Application.dataPath, "Example/CSV/DownloadedFile.csv");
            var result = CsvImporter.ReadCsvAtPath(filePath, FParseOptions.allTrue);

            //and create scriptable objects
            if (string.IsNullOrEmpty(result.DefaultFileContent) == false)
                CreateScriptableObjects(result);
        }

        [MenuItem("Tools/redd096/CSV Importer/Examples/Example Download and Create All in One")]
        static void ExampleImportAndCreateAllInOne()
        {
            //download csv and use it to create scriptable objects
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSkP8ujm2voXc1L34U4LTbhWybChPVCfSUk3_Z-74Lpvr8h-ZFoQ8DwVIvGXA2IQmPf9Tychxt6RGbi/pub?output=csv";
            CsvImporter.ReadCsvFromUrl(url, FParseOptions.allTrue, CreateScriptableObjects);
        }

        /// <summary>
        /// Example of create scriptable objects from parsed file
        /// </summary>
        /// <param name="result"></param>
        private static async void CreateScriptableObjects(FParseResult result)
        {
            //directory path relative to project
            string directoryScriptableObjects = "Assets/Example/ScriptableObjects";

            //create a scriptable object for every row
            for (int row = 0; row < 8; row++)// for (int row = 0; row < result.Rows.Count; row++)
            {
                //ignore first row, is used only to give a name to every column
                if (row == 0)
                    continue;

                //use first column also to set asset name
                string fileName = result.GetCellContent("Pokemon", row);
                string filePath = Path.Combine(directoryScriptableObjects, fileName + ".asset");

                //load if already created, or create a new scriptable object with fileName
                var data = LoadAssetsUtilities.GetOrCreateAsset<ExampleScriptableObject>(filePath);

                //set values
                data.Name = result.GetCellContent("Pokemon", row);                                                          //string
                data.Life = int.TryParse(result.GetCellContent("Vita", row), out int n) ? n : 0;                            //int
                System.Enum.TryParse(result.GetCellContent("Tipo", row), ignoreCase: true, out data.Type);                  //enum
                data.ExampleArrayString = result.GetCellArrayContent("Esempio array stringhe per utilities Unity", row);    //array string

                //array string to array int
                string[] arrayString = result.GetCellArrayContent("Esempio array int per utilities Unity", row);
                int[] arrayInt = arrayString.Select(s => int.TryParse(s, out int n) ? n : 0).ToArray();
                data.ExampleArrayInt = arrayInt;

                //set dirty to save
                EditorUtility.SetDirty(data);

                //show progress bar and delay if created too much files
                EditorUtility.DisplayProgressBar("Creating Scriptable Objects", $"Creating... {row}/{result.Rows.Count}", (float)row / result.Rows.Count);
                if (row % 20 == 0)
                    await Task.Delay((int)(Time.deltaTime * 1000));
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Example of create scriptable objects from parsed file, by using Helper function
        /// </summary>
        /// <param name="result"></param>
        private static async void ExampleToCreateWithHelper(FParseResult result)
        {
            //directory path relative to project
            string directoryScriptableObjects = "Assets/Example/ScriptableObjects";

            await EditorHelper.LoadOrCreateOneAssetForEveryRow<ExampleScriptableObject>(result, true, directoryScriptableObjects, SetFileNameAndExtension, SetAssetVariables);
        }

        private static string SetFileNameAndExtension(FParseResult result, int row)
        {
            return result.GetCellContent("Pokemon", row) + ".asset";
        }

        private static void SetAssetVariables(ExampleScriptableObject data, FParseResult result, int row, string filePath)
        {
            //set values
            data.Name = result.GetCellContent("Pokemon", row);                                                          //string
            data.Life = int.TryParse(result.GetCellContent("Vita", row), out int n) ? n : 0;                            //int
            System.Enum.TryParse(result.GetCellContent("Tipo", row), ignoreCase: true, out data.Type);                  //enum
            data.ExampleArrayString = result.GetCellArrayContent("Esempio array stringhe per utilities Unity", row);    //array string

            //array string to array int
            string[] arrayString = result.GetCellArrayContent("Esempio array int per utilities Unity", row);
            int[] arrayInt = arrayString.Select(s => int.TryParse(s, out int n) ? n : 0).ToArray();
            data.ExampleArrayInt = arrayInt;
        }
    }
}
#endif