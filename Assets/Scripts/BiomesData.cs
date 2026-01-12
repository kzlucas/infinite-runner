using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BiomesData : Singleton<BiomesData>, IInitializable
{

    [Header("References")]
    private IEnumerator lerpBiomeColorCoroutineInstance;
    public WorldGenerationManager worldGenerationManager;


    [Header("Initialization")]
    public int initPriority => 0;
    public System.Type[] initDependencies => null;


    [Header("Biome Data")]
    public List<BiomeData> items = new List<BiomeData>();
    public BiomeData current = null;



    /// <summary>
    ///   Initializes the Biomes Data Manager.
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync()
    {
        if (items.Count > 0)
            ApplyDataAtIndex(0);
        else
            Debug.LogError("[BiomesData] No biome data found!");


        worldGenerationManager = FindFirstObjectByType<WorldGenerationManager>();
        if (worldGenerationManager == null)
        {
            Debug.LogError("[BiomesData] WorldGenerationManager not found in scene!");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///   Get next biome data based on current biome.
    /// </summary>
    /// <returns>Returns true if biome was changed, false if already at last biome.</returns>
    public bool SetNext()
    {
        if (current == null)
        {
            Debug.LogError("[BiomesData] Current biome is null!");
            return false;
        }
        int currentIndex = items.IndexOf(current);
        if (currentIndex == -1)
        {
            currentIndex = 0;
        }

        // Set next biome data. If at end of list, stay at current biome.
        if((currentIndex + 1) < items.Count)
        {
            int nextIndex = currentIndex + 1;
            ApplyDataAtIndex(nextIndex);
            
            // Clear world segments to force regeneration with new biome
            worldGenerationManager.ClearNextSegments();
            return true;
        }
        
        return false;
    }



    /// <summary>
    ///   Apply biome data at given index.
    /// </summary>
    /// <param name="index"></param>
    private void ApplyDataAtIndex(int index)
    {            
        if (index < 0 || index >= items.Count)
        {
            Debug.LogError("[BiomesData] Index out of range when applying biome data!");
            return;
        }

        current = items[index];
        Debug.Log("[BiomesData] Changing to biome: " + current.name);

        // Update "sky color of RenderSettings.skybox to match biome color
        float lerpDuration = 1f;
        lerpBiomeColorCoroutineInstance = LerpBiomeColors(current.colorSky, current.colorSkyHorizon, current.colorSkyGround, lerpDuration);
        StartCoroutine(lerpBiomeColorCoroutineInstance);
    }


    /// <summary>
    ///   Gradually change biome colors
    ///  </summary>
    private IEnumerator LerpBiomeColors(Color targetSky, Color targetHorizon, Color targetGround, float duration)
    {
        Color initialSky = RenderSettings.skybox.GetColor("_SkyColor");
        Color initialHorizon = RenderSettings.skybox.GetColor("_HorizonColor");
        Color initialGround = RenderSettings.skybox.GetColor("_GroundColor");

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            RenderSettings.skybox.SetColor("_SkyColor", Color.Lerp(initialSky, targetSky, t));
            RenderSettings.skybox.SetColor("_HorizonColor", Color.Lerp(initialHorizon, targetHorizon, t));
            RenderSettings.skybox.SetColor("_GroundColor", Color.Lerp(initialGround, targetGround, t));

            yield return null;
        }
        // Ensure final colors are set
        RenderSettings.skybox.SetColor("_SkyColor", targetSky);
        RenderSettings.skybox.SetColor("_HorizonColor", targetHorizon);
        RenderSettings.skybox.SetColor("_GroundColor", targetGround);
    }
}
