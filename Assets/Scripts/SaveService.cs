using System.IO;
using UnityEngine;

/// <summary>
///  Service for saving and loading data
/// </summary>
/// 
/// // Example usage:
///    // To save data:
///    SaveService.Save(yourSerializableDataObject);
///    // To load data:
///    if(SaveService.TryLoad<YourDataType>(out var data))
///    {
///        // Use loaded data
///    }
/// //
/// 
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
    public static T TryLoad<T>(out T data) where T : Object
    {
        string path = GetSaveFilePath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[SaveService] No save file found at " + path);
            data = null;
            return null;
        }
        try
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<T>(json);
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[SaveService] Failed to load save file: " + e.Message);
            data = null;
            return null;
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