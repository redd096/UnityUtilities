using UnityEngine.Networking;
using UnityEngine;

namespace redd096.CsvImporter
{
    /// <summary>
    /// Functions to download and read files .csv
    /// </summary>
    public static class CsvImporter
    {
        /// <summary>
        /// Download csv from url and save in directory path
        /// </summary>
        /// <param name="url"></param>
        /// <param name="directoryPath"></param>
        /// <param name="fileName">file name and extension</param>
        public static void DownloadCsv(string url, string directoryPath, string fileName)
        {
            ManageDownload.DownloadFile(url, (op, webRequest) =>
            {
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    ManageFile.SaveFile(directoryPath, fileName, webRequest.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Error downloading file");
                }
            });
        }

        /// <summary>
        /// Read csv content and return parsed file
        /// </summary>
        /// <param name="csvContent"></param>
        public static FParseResult ReadCsv(string csvContent, FParseOptions parseOptions)
        {
            return ManageParse.Parse(csvContent, parseOptions);
        }

        /// <summary>
        /// Find csv at path, then read csv content and return parsed file
        /// </summary>
        /// <param name="csvFilePath"></param>
        public static FParseResult ReadCsvAtPath(string csvFilePath, FParseOptions parseOptions)
        {
            return ReadCsv(ManageFile.LoadFile(csvFilePath), parseOptions);
        }

        /// <summary>
        /// Download csv file, then read csv content and call event by passing parsed file as parameter
        /// </summary>
        /// <param name="url"></param>
        public static void ReadCsvFromUrl(string url, FParseOptions parseOptions, System.Action<FParseResult> onDownloadedFile)
        {
            ManageDownload.DownloadFile(url, (op, webRequest) =>
            {
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onDownloadedFile?.Invoke(ReadCsv(webRequest.downloadHandler.text, parseOptions));
                }
                else
                {
                    Debug.LogError("Error downloading file");
                }
            });
        }
    }
}