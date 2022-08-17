using System.Threading.Tasks;

namespace redd096
{
    public interface ISaveLoadSystem
    {
        /// <summary>
        /// Get path to the file (directory path + name of the file (key) + format (.json))
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        string GetPathFile(string key);

        /// <summary>
        /// Load every file and call SaveManager to update dictionary
        /// </summary>
        /// <returns></returns>
        void Preload();

        /// <summary>
        /// Save in directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        bool Save<T>(string key, T value);

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <returns></returns>
        T Load<T>(string key);

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="type">type of the value to load</param>
        /// <returns></returns>
        object Load(string key, System.Type type);

        /// <summary>
        /// Save async in directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <param name="value"></param>
        Task<bool> SaveAsync<T>(string key, T value);

        /// <summary>
        /// Load async from directory/key.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">file name</param>
        /// <returns></returns>
        Task<T> LoadAsync<T>(string key);

        /// <summary>
        /// Load from directory/key.json
        /// </summary>
        /// <param name="key">file name</param>
        /// <param name="type">type of the value to load</param>
        /// <returns></returns>
        Task<object> LoadAsync(string key, System.Type type);

        /// <summary>
        /// Check if file is saved on disk
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        bool FileExists(string key);

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="key">file name</param>
        /// <returns></returns>
        bool DeleteData(string key);

        /// <summary>
        /// Delete every file
        /// </summary>
        /// <returns></returns>
        bool DeleteAll();
    }
}