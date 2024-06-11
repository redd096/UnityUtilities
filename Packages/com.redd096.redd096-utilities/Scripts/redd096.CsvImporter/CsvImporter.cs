using UnityEngine.Networking;

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
            });
        }
    }
}