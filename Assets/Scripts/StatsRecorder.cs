using Components.EndGame.Scripts;
using Components.Events;
using Components.ServiceLocator.Scripts;
using UnityEngine;

/// <summary>
///   Records and manages player statistics and progress
/// </summary>
public static class StatsRecorder
{

    [Header("Dependencies")]
    private static EndGameManager EndGameManager => ServiceLocator.Get<EndGameManager>();


    [Header("Player Stats")]
    private static SaveData saveData;
    public static string lastBiomeReached = "None";
    public static int currentRunDistanceReached = 0;
    public static int currentRunCoinsCollected = 0;

    static StatsRecorder()
    {
        GetSaveData();
        EventBus.Subscribe<OnRunStart>(OnRunStart);
        EndGameManager.OnEndGame += OnGameEnd;
    }

    private static void OnRunStart(OnRunStart evt)
    {
        IncrementRunsCount();
    }

    private static void OnGameEnd()
    {
        SaveService.Save(saveData);
    }

    private static void GetSaveData()
    {
        saveData = SaveService.Load();
        if (saveData == null)
        {
            saveData = new SaveData();
        }
    }

    private static void IncrementRunsCount()
    {
        saveData.RunsCount += 1;
    }

    public static void UpdateLastBiomeReached(string biomeName)
    {
        lastBiomeReached = biomeName;
    }

    public static void SetMaxDistanceReached(int distance)
    {
        currentRunDistanceReached = distance;
        saveData.MaxDistanceReached = Mathf.Max(saveData.MaxDistanceReached, distance);
    }

    public static int GetMaxDistanceReached()
    {
        return saveData.MaxDistanceReached;
    }

    public static void SetMaxCoinsCollected(int crystalsCollected)
    {
        currentRunCoinsCollected = crystalsCollected;
        saveData.MaxCrystalsCollectedInRun = Mathf.Max(saveData.MaxCrystalsCollectedInRun, crystalsCollected);
    }

    public static int GetMaxCoinsCollected()
    {
        return saveData.MaxCrystalsCollectedInRun;
    }
}