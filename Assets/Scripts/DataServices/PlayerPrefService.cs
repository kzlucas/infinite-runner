using UnityEngine;

/// <summary>
/// Save User progress and settings
/// </summary>
public static class PlayerPrefService
{
    /// <summary>
    ///   Save user data to player prefs
    /// </summary>    
    /// <param name="key">The key to save the data under</param>
    /// <param name="value">The value to save</param>
    public static void Save(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }


    /// <summary>
    ///   Load user data from player prefs
    /// </summary>    
    /// <param name="key">The key to load the data from</param>
    /// <returns>The loaded value, or null if not found</returns>
    public static string Load(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        return null;
    }
}