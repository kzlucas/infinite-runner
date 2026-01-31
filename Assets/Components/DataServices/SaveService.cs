using System.IO;
using UnityEngine;


namespace Components.DataServices
{


    /// <summary>
    ///  Service for saving and loading data
    /// </summary>
    public static class SaveService
    {
        private static readonly string saveFileName = "save.json";


        /// <summary>
        ///   Save data
        /// </summary>    
        /// <param name="data">The data to save</param>
        public static void Save<T>(T data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(GetSaveFilePath(), json);
        }


        /// <summary>
        ///   Load data
        /// </summary>
        /// <returns>The loaded data</returns>
        public static SaveData Load()
        {
            string path = GetSaveFilePath();
            if (!File.Exists(path))
            {
                Debug.Log("[SaveService] No save file found at " + path + ". Creating new save file.");
                var data = new SaveData();
                Save(data);
                return data;
            }
            try
            {
                string json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<SaveData>(json);
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError("[SaveService] Failed to load save file: " + e.Message);
                return null;
            }
        }


        /// <summary>
        ///  Delete save file
        /// </summary>
        public static void DeleteSave()
        {
            string path = GetSaveFilePath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        /// <summary>
        ///   Save file path retriever
        /// </summary>
        /// <returns>Path to save file</returns>
        private static string GetSaveFilePath()
        {
            return Application.persistentDataPath + "/" + saveFileName;
        }



    }
}