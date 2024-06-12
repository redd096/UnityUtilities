using UnityEngine;
using UnityEngine.Networking;

namespace redd096.CsvImporter
{
    public static class ManageDownload
    {
        /// <summary>
        /// Start Download from link
        /// </summary>
        /// <returns></returns>
        public static void DownloadFile(string link, System.Action<AsyncOperation, UnityWebRequest> onComplete)
        {
            //UnityWebRequest replace old WWW
            UnityWebRequest webRequest = UnityWebRequest.Get(link);
            UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();

            asyncOperation.completed += (x) => onComplete?.Invoke(x, webRequest);

            //show download progress bar (only in editor)
#if UNITY_EDITOR
            ShowProgressBar(asyncOperation);
#endif
        }

#if UNITY_EDITOR
        private static async void ShowProgressBar(UnityWebRequestAsyncOperation asyncOperation)
        {
            while (asyncOperation.isDone == false)
            {
                UnityEditor.EditorUtility.DisplayProgressBar("Download file", $"File download... {(int)(asyncOperation.progress * 100)}/100", asyncOperation.progress);
                await System.Threading.Tasks.Task.Delay((int)(Time.deltaTime * 1000));
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }
#endif

        /// <summary>
        /// If downloading, Abort it
        /// </summary>
        public static void AbortDownload(UnityWebRequest www)
        {
            if (www != null)
            {
                www.Abort();
            }
        }
    }
}