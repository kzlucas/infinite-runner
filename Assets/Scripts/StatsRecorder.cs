using UnityEngine;

/// <summary>
///   Records and manages player statistics and progress
/// </summary>
public class StatsRecorder : Singleton<StatsRecorder>
{
    private SaveData saveData;
    public string lastBiomeReached = "None";
    public int currentRunDistanceReached = 0;
    public int currentRunCoinsCollected = 0;

    private void Start()
    {
        GetSaveData();
        IncrementRunsCount();
        EndGameManager.Instance.OnEndGame += OnGameEnd;
    }

    private void OnGameEnd()
    {
        SaveService.Save(saveData);
    }

    private void GetSaveData()
    {
        saveData = SaveService.Load();
        if (saveData == null)
        {
            saveData = new SaveData();
        }
    }

    private void IncrementRunsCount()
    {
        saveData.RunsCount += 1;
    }

    public void UpdateLastBiomeReached(string biomeName)
    {
        lastBiomeReached = biomeName;
    }

    public void SetMaxDistanceReached(int distance)
    {
        currentRunDistanceReached = distance;
        saveData.MaxDistanceReached = Mathf.Max(saveData.MaxDistanceReached, distance);
    }

    public int GetMaxDistanceReached()
    {
        return saveData.MaxDistanceReached;
    }

    public void SetMaxCoinsCollected(int crystalsCollected)
    {
        currentRunCoinsCollected = crystalsCollected;
        saveData.MaxCrystalsCollectedInRun = Mathf.Max(saveData.MaxCrystalsCollectedInRun, crystalsCollected);
    }

    public int GetMaxCoinsCollected()
    {
        return saveData.MaxCrystalsCollectedInRun;
    }
}