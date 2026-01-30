using UnityEditor;
using UnityEngine;
using WorldGenerator.Scripts;

public class EditorMenuExtras : MonoBehaviour
{

    [MenuItem("Tools/World Generation/Generate World Segments %g")] // Ctrl + G
    private static void GenerateWorldSegments()
    {

        
        if(BiomesData.Instance != null)
        {
            BiomesData.Instance.SetNext();
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


}
