#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace redd096.CsvImporter
{
    public class ExampleSimpleCsvImporterAndReader
    {
        [MenuItem("Tools/redd096/CSV Importer/Example Import Csv")]
        static void ExampleImportCsv()
        {
            CsvImporter.DownloadCsv(
                url: "https://docs.google.com/spreadsheets/d/e/2PACX-1vSkP8ujm2voXc1L34U4LTbhWybChPVCfSUk3_Z-74Lpvr8h-ZFoQ8DwVIvGXA2IQmPf9Tychxt6RGbi/pub?output=csv",
                directoryPath: Path.Combine(Application.dataPath, "Example/CSV"),
                fileName: "DownloadedFile.csv");
        }
    }
}
#endif