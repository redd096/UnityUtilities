using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace redd096
{
    #region custom editor

#if UNITY_EDITOR

    using UnityEditor;

    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SaveManager saveLoad = target as SaveManager;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(5);

            //header
            EditorGUILayout.LabelField("Path Directory:");

            EditorGUILayout.Space(-15);

            //show path directory
            EditorGUILayout.SelectableLabel(saveLoad.PathDirectory);

            EditorGUILayout.Space(10);

            //button
            if (GUILayout.Button("Delete Saves"))
            {
                DeleteAll();
            }

            EditorGUILayout.EndVertical();
        }

        void DeleteAll()
        {
            //use this SaveLoadSystem, cause instance is not setted
            SaveManager saveLoadSystem = target as SaveManager;
            if (saveLoadSystem == null)
                return;

            //check there is a directory
            if (Directory.Exists(saveLoadSystem.PathDirectory) == false)
            {
                if (saveLoadSystem.ShowDebugLogs)
                    Debug.Log("Directory not found: " + saveLoadSystem.PathDirectory);

                return;
            }

            //delete directory
            Directory.Delete(saveLoadSystem.PathDirectory, true);
        }
    }

#endif

    #endregion

    #region example save class

    [System.Serializable]
    public class ExampleClassToSave
    {
        public int test;
        public string[] testArray;

        public ExampleClassToSave(int test, string[] testArray)
        {
            this.test = test;
            this.testArray = testArray;
        }
    }

    #endregion

    enum SaveFolder
    {
        persistentDataPath, gameFolder, nothing
    }

    [AddComponentMenu("redd096/Singletons/Save Manager")]
    [DefaultExecutionOrder(-200)]
    public class SaveManager : Singleton<SaveManager>
    {
        [Header("Data Directory")]
        [SerializeField] SaveFolder saveFolder = SaveFolder.persistentDataPath;
        [SerializeField] string directoryName = "Saves";

        [Header("Optimization - if LoadAsync is true, is recommended to set true also LoadEverythingAtStart")]
        [SerializeField] bool saveAsync = true;
        [SerializeField] bool loadAsync = true;

        [Header("Debug Mode")]
        public bool ShowDebugLogs = false;

        public string PathDirectory
        {
            get
            {
                if (saveFolder == SaveFolder.persistentDataPath)
                    return Path.Combine(Application.persistentDataPath, directoryName);     //return persistent data path + directory path
                else if (saveFolder == SaveFolder.gameFolder)
                    return Path.Combine(Application.dataPath, directoryName);               //return game folder path + directory path
                else
                    return directoryName;                                                   //return only directory path
            }
        }

        //key: file name without extension, value: json
        private Dictionary<string, string> savesJson = new Dictionary<string, string>();
        private ISaveLoad saveLoadSystem;

        protected override void Awake()
        {
            base.Awake();

            //if this is the instance and load everything at start
            if (instance == this)
            {
                //set save load system based on platform
#if UNITY_STEAM
#elif UNITY_STANDALONE
                saveLoadSystem = new SaveLoadJson_PC();
#elif UNITY_IOS || UNITY_ANDROID
#elif UNITY_GAMECORE
#elif UNITY_PS4
#elif UNITY_PS5
#elif UNITY_SWITCH
#endif

                //preload every file
                saveLoadSystem.Preload();
            }
        }

        /// <summary>
        /// Called from Preload, to set dictionary (used when load async)
        /// </summary>
        public void FinishPreload(Dictionary<string, string> jsons)
        {
            savesJson = new Dictionary<string, string>(jsons);
        }

        #region custom

        /// <summary>
        /// Save in directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public static void Save<T>(string key, T value)
        {
            //add to dictionary
            if (instance.savesJson.ContainsKey(key))
                instance.savesJson[key] = JsonUtility.ToJson(value);
            else
                instance.savesJson.Add(key, JsonUtility.ToJson(value));

            //save async or normal
            if (instance.saveAsync)
            {
                _ = instance.saveLoadSystem.SaveAsync(key, value);
            }
            else
            {
                instance.saveLoadSystem.Save(key, value);
            }
        }

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static T Load<T>(string key)
        {
            //load from dictionary if async
            if (instance.loadAsync)
            {
                if (instance.savesJson.ContainsKey(key))
                    return JsonUtility.FromJson<T>(instance.savesJson[key]);

                if (instance.ShowDebugLogs)
                    Debug.Log("Save file not found: " + instance.saveLoadSystem.GetPathFile(key));
            }
            //else load normally from file
            else
            {
                return instance.saveLoadSystem.Load<T>(key);
            }

            return default;
        }

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="type">type of the value to load</param>
        /// <returns></returns>
        public static object Load(string key, System.Type type)
        {
            //load from dictionary if async
            if (instance.loadAsync)
            {
                if (instance.savesJson.ContainsKey(key))
                    return JsonUtility.FromJson(instance.savesJson[key], type);

                if (instance.ShowDebugLogs)
                    Debug.Log("Save file not found: " + instance.saveLoadSystem.GetPathFile(key));
            }
            //else load normally from file
            else
            {
                return instance.saveLoadSystem.Load(key, type);
            }

            return default;
        }

        /// <summary>
        /// Delete a file (same as DeleteKey)
        /// </summary>
        /// <param name="key">file name</param>
        public static void DeleteData(string key)
        {
            instance.saveLoadSystem.DeleteData(key);
        }

        #endregion

        #region playerPrefs

        /// <summary>
        /// Save value
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public static void SetInt(string key, int value)
        {
            Save(key, value);
        }

        /// <summary>
        /// Load value. If there is no file, return defaultValue
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt(string key, int defaultValue)
        {
            //if load async, check if there is already a file in dictionary to return
            if (instance.loadAsync && instance.savesJson.ContainsKey(key))
            {
                return JsonUtility.FromJson<int>(instance.savesJson[key]);
            }
            //else, check if there is a file saved to return
            else if (instance.loadAsync == false && instance.saveLoadSystem.FileExists(key))
            {
                return instance.saveLoadSystem.Load<int>(key);
            }

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static int GetInt(string key)
        {
            return Load<int>(key);
        }

        /// <summary>
        /// Save value
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public static void SetFloat(string key, float value)
        {
            Save(key, value);
        }

        /// <summary>
        /// Load value. If there is no file, return defaultValue
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetFloat(string key, float defaultValue)
        {
            //if load async, check if there is already a file in dictionary to return
            if (instance.loadAsync && instance.savesJson.ContainsKey(key))
            {
                return JsonUtility.FromJson<float>(instance.savesJson[key]);
            }
            //else, check if there is a file saved to return
            else if (instance.loadAsync == false && instance.saveLoadSystem.FileExists(key))
            {
                return instance.saveLoadSystem.Load<float>(key);
            }

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static float GetFloat(string key)
        {
            return Load<float>(key);
        }

        /// <summary>
        /// Save value
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public static void SetString(string key, string value)
        {
            Save(key, value);
        }

        /// <summary>
        /// Load value. If there is no file, return defaultValue
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString(string key, string defaultValue)
        {
            //if load async, check if there is already a file in dictionary to return
            if (instance.loadAsync && instance.savesJson.ContainsKey(key))
            {
                return JsonUtility.FromJson<string>(instance.savesJson[key]);
            }
            //else, check if there is a file saved to return
            else if (instance.loadAsync == false && instance.saveLoadSystem.FileExists(key))
            {
                return instance.saveLoadSystem.Load<string>(key);
            }

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return Load<string>(key);
        }

        /// <summary>
        /// Save value
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public static void SetBool(string key, bool value)
        {
            Save(key, value);
        }

        /// <summary>
        /// Load value. If there is no file, return defaultValue
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetBool(string key, bool defaultValue)
        {
            //if load async, check if there is already a file in dictionary to return
            if (instance.loadAsync && instance.savesJson.ContainsKey(key))
            {
                return JsonUtility.FromJson<bool>(instance.savesJson[key]);
            }
            //else, check if there is a file saved to return
            else if (instance.loadAsync == false && instance.saveLoadSystem.FileExists(key))
            {
                return instance.saveLoadSystem.Load<bool>(key);
            }

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static bool GetBool(string key)
        {
            return Load<bool>(key);
        }

        /// <summary>
        /// Check if there is a file with this name
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            //if load async, check if there is inside dictionary
            if (instance.loadAsync)
            {
                return instance.savesJson.ContainsKey(key);
            }
            //else, check if exists this file
            else
            {
                return instance.saveLoadSystem.FileExists(key);
            }
        }

        /// <summary>
        /// If there is a file with this name, delete it
        /// </summary>
        /// <param name="key">file name</param>
        public static void DeleteKey(string key)
        {
            DeleteData(key);
        }

        /// <summary>
        /// Delete every file
        /// </summary>
        public static void DeleteAll()
        {
            instance.saveLoadSystem.DeleteAll();
        }

        #endregion
    }
}