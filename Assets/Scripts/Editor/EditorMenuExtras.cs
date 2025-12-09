using UnityEditor;
using UnityEngine;

public class EditorMenuExtras : MonoBehaviour
{

    [MenuItem("Tools/World Generation/Generate World Segments %g")] // Ctrl + G
    private static void GenerateWorldSegments()
    {
        var worldGenManager = FindFirstObjectByType<WorldGenerationManager>();
        if (worldGenManager != null)
        {
            worldGenManager.GenerateSegments();
        }
        else
        {
            Debug.LogError("[EditorMenuExtras] WorldGenerationManager not found in the scene.");
        }
    }
}
