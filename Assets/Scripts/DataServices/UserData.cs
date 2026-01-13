using UnityEngine;

/// <summary>
/// Save User progress and settings
/// </summary>
public class UserData : Singleton<UserData>
{


    /// <summary>
    ///   Save user data to player prefs
    /// </summary>    
    /// <param name="key">The key to save the data under</param>
    /// <param name="value">The value to save</param>
    public void Save(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }


    /// <summary>
    ///   Load user data from player prefs
    /// </summary>    
    /// <param name="key">The key to load the data from</param>
    /// <returns>The loaded value, or null if not found</returns>
    public string Load(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        return null;
    }

}