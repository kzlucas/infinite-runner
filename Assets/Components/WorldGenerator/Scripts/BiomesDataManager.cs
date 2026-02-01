using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components.Events;
using Components.UI.Scripts;
using Components.Collectible;
using Components.Tutorials;
using UnityEngine;


namespace WorldGenerator.Scripts
{

    /// <summary>
    ///  Manager for biome data and transitions between biomes
    /// </summary>
    public class BiomesDataManager : Singleton.Model<BiomesDataManager>, IInitializable, IGameService
    {


        [Header("References")]
        private IEnumerator _lerpBiomeColorCoroutineInstance;


        [Header("Initialization")]
        public int InitPriority => 1;
        public System.Type[] InitDependencies => new System.Type[]
        {
            typeof(UiRegistry),
        };


        [Header("Biome Data")]
        public List<SO_BiomeData> Items = new List<SO_BiomeData>();
        public SO_BiomeData Current = null;



        /// <summary>
        ///   Initializes the Biomes Data Manager.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            if(SceneLoader.Instance.IsGameScene())
            {
                SetBiome(1); // set initial biome
            }
            await Task.CompletedTask;
        }

        /// <summary>
        ///   Cycle to the next biome in the list.
        /// </summary>
        public void CycleToNextBiome()
        {
            int CycleBiomesIndex()
            {
                int currentIndex = Items.FindIndex(b => b.name == Current.name);
                int nextIndex = currentIndex + 1;
                if (nextIndex >= Items.Count) nextIndex = 1;
                return nextIndex;
            }

            SetBiome(CycleBiomesIndex());
        }

        ///   Apply biome data at given index.
        /// </summary>
        /// <param name="index"></param>
        private void SetBiome(int index)
        {
            if (!TutorialManager.Instance.AllTutorialsCompleted)
                index = 0;

            if (index < 0 || index >= Items.Count)
            {
                Debug.LogError("[BiomesData] Index out of range when applying biome data!");
                return;
            }

            Current = Items[index];
            EventBus.Publish(new BiomeChangedEvent(Current));

            Debug.Log("[BiomesData] Changing to biome: " + Current.BiomeName);

            // Update Sky color and Regenerate world
            float lerpDuration = .5f;
            _lerpBiomeColorCoroutineInstance = LerpBiomeColors(Current.ColorSky, Current.ColorSkyHorizon, Current.ColorSkyGround, lerpDuration);
            StartCoroutine(_lerpBiomeColorCoroutineInstance);
            StartCoroutine(RegenWorld());
        }


        /// <summary>
        ///   Gradually change biome colors
        ///  </summary>
        private IEnumerator LerpBiomeColors(Color targetSky, Color targetHorizon, Color targetGround, float duration)
        {

            if (SceneLoader.Instance.CurrentSceneName != "Game")
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
            yield return new WaitUntil(() => UiRegistry.Instance.ScreenOverlay != null);
            if (Application.isPlaying) UiRegistry.Instance.ScreenOverlay.Flash("white");
            yield return new WaitForSeconds(0.1f);
            var playerTransform = Components.Player.Utils.PlayerController.transform;
            InstancesRegistry.ClearSegmentsInFront((int)(playerTransform.position.z + 30)); // force regen
            CrystalsManager.ClearBucket();
        }
    }
}
