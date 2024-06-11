using System.IO;
using UnityEngine;

namespace redd096.CsvImporter
{
    public static class ManageFile
    {
        /// <summary>
        /// Save file in directory
        /// </summary>
        public static void SaveFile(string directoryPath, string fileName, string fileValue)
        {
            //if there is no directory, create it
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(Path.Combine(directoryPath, fileName), fileValue);
        }

        /// <summary>
        /// Load File in directory
        /// </summary>
        public static string LoadFile(string filePath)
        {
            //if there is no file, return null
            if (File.Exists(filePath) == false)
            {
                Debug.Log("File not found: " + filePath);
                return null;
            }

            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        public static void DeleteFile(string filePath)
        {
            //check there is a file
            if (File.Exists(filePath) == false)
            {
                Debug.Log("File not found: " + filePath);
                return;
            }

            //delete file
            File.Delete(filePath);
            Debug.Log("File deleted successfully: " + filePath);
        }

        /// <summary>
        /// Delete directory with every file
        /// </summary>
        public static void DeleteDirectory(string directoryPath)
        {
            //check there is a directory
            if (Directory.Exists(directoryPath) == false)
            {
                Debug.Log("Directory not found: " + directoryPath);
                return;
            }

            //delete directory
            Directory.Delete(directoryPath, true);
            Debug.Log("Directory deleted successfully: " + directoryPath);
        }
    }
}