using UnityEngine;

/// <summary>
///   Records and manages player statistics and progress
/// </summary>
public class StatsRecorder : Singleton<StatsRecorder>
{
    private SaveData saveData;
    public int paintCollected = 0;
    public string lastBiomeReached = "None";

    private void Start()
    {
        paintCollected = 0;
        
        GetSaveData();
        IncrementRunsCount();
        EndGameManager.Instance.OnEndGame += OnGameEnd;
    }

    private void OnDestroy() => EndGameManager.Instance.OnEndGame -= OnGameEnd;

    private void OnGameEnd()
    {
        saveData.MaxPaintCollectedInRun = Mathf.Max(saveData.MaxPaintCollectedInRun, paintCollected);
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

    public void IncrementPaintCollected(int amount)
    {
        paintCollected += amount;
    }

    public void UpdateLastBiomeReached(string biomeName)
    {
        lastBiomeReached = biomeName;
    }
}