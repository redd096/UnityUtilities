namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;

    [System.Serializable]
    public class ClassToSave
    {
    }

    //TODO 
    //dovrà salvare la dictionary del World
    //UnityEngine.SceneManagement.SceneManager.GetActiveScene().name -> come nome del salvataggio utilizziamo il nome della scena + la wave raggiunta
    //quando carica la scena, il level manager dovrà checkare il Game Manager per vedere se è stata caricata la scena 0 e quindi eseguire tutto come al solito, o è stata caricata un'altra scena ed in quel caso si deve chiamare SetWave
    //quando si chiama WaveManager.EndWave(), deve salvare la dictionary (con il nome scritto sopra), così da poter ricaricare con le celle distrutte -> se si sta passando ad un livello già finito in precedenza, si sovrascrive
    //NB CHE QUINDI SERVE UNA FUNZIONE NUOVA NEL GAME MANAGER, PER CARICARE LA SCENA MA PRIMA SETTARSI UN VALORE CHE DICE QUALE ONDATA VOGLIAMO AVVIARE (probabilmente basta una funzione chiamata dal bottone prima di LoadScene, che semplicemente setta una variabile int in GameManager)

    [AddComponentMenu("redd096/Singletons/Save & Load System")]
    public class SaveLoadSystem : Singleton<SaveLoadSystem>
    {
        [Header("Data Directory")]
        [SerializeField] bool usePersistentDataPath = true;
        [SerializeField] string directory = "Saves/";

        string pathDirectory => usePersistentDataPath ?
            Application.persistentDataPath + directory :    //return persistent data path + directory path
            directory;                                      //return only directory path


        /// <summary>
        /// Get path to the file (directory path + name of the file (key) + format (.json))
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <returns></returns>
        public static string GetPathFile(string key)
        {
            //directory path + name of the file (key) + format (.json)
            return Path.Combine(instance.pathDirectory, key, ".json");
        }

        /// <summary>
        /// Save class in directory/key.json
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <param name="value">Value to save</param>
        public static void Save(string key, ClassToSave value)
        {
            //if there is no directory, create it
            if (Directory.Exists(instance.pathDirectory) == false)
            {
                Directory.CreateDirectory(instance.pathDirectory);
            }

            //value to json, then save file
            string jsonValue = JsonUtility.ToJson(value);
            File.WriteAllText(GetPathFile(key), jsonValue);
        }

        /// <summary>
        /// Load class from directory/key.json
        /// </summary>
        /// <param name="key">name of the file</param>
        public static ClassToSave Load(string key)
        {
            //if there is no file, return null
            if (File.Exists(GetPathFile(key)) == false)
            {
                Debug.Log("There is no saved at " + GetPathFile(key));
                return null;
            }

            //load file, then json to value
            string jsonValue = File.ReadAllText(GetPathFile(key));
            return JsonUtility.FromJson<ClassToSave>(jsonValue);
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="key">Name of the file</param>
        public static void DeleteData(string key)
        {
            //delete file
            File.Delete(GetPathFile(key));
        }
    }
}