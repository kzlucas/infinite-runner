using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BiomesData : Singleton<BiomesData>, IInitializable
{
    private IEnumerator lerpBiomeColorCoroutineInstance;

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

        return Task.CompletedTask;
    }

    /// <summary>
    ///   Get next biome data based on current biome.
    ///   Loop back to first biome if at the end.
    /// </summary>
    /// <returns></returns>
    public BiomeData SetNext()
    {
        if (current == null)
        {
            Debug.LogError("[BiomesData] Current biome is null!");
            return null;
        }
        int currentIndex = items.IndexOf(current);
        if (currentIndex == -1)
        {
            currentIndex = 0;
        }
        int nextIndex = (currentIndex + 1) % items.Count;
        ApplyDataAtIndex(nextIndex);
        return current;
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
