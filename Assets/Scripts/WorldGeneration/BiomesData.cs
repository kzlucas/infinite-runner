using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components.ServiceLocator.Scripts;
using Components.UI.Scripts;
using UnityEngine;


public class BiomesData : Singleton<BiomesData>, IInitializable
{
        
    [Header("Dependencies")]
    private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();


    [Header("References")]
    private IEnumerator lerpBiomeColorCoroutineInstance;
    public WorldGenerationManager worldGenerationManager;


    [Header("Initialization")]
    public int initPriority => 1;
    public System.Type[] initDependencies => null;


    [Header("Biome Data")]
    public List<SO_BiomeData> items = new List<SO_BiomeData>();
    public SO_BiomeData current = null;



    /// <summary>
    ///   Initializes the Biomes Data Manager.
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync()
    {
        if(!TutorialManager.Instance.tutorialsCompleted)
            ApplyDataAtIndex(0);
        else
            ApplyDataAtIndex(1);

        return Task.CompletedTask;
    }

    /// <summary>
    ///   Get next biome data based on current biome.
    /// </summary>
    /// <returns>Returns true if biome was changed, false if already at last biome.</returns>
    public void SetNext()
    {
        // Mark tutorial completed if called on first biome
        int currentIndex = items.FindIndex(b => b.name == current.name);
        if (currentIndex == 0)
        {
            TutorialManager.Instance.Play("Completed");
            PlayerPrefService.Save("SkipTutorials", "1");
            TutorialManager.Instance.tutorialsCompleted = true;
        }

        // Set next biome data. If at end of list loop back to start
        int nextIndex = currentIndex + 1;
        if (nextIndex >= items.Count) nextIndex = 1;
        ApplyDataAtIndex(nextIndex);

        // Clear world segments to force regeneration with new biome
        if (worldGenerationManager == null) 
            worldGenerationManager = FindFirstObjectByType<WorldGenerationManager>();

        StartCoroutine(FlashAndRegenWorld());
    }


    private IEnumerator FlashAndRegenWorld()
    {
        if(Application.isPlaying) UiRegistry.screenOverlay.Flash("white");
        yield return new WaitForSeconds(0.1f);
        worldGenerationManager.RemoveBiomeCrystals(current.name);
        worldGenerationManager.ClearSegmentsInFrontPlayer(30);
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
        float lerpDuration = .5f;
        lerpBiomeColorCoroutineInstance = LerpBiomeColors(current.ColorSky, current.ColorSkyHorizon, current.ColorSkyGround, lerpDuration);
        
        StartCoroutine(lerpBiomeColorCoroutineInstance);
    }


    /// <summary>
    ///   Gradually change biome colors
    ///  </summary>
    private IEnumerator LerpBiomeColors(Color targetSky, Color targetHorizon, Color targetGround, float duration)
    {

        if(SceneLoader.Instance.currentSceneName != "Game")
            yield break;

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
