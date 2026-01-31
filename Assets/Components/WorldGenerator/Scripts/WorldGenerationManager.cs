using System;
using System.Collections;
using System.Threading.Tasks;
using Assets.Components.SquareColliders.Scripts;
using Components.Events;
using Components.Scenes;
using Components.ServiceLocator.Scripts;
using UnityEngine;


namespace WorldGenerator.Scripts
{

    /// <summary>
    ///   Manager responsible for generating world segments
    ///   as the player progresses through the world.
    /// </summary>
    public class WorldGenerationManager : Singleton.Model<WorldGenerationManager>, IInitializable
    {


        [Header("Dependencies")]
        private SquareCollidersMerger SquareCollidersMerger => ServiceLocator.Get<SquareCollidersMerger>();



        [Header("Initialization")]
        public int InitPriority => 2;
        public Type[] InitDependencies => new Type[] { typeof(BiomesDataManager) };



        [Header("References")]

        /// <summary> Biome limit game object</summary>
        public WorldStartOverlay WorldStartOverlay;

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

        /// <summary> Index for generated segments</summary>
        private int _generatedIndex = 0;

        /// <summary> Coroutine reference for world generation</summary>
        private IEnumerator _generationCoroutine;

        /// <summary> Z position of last recorded player position</summary>
        public float LastRecordZPosition = 0f;

        /// <summary>Number of segments to generate in front of the player</summary>
        private int _frontGenerationWindowSize { get {
            if (Application.platform == RuntimePlatform.WebGLPlayer) 
                return 120;  
            else
                return 300;
        } }


        private void Start() => EventBus.Subscribe<SceneExitEvent>(OnSceneExitEvent);
        private void OnDestroy() => EventBus.Unsubscribe<SceneExitEvent>(OnSceneExitEvent);
        private void OnSceneExitEvent(SceneExitEvent e)
        {
            StopAllCoroutines();
        }


        /// <summary>
        /// Get the needed references and setup for world generation
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            // Start generation thread
            _generatedIndex = 0;
            GenerateSegments();
            await Task.CompletedTask;
        }



        /// <summary> Generate segments</summary>
        public void GenerateSegments(bool clearExisting = true)
        {
            // destroy existing segments
            if (clearExisting)
            {
                _generatedIndex = 0;
                InstancesRegistry.Clear();
            }

            // (re)Start generation thread
            if (_generationCoroutine != null) StopCoroutine(_generationCoroutine);
            _generationCoroutine = GenerationWalkerRoutine();
            StartCoroutine(_generationCoroutine);
        }



        /// <summary>
        /// Check if we need to generate or rm segments and do it
        /// </summary>
        private IEnumerator GenerationWalkerRoutine()
        {
            if (playerTransform == null) yield break;

            // Generate new segments if needed
            int cursor = (int)playerTransform.position.z - 10; // generate slightly behind player to avoid display empty world in camera
            int maxZ = cursor + _frontGenerationWindowSize;
            while (cursor < maxZ)
            {
                // Check if segment at this z already exists
                var segmentInstance = InstancesRegistry.FindByZ(cursor);
                if (segmentInstance == null)
                {
                    // Data init
                    var lastSegment = InstancesRegistry.LastOrDefault();
                    var zTarget = cursor;

                    // Add biome change overlay if biome changed 
                    if(ShouldGenerateChangeOverlay(ref lastSegment))
                        GenerateBiomeChangeOverlay(ref lastSegment, lastSegment.Position);

                    // Create new segment 
                    InstantiateNewSegment(ref lastSegment, ref segmentInstance, zTarget);

                    // Special case: Remove crystals in tutorial biome segments until certain index
                    if ((BiomesDataManager.Instance.Current.BiomeName == "World 0 - Tuto")
                    && (_generatedIndex <= 24)) segmentInstance.RemoveCrystals();

                    // no need to block frame, each segment can be created in its own frame 
                    if (Application.isPlaying) yield return null;
                }
                cursor += segmentInstance.SizeZ;
            }

            // Remove all segments that are behind the player
            InstancesRegistry.ClearSegmentsBehind(LastRecordZPosition - 10);

            // Update colliders
            SquareCollidersMerger.GenerateSquareColliders();

            // Restart coroutine
            yield return new WaitForEndOfFrame();
            GenerateSegments(clearExisting: false);
        }


        /// <summary>
        /// Check if we need to generate biome change overlay
        /// </summary>
        /// <param name="lastSegment"></param>
        /// <returns></returns>
        private bool ShouldGenerateChangeOverlay(ref WorldSegment lastSegment)
        {
            var currentBiomeName = BiomesDataManager.Instance.Current.BiomeName;
            var previousBiomeName = InstancesRegistry.PreviousOrDefault()?.BiomeData?.BiomeName;
            var worldIsStarting = lastSegment != null && (previousBiomeName != null) && (currentBiomeName != previousBiomeName);
            return worldIsStarting;
        }

        /// <summary>
        /// Generate overlay at the start of a new biome
        /// </summary>
        private void GenerateBiomeChangeOverlay(ref WorldSegment lastSegment, Vector3 pos)
        {
            // cleanup crystals from previous biome
            var previousBiomeName = InstancesRegistry.PreviousOrDefault()?.BiomeData?.BiomeName;
            InstancesRegistry.RemoveBiomeCrystals(previousBiomeName);

            // cleanup existing overlays
            foreach (var go in GameObject.FindGameObjectsWithTag("WorldStart"))
            {
                Destroy(go);
            }
            // create new overlay
            var _worldStartOverlay = Instantiate(WorldStartOverlay.gameObject);
            _worldStartOverlay.transform.parent = this.transform;
            _worldStartOverlay.GetComponent<WorldStartOverlay>().Set(pos, BiomesDataManager.Instance.Current.ColorSkyGround);
            _generatedIndex = 0; // reset index on biome change
        }

        /// <summary>
        /// Instantiate a new world segment at given z position
        /// </summary>
        private void InstantiateNewSegment(ref WorldSegment lastSegment, ref WorldSegment segmentInstance, int zTarget)
        {
            _generatedIndex++;
            if (lastSegment != null) zTarget = lastSegment.RangeZ.Item2;
            var selectionStrategy = new SelectionStrategy(BiomesDataManager.Instance.Current);
            var segmentModel = selectionStrategy.Select(_generatedIndex);
            segmentInstance = segmentModel.ToInstance(zTarget, BiomesDataManager.Instance.Current, this.transform);
            InstancesRegistry.RegisterInstance(segmentInstance);
        }
    }
}
