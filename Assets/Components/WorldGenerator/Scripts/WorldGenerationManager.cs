using System;
using System.Collections;
using System.Threading.Tasks;
using Assets.Components.SquareColliders.Scripts;
using Components.ServiceLocator.Scripts;
using UnityEngine;
using WorldGenerator.Scripts;


namespace WorldGenerator.Scripts
{

    /// <summary>
    ///   Manager responsible for generating world segments
    ///   as the player progresses through the world.
    /// </summary>
    public class WorldGenerationManager : Singleton<WorldGenerationManager>, IInitializable
    {


        [Header("Dependencies")]
        private SquareCollidersMerger SquareCollidersMerger => ServiceLocator.Get<SquareCollidersMerger>();



        [Header("Initialization")]
        public int initPriority => 2;
        public Type[] initDependencies => new Type[] { typeof(BiomesData) };



        [Header("References")]

        /// <summary>Current world segments</summary>

        /// <summary>Reference to the player transform for generation tracking</summary>
        private Transform _playerTransform;
        public Transform playerTransform
        {
            get
            {
                if (_playerTransform == null) _playerTransform = GameObject.FindWithTag("Player")?.transform;
                return _playerTransform;
            }
        }


        [Header("Settings")]

        /// <summary>Number of segments to generate in front of the player</summary>
        public int frontGenerationWindowSize = 100;

        /// <summary> Index for generated segments</summary>
        private int generatedIndex = 0;

        /// <summary> Coroutine reference for world generation</summary>
        private IEnumerator generationCoroutine;

        /// <summary> Z position of last recorded player position</summary>
        public float lastRecordZPosition = 0f;


        private void OnDestroy() => StopAllCoroutines();


        /// <summary>
        /// Get the needed references and setup for world generation
        /// </summary>
        /// <returns></returns>
        public Task InitializeAsync()
        {
            // Start generation thread
            generatedIndex = 0;
            GenerateSegments();

            // Stop generation on scene exit
            SceneLoader.Instance.OnSceneExit += () => StopAllCoroutines();

            return Task.CompletedTask;
        }



        /// <summary> Generate segments</summary>
        public void GenerateSegments(bool clearExisting = true)
        {
            // destroy existing segments
            if (clearExisting)
            {
                generatedIndex = 0;
                InstancesRegistry.Clear();
                foreach (var seg in GameObject.FindGameObjectsWithTag("World Segment"))
                    DestroyImmediate(seg);
            }

            // (re)Start generation thread
            if (generationCoroutine != null) StopCoroutine(generationCoroutine);
            generationCoroutine = GenerationRoutine();
            StartCoroutine(generationCoroutine);
        }



        /// <summary>
        /// Check if we need to generate or rm segments and do it
        /// </summary>
        private IEnumerator GenerationRoutine()
        {
            if (playerTransform == null) yield break;

            // Generate new segments if needed
            int cursor = (int)playerTransform.position.z;
            int maxZ = cursor + frontGenerationWindowSize;
            while (cursor < maxZ)
            {
                // Check if segment at this z already exists
                var segmentInstance = InstancesRegistry.FindByZ(cursor);
                if (segmentInstance == null)
                {
                    var lastSegment = InstancesRegistry.LastOrDefault();
                    var currentBiomeName = BiomesData.Instance.current.BiomeName;
                    var previousBiomeName = InstancesRegistry.PreviousOrDefault()?.BiomeData?.BiomeName;
                    var zTarget = cursor;

                    /*
                     *
                     * Add biome change overlay if biome changed 
                     */

                    if (lastSegment != null && (currentBiomeName != previousBiomeName))
                    {
                        WorldStartOverlay.Instance.Set(
                            lastSegment.position + new Vector3(0, 0, lastSegment.sizeZ)
                            , BiomesData.Instance.current.ColorSkyGround
                        );
                        generatedIndex = 0; // reset index on biome change
                    }

                    /*
                     *
                     * Create new segment 
                     */

                    generatedIndex++;

                    if (lastSegment != null) zTarget = lastSegment.rangeZ.Item2;
                    var selectionStrategy = new SelectionStrategy(BiomesData.Instance.current);
                    var segmentModel = selectionStrategy.Select(generatedIndex);
                    segmentInstance = segmentModel.ToInstance(zTarget, BiomesData.Instance.current, this.transform);
                    InstancesRegistry.RegisterInstance(segmentInstance);


                    /*
                     *
                     * Special case: Remove crystals in tutorial biome segments until certain index
                     */

                    if ((BiomesData.Instance.current.BiomeName == "Tutorial")
                    && (generatedIndex <= 24))
                    {
                        segmentInstance.RemoveCrystals();
                    }


                    /*
                     *
                     * no need to block frame, each segment can be created in its own frame 
                     */

                    if (Application.isPlaying) yield return null;
                }

                if (segmentInstance == null)
                {
                    Debug.LogError("[WorldGenerationManager] World segment is null after instantiation!");
                    break;
                }
                if (segmentInstance.sizeZ <= 0)
                {
                    Debug.LogError("[WorldGenerationManager] World segment sizeZ is invalid after calculation!");
                    break;
                }
                cursor += segmentInstance.sizeZ;
            }

            // Remove all segments that are behind the player
            InstancesRegistry.ClearSegmentsBehind(lastRecordZPosition - 10);

            // Update colliders
            SquareCollidersMerger.GenerateSquareColliders();

            // Restart coroutine
            yield return new WaitForEndOfFrame();
            GenerateSegments(clearExisting: false);
        }



    }
}
