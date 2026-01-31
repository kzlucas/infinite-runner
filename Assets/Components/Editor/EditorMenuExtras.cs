using Components.DataServices;
using Components.Events;
using UnityEditor;
using UnityEngine;
using WorldGenerator.Scripts;

namespace Components.Editor.EditorExtra
{
    /// <summary>
    /// Extra editor menu items to call debug and utility functions
    /// </summary>

    public class EditorMenuExtras : MonoBehaviour
    {

        [MenuItem("Tools/World Generation/Generate World Segments %g")] // Ctrl + G
        private static void GenerateWorldSegments()
        {
            if (BiomesDataManager.Instance != null)
            {
                BiomesDataManager.Instance.CycleToNextBiome();
            }
            else
            {
                Debug.LogError("[EditorMenuExtras] WorldGenerationManager not found in the scene.");
            }



            var worldGenManager = FindFirstObjectByType<WorldGenerationManager>();
            if (worldGenManager != null)
            {
                worldGenManager.GenerateSegments();
            }

            else
            {
                Debug.LogError("[EditorMenuExtras] BiomesData instance not found.");
            }
        }


        [MenuItem("Tools/Delete Save File and Player Prefs %&d")] // Ctrl + Alt + D
        private static void DeleteSave()
        {
            SaveService.DeleteSave();
            PlayerPrefService.DeleteAll();
        }


        [MenuItem("Tools/EventBus/Log Active Subscriptions")]
        private static void LogEventBusSubscriptions()
        {
            EventBus.LogActiveSubscriptions();
        }

    }
}
