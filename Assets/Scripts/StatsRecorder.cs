using UnityEngine;

/// <summary>
///   Records and manages player statistics.
/// </summary>
public class StatsRecorder : Singleton<StatsRecorder>
{
    private void Start()
    {
        // Update RunsCount in save data
        var data = SaveService.Load();
        if (data == null) data = new SaveData();
        data.RunsCount += 1;
        SaveService.Save(data);
    }
}