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

        public void Serialize() => SaveManager.Save("Example", JsonUtility.ToJson(this));
        public ExampleClassToSave Deserialize() => JsonUtility.FromJson<ExampleClassToSave>(SaveManager.Load("Example"));
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

        [Header("Optimization")]
        [SerializeField] bool saveAsync = true;
        [SerializeField] bool usePreload = true;

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

        //used when "Use Preload" is enabled -> key: file name without extension, value: json
        private Dictionary<string, string> savesJson = new Dictionary<string, string>();
        //used when create one single file with more variables -> key: file name without extension, value: another dictionary with key: variableName, value: json
        private Dictionary<string, Dictionary<string, string>> filesWithMoreVariables = new Dictionary<string, Dictionary<string, string>>();
        //system to use for save and load
        private ISaveLoadSystem saveLoadSystem;

        protected override void Awake()
        {
            base.Awake();

            //if this is the instance and load everything at start
            if (instance == this)
            {
                //set save load system based on platform
#if UNITY_STEAM
#elif UNITY_STANDALONE
                saveLoadSystem = new SaveLoadSystem_PC();
#elif UNITY_IOS || UNITY_ANDROID
#elif UNITY_GAMECORE
#elif UNITY_PS4
#elif UNITY_PS5
#elif UNITY_SWITCH
#endif

                //preload every file
                if (usePreload)
                    saveLoadSystem.Preload();
            }
        }

        /// <summary>
        /// Called from Preload, to set dictionary (used when "Use Preload" is enabled)
        /// </summary>
        public void FinishPreload(Dictionary<string, string> jsons)
        {
            savesJson = new Dictionary<string, string>(jsons);

            //add also file with more variables to their dictionary
            foreach (string fileName in savesJson.Keys) LoadFromDisk(fileName);
        }

        #region custom - single file with more variables

        /// <summary>
        /// Save value in a dictionary. Use SaveOnDisk to save on disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="json">variable value</param>
        public static void Save(string fileName, string key, string json)
        {
            //if there is already a file, set or add variable
            if (instance.filesWithMoreVariables.ContainsKey(fileName))
            {
                if (instance.filesWithMoreVariables[fileName].ContainsKey(key))
                    instance.filesWithMoreVariables[fileName][key] = json;
                else
                    instance.filesWithMoreVariables[fileName].Add(key, json);
            }
            //else add file name and variable
            else
            {
                instance.filesWithMoreVariables.Add(fileName, new Dictionary<string, string>());
                instance.filesWithMoreVariables[fileName].Add(key, json);
            }
        }

        /// <summary>
        /// Save on disk every file setted in dictionary
        /// </summary>
        public static void SaveOnDisk()
        {
            //save every file to disk
            foreach (string fileName in instance.filesWithMoreVariables.Keys)
            {
                SaveOnDisk(fileName);
            }
        }

        /// <summary>
        /// Save file on disk with every variable setted in dictionary
        /// </summary>
        public static void SaveOnDisk(string fileName)
        {
            System.Text.StringBuilder fileString = new System.Text.StringBuilder("redd096File with more variables\n");
            if (instance.filesWithMoreVariables.ContainsKey(fileName))
            {
                //create a string "1stVarName\n1stJson\n2ndVarName\n2ndJson\n..."
                foreach (string variableName in instance.filesWithMoreVariables[fileName].Keys)
                {
                    fileString.Append(variableName).Append("\n").Append(instance.filesWithMoreVariables[fileName][variableName]).Append("\n");
                }
            }

            //save
            Save(fileName, fileString.ToString());
        }

        /// <summary>
        /// Load variable from dictionary. Use LoadFromDisk or enable "Use Preload" to fill dictionary
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <returns></returns>
        public static string Load(string fileName, string key)
        {
            if (instance.filesWithMoreVariables.ContainsKey(fileName))
            {
                //get variable from dictionary
                if (instance.filesWithMoreVariables[fileName].ContainsKey(key))
                    return instance.filesWithMoreVariables[fileName][key];
                else if (instance.ShowDebugLogs)
                    Debug.Log("Variable " + key + " not found in: " + instance.saveLoadSystem.GetPathFile(fileName));
            }
            else
                Debug.Log("File not found: " + instance.saveLoadSystem.GetPathFile(fileName));

            return default;
        }

        /// <summary>
        /// Load from disk and set in dictionary
        /// </summary>
        /// <param name="fileName"></param>
        public static void LoadFromDisk(string fileName)
        {
            //load from disk
            string fileString = Load(fileName);
            if (fileString == null || fileString.StartsWith("redd096File with more variables") == false)
            {
                if (instance.ShowDebugLogs)
                    Debug.Log("Incorrect file: " + instance.saveLoadSystem.GetPathFile(fileName));

                return;
            }

            //add file to dictionary
            if (instance.filesWithMoreVariables.ContainsKey(fileName) == false)
                instance.filesWithMoreVariables.Add(fileName, new Dictionary<string, string>());

            string[] lines = fileString.Split('\n');
            for (int i = 2; i < lines.Length; i += 2)   //skip 0 because is our custom string, then move by 2 and read variable name and value
            {
                //first line is variable name
                if (instance.filesWithMoreVariables[fileName].ContainsKey(lines[i - 1]) == false)
                    instance.filesWithMoreVariables[fileName].Add(lines[i - 1], "");

                //and second line is json
                if (i < lines.Length)
                    instance.filesWithMoreVariables[fileName][lines[i - 1]] = lines[i];
            }
        }

        /// <summary>
        /// Delete a file (same as DeleteKey)
        /// </summary>
        /// <param name="fileName"></param>
        public static void DeleteFile(string fileName)
        {
            DeleteData(fileName);
        }

        /// <summary>
        /// Delete a variable from dictionary
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        public static void DeleteVariable(string fileName, string key)
        {
            if (instance.filesWithMoreVariables.ContainsKey(fileName))
            {
                //remove variable from dictionary
                if (instance.filesWithMoreVariables[fileName].ContainsKey(key))
                    instance.filesWithMoreVariables[fileName].Remove(key);
                else if (instance.ShowDebugLogs)
                    Debug.Log("Variable " + key + " not found in: " + instance.saveLoadSystem.GetPathFile(fileName));
            }
            else if (instance.ShowDebugLogs)
                Debug.Log("Save file not found: " + instance.saveLoadSystem.GetPathFile(fileName));
        }

        #endregion

        #region custom

        /// <summary>
        /// Save in directory/key.json
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="json">file value</param>
        public static void Save(string key, string json)
        {
            //add to dictionary
            if (instance.savesJson.ContainsKey(key))
                instance.savesJson[key] = json;
            else
                instance.savesJson.Add(key, json);

            //save async or normal
            if (instance.saveAsync)
            {
                _ = instance.saveLoadSystem.SaveAsync(key, json);
            }
            else
            {
                instance.saveLoadSystem.Save(key, json);
            }
        }

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static string Load(string key)
        {
            //load from dictionary if use preload
            if (instance.usePreload)
            {
                if (instance.savesJson.ContainsKey(key))
                    return instance.savesJson[key];

                if (instance.ShowDebugLogs)
                    Debug.Log("Save file not found: " + instance.saveLoadSystem.GetPathFile(key));
            }
            //else load normally from file
            else
            {
                return instance.saveLoadSystem.Load(key);
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

            //delete also from dictionaries
            if (instance.savesJson.ContainsKey(key))
                instance.savesJson.Remove(key);
            if (instance.filesWithMoreVariables.ContainsKey(key))
                instance.filesWithMoreVariables.Remove(key);
        }

        #endregion

        #region playerPrefs - single file with more playerPrefs

        /// <summary>
        /// Save value in a dictionary. Use SaveOnDisk to save on disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="value"></param>
        /// <param name="automaticSaveOnDisk">call automatically SaveOnDisk</param>
        public static void SetInt(string fileName, string key, int value, bool automaticSaveOnDisk = true)
        {
            Save(fileName, key, value.ToString());
            if (automaticSaveOnDisk) SaveOnDisk(fileName);
        }

        /// <summary>
        /// Load value from dictionary. If there is no variable, return defaultValue
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt(string fileName, string key, int defaultValue)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            //check if there is a file saved to return
            if (HasKey(fileName, key))
                return int.Parse(Load(fileName, key));

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value from dictionary
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <returns></returns>
        public static int GetInt(string fileName, string key)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            return int.Parse(Load(fileName, key));
        }

        /// <summary>
        /// Save value in a dictionary. Use SaveOnDisk to save on disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="value"></param>
        /// <param name="automaticSaveOnDisk">call automatically SaveOnDisk</param>
        public static void SetFloat(string fileName, string key, float value, bool automaticSaveOnDisk = true)
        {
            Save(fileName, key, value.ToString());
            if (automaticSaveOnDisk) SaveOnDisk(fileName);
        }

        /// <summary>
        /// Load value from dictionary. If there is no variable, return defaultValue
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetFloat(string fileName, string key, float defaultValue)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            //check if there is a file saved to return
            if (HasKey(fileName, key))
                return float.Parse(Load(fileName, key));

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value from dictionary
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <returns></returns>
        public static float GetFloat(string fileName, string key)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            return float.Parse(Load(fileName, key));
        }

        /// <summary>
        /// Save value in a dictionary. Use SaveOnDisk to save on disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="value"></param>
        /// <param name="automaticSaveOnDisk">call automatically SaveOnDisk</param>
        public static void SetString(string fileName, string key, string value, bool automaticSaveOnDisk = true)
        {
            Save(fileName, key, value);
            if (automaticSaveOnDisk) SaveOnDisk(fileName);
        }

        /// <summary>
        /// Load value from dictionary. If there is no variable, return defaultValue
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString(string fileName, string key, string defaultValue)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            //check if there is a file saved to return
            if (HasKey(fileName, key))
                return Load(fileName, key);

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value from dictionary
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <returns></returns>
        public static string GetStringVar(string fileName, string key)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            return Load(fileName, key);
        }

        /// <summary>
        /// Save value in a dictionary. Use SaveOnDisk to save on disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="value"></param>
        /// <param name="automaticSaveOnDisk">call automatically SaveOnDisk</param>
        public static void SetBool(string fileName, string key, bool value, bool automaticSaveOnDisk = true)
        {
            Save(fileName, key, value.ToString());
            if (automaticSaveOnDisk) SaveOnDisk(fileName);
        }

        /// <summary>
        /// Load value from dictionary. If there is no variable, return defaultValue
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetBool(string fileName, string key, bool defaultValue)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            //check if there is a file saved to return
            if (HasKey(fileName, key))
                return bool.Parse(Load(fileName, key));

            //else return default value
            return defaultValue;
        }

        /// <summary>
        /// Load value from dictionary
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <returns></returns>
        public static bool GetBool(string fileName, string key)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            return bool.Parse(Load(fileName, key));
        }

        /// <summary>
        /// Check if there is a file with this name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool HasFile(string fileName)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            //return if there is a file with this name
            return instance.filesWithMoreVariables.ContainsKey(fileName);
        }

        /// <summary>
        /// Check if there is a variable with this name in this file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        /// <returns></returns>
        public static bool HasKey(string fileName, string key)
        {
            //load and add to dictionary, if don't use preload
            if (instance.usePreload == false)
                LoadFromDisk(fileName);

            //check there is this file and has this variable
            if (instance.filesWithMoreVariables.ContainsKey(fileName) && instance.filesWithMoreVariables[fileName].ContainsKey(key))
                return true;

            return false;
        }

        /// <summary>
        /// Delete a variable from dictionary
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key">variable name</param>
        public static void DeleteKey(string fileName, string key)
        {
            DeleteVariable(fileName, key);
        }

        /// <summary>
        /// Save on disk every file setted in dictionary (same as SaveOnDisk())
        /// </summary>
        public static void Save()
        {
            SaveOnDisk();
        }

        /// <summary>
        /// Save file on disk with every variable setted in dictionary (same as SaveOnDisk(fileName))
        /// </summary>
        public static void Save(string fileName)
        {
            SaveOnDisk(fileName);
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
            Save(key, value.ToString());
        }

        /// <summary>
        /// Load value. If there is no file, return defaultValue
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt(string key, int defaultValue)
        {
            //check if there is a file saved to return
            if (HasKey(key))
                return int.Parse(Load(key));

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
            return int.Parse(Load(key));
        }

        /// <summary>
        /// Save value
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public static void SetFloat(string key, float value)
        {
            Save(key, value.ToString());
        }

        /// <summary>
        /// Load value. If there is no file, return defaultValue
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetFloat(string key, float defaultValue)
        {
            //check if there is a file saved to return
            if (HasKey(key))
                return float.Parse(Load(key));

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
            return float.Parse(Load(key));
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
            //check if there is a file saved to return
            if (HasKey(key))
                return Load(key);

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
            return Load(key);
        }

        /// <summary>
        /// Save value
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        public static void SetBool(string key, bool value)
        {
            Save(key, value.ToString());
        }

        /// <summary>
        /// Load value. If there is no file, return defaultValue
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetBool(string key, bool defaultValue)
        {
            //check if there is a file saved to return
            if (HasKey(key))
                return bool.Parse(Load(key));

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
            return bool.Parse(Load(key));
        }

        /// <summary>
        /// Check if there is a file with this name
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            //if use preload, check if there is inside dictionary
            if (instance.usePreload)
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

            //clear also dictionaries
            instance.savesJson.Clear();
            instance.filesWithMoreVariables.Clear();
        }

        #endregion
    }
}