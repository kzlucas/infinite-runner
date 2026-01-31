using Components.DataServices;
using Components.EndGame.Scripts;
using Components.Events;
using Components.Player.Events;
using UnityEngine;


namespace Components.Stats
{

    /// <summary>
    ///   Records and manages player statistics and progress
    /// </summary>
    public static class StatsRecorder
    {

        [Header("Dependencies")]
        private static EndGameManager EndGameManager => ServiceLocator.Scripts.ServiceLocator.Get<EndGameManager>();


        [Header("Player Stats")]
        private static SaveData saveData;
        public static string LastBiomeReached = "None";
        public static int CurrentRunDistanceReached = 0;
        public static int CurrentRunCoinsCollected = 0;

        static StatsRecorder()
        {
            GetSaveData();
            EventBus.Subscribe<OnRunStart>(OnRunStart);
            EventBus.Subscribe<Dead>(OnGameEnd);
        }

        private static void OnRunStart(OnRunStart evt)
        {
            IncrementRunsCount();
        }

        private static void OnGameEnd(Dead deadEvent)
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
            LastBiomeReached = biomeName;
        }

        public static void SetMaxDistanceReached(int distance)
        {
            CurrentRunDistanceReached = distance;
            saveData.MaxDistanceReached = Mathf.Max(saveData.MaxDistanceReached, distance);
        }

        public static int GetMaxDistanceReached()
        {
            return saveData.MaxDistanceReached;
        }

        public static void SetMaxCoinsCollected(int crystalsCollected)
        {
            CurrentRunCoinsCollected = crystalsCollected;
            saveData.MaxCrystalsCollectedInRun = Mathf.Max(saveData.MaxCrystalsCollectedInRun, crystalsCollected);
        }

        public static int GetMaxCoinsCollected()
        {
            return saveData.MaxCrystalsCollectedInRun;
        }
    }
}