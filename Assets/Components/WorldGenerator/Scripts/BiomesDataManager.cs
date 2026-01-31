using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components.Events;
using Components.ServiceLocator.Scripts;
using Components.UI.Scripts;
using Tutorials;
using UnityEngine;


namespace WorldGenerator.Scripts
{

    /// <summary>
    ///  Manager for biome data and transitions between biomes
    /// </summary>
    public class BiomesDataManager : Singleton.Model<BiomesDataManager>, IInitializable, IGameService
    {

        [Header("Dependencies")]
        private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();


        [Header("References")]
        private IEnumerator lerpBiomeColorCoroutineInstance;


        [Header("Initialization")]
        public int initPriority => 1;
        public System.Type[] initDependencies => new System.Type[]
        {
            typeof(UiRegistry),
        };


        [Header("Biome Data")]
        public List<SO_BiomeData> items = new List<SO_BiomeData>();
        public SO_BiomeData current = null;



        /// <summary>
        ///   Initializes the Biomes Data Manager.
        /// </summary>
        /// <returns></returns>
        public Task InitializeAsync()
        {
            SetBiome(1); // set initial biome
            return Task.CompletedTask;
        }

        /// <summary>
        ///   Cycle to the next biome in the list.
        /// </summary>
        public void CycleToNextBiome()
        {
            int CycleBiomesIndex()
            {
                int currentIndex = items.FindIndex(b => b.name == current.name);
                int nextIndex = currentIndex + 1;
                if (nextIndex >= items.Count) nextIndex = 1;
                return nextIndex;
            }

            SetBiome(CycleBiomesIndex());
        }

        ///   Apply biome data at given index.
        /// </summary>
        /// <param name="index"></param>
        private void SetBiome(int index)
        {
            Debug.Log("[BiomesData] Setting biome at index: " + index);
            
            var tutorialManager = ServiceLocator.Get<TutorialManager>();
            if (!tutorialManager.tutorialsCompleted)
                index = 0;

            if (index < 0 || index >= items.Count)
            {
                Debug.LogError("[BiomesData] Index out of range when applying biome data!");
                return;
            }

            current = items[index];
            EventBus.Publish(new BiomeChangedEvent(current));

            Debug.Log("[BiomesData] Changing to biome: " + current.BiomeName);

            // Update Sky color and Regenerate world
            float lerpDuration = .5f;
            lerpBiomeColorCoroutineInstance = LerpBiomeColors(current.ColorSky, current.ColorSkyHorizon, current.ColorSkyGround, lerpDuration);
            StartCoroutine(lerpBiomeColorCoroutineInstance);
            StartCoroutine(RegenWorld());
        }


        /// <summary>
        ///   Gradually change biome colors
        ///  </summary>
        private IEnumerator LerpBiomeColors(Color targetSky, Color targetHorizon, Color targetGround, float duration)
        {

            if (SceneLoader.Instance.currentSceneName != "Game")
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


        private IEnumerator RegenWorld()
        {
            yield return new WaitUntil(() => UiRegistry.screenOverlay != null);
            if (Application.isPlaying) UiRegistry.screenOverlay.Flash("white");
            yield return new WaitForSeconds(0.1f);
            var playerTransform = FindFirstObjectByType<Player.Controller>()?.transform;
            InstancesRegistry.ClearSegmentsInFront((int)(playerTransform.position.z + 30)); // force regen
        }
    }
}
