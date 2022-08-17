using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

namespace redd096
{
    public class SaveLoadJson_PC : ISaveLoad
    {
        /// <summary>
        /// Get path to the file (directory path + name of the file (key) + format (.json))
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public string GetPathFile(string key)
        {
            //directory path + name of the file (key) + format (.json)
            return Path.Combine(SaveManager.instance.PathDirectory, key + ".json");
        }

        /// <summary>
        /// Load every file and call SaveManager to update dictionary
        /// </summary>
        /// <returns></returns>
        public void Preload()
        {
            Dictionary<string, string> jsons = new Dictionary<string, string>();

            //get all files in directory
            if (Directory.Exists(SaveManager.instance.PathDirectory))
            {
                foreach (string filePath in Directory.GetFiles(SaveManager.instance.PathDirectory, "*", SearchOption.AllDirectories))
                {
                    //and save in dictionary (key, json), where key is file name without extension
                    string key = Path.GetFileNameWithoutExtension(filePath);
                    jsons.Add(key, GetJson(key));
                }
            }

            //update dictionary in SaveManager, used when use preload is true
            SaveManager.instance.FinishPreload(jsons);
        }

        /// <summary>
        /// Save in directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public bool Save<T>(string key, T value)
        {
            try
            {
                //if there is no directory, create it
                if (Directory.Exists(SaveManager.instance.PathDirectory) == false)
                {
                    Directory.CreateDirectory(SaveManager.instance.PathDirectory);
                }

                //value to json, then save file
                string jsonValue = JsonUtility.ToJson(value);
                File.WriteAllText(GetPathFile(key), jsonValue);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public T Load<T>(string key)
        {
            try
            {
                //if there is no file, return null
                if (File.Exists(GetPathFile(key)) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Save file not found: " + GetPathFile(key));

                    return default;
                }

                //load file, then json to value
                string jsonValue = File.ReadAllText(GetPathFile(key));
                return JsonUtility.FromJson<T>(jsonValue);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="type">type of the value to load</param>
        /// <returns></returns>
        public object Load(string key, System.Type type)
        {
            try
            {
                //if there is no file, return null
                if (File.Exists(GetPathFile(key)) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Save file not found: " + GetPathFile(key));

                    return null;
                }

                //load file, then json to value
                string jsonValue = File.ReadAllText(GetPathFile(key));
                return JsonUtility.FromJson(jsonValue, type);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Save async in directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public async Task<bool> SaveAsync<T>(string key, T value)
        {
            try
            {
                //if there is no directory, create it
                if (Directory.Exists(SaveManager.instance.PathDirectory) == false)
                {
                    Directory.CreateDirectory(SaveManager.instance.PathDirectory);
                }

                //value to json, then save file
                string jsonValue = JsonUtility.ToJson(value);
                await File.WriteAllTextAsync(GetPathFile(key), jsonValue);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Load async from directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public async Task<T> LoadAsync<T>(string key)
        {
            try
            {
                //if there is no file, return null
                if (File.Exists(GetPathFile(key)) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Save file not found: " + GetPathFile(key));

                    return default;
                }

                //load file, then json to value
                string jsonValue = await File.ReadAllTextAsync(GetPathFile(key));
                return JsonUtility.FromJson<T>(jsonValue);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="type">type of the value to load</param>
        /// <returns></returns>
        public async Task<object> LoadAsync(string key, System.Type type)
        {
            try
            {
                //if there is no file, return null
                if (File.Exists(GetPathFile(key)) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Save file not found: " + GetPathFile(key));

                    return null;
                }

                //load file, then json to value
                string jsonValue = await File.ReadAllTextAsync(GetPathFile(key));
                return JsonUtility.FromJson(jsonValue, type);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Check if file is saved on disk
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public bool FileExists(string key)
        {
            return File.Exists(GetPathFile(key));
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public bool DeleteData(string key)
        {
            try
            {
                //check there is a file
                if (File.Exists(GetPathFile(key)) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Save file not found: " + GetPathFile(key));

                    return false;
                }

                //delete file
                File.Delete(GetPathFile(key));
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Delete directory with every file
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            try
            {
                //check there is a directory
                if (Directory.Exists(SaveManager.instance.PathDirectory) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Directory not found: " + SaveManager.instance.PathDirectory);

                    return false;
                }

                //delete directory
                Directory.Delete(SaveManager.instance.PathDirectory, true);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        #region private API

        /// <summary>
        /// Load file as a Json. Same as call Load without the last line (JsonUtility.FromJson)
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        private string GetJson(string key)
        {
            try
            {
                //if there is no file, return null
                if (File.Exists(GetPathFile(key)) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Save file not found: " + GetPathFile(key));

                    return default;
                }

                //load file
                return File.ReadAllText(GetPathFile(key));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// Load async file as a Json. Same as call LoadAsync without the last line (JsonUtility.FromJson)
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        private async Task<string> GetJsonAsync(string key)
        {
            try
            {
                //if there is no file, return null
                if (File.Exists(GetPathFile(key)) == false)
                {
                    if (SaveManager.instance.ShowDebugLogs)
                        Debug.Log("Save file not found: " + GetPathFile(key));

                    return default;
                }

                //load file
                return await File.ReadAllTextAsync(GetPathFile(key));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }

        #endregion
    }
}