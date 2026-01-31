using Components.DataServices;
using Components.Player.Events;
using UnityEngine;


namespace Components.Stats
{

    /// <summary>
    ///   Records and manages player statistics and progress
    /// </summary>
    public static class StatsRecorder
    {

        [Header("Player Stats")]
        private static SaveData saveData;
        public static string LastBiomeReached = "None";
        public static int CurrentRunDistanceReached = 0;
        public static int CurrentRunCoinsCollected = 0;

        static StatsRecorder()
        {
            GetSaveData();
        }

        public static void OnGameEnd()
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

        public static void IncrementRunsCount()
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