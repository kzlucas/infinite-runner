using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WorldGenerationManager : Singleton<WorldGenerationManager>, IInitializable
{

    public int initPriority => 2;
    public Type[] initDependencies => new Type[] { typeof(BiomesData) };



    /// <summary>Current world segments</summary>
    private List<WorldSegment> currentWorldSegments = new List<WorldSegment>();

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

    /// <summary>Number of segments to generate in front of the player</summary>
    public int frontGenerationWindowSize = 100;

    /// <summary> Index for generated segments</summary>
    private int generatedIndex = 0;

    /// <summary> Coroutine reference for world generation</summary>
    private IEnumerator generationCoroutine;

    /// <summary> Last instantiated biome name to track biome changes </summary>
    private string lastInstantiatedBiomeName = "";

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
            currentWorldSegments.Clear();
            foreach (var seg in GameObject.FindGameObjectsWithTag("World Segment"))
                DestroyImmediate(seg);
        }

        // (re)Start generation thread
        if (generationCoroutine != null) StopCoroutine(generationCoroutine);
        generationCoroutine = GenerationRoutine();
        StartCoroutine(generationCoroutine);
    }


    /// <summary>
    /// Pick a world segment prefab to instantiate
    /// </summary>
    private WorldSegment PickWorldSegmentPrefab()
    {
        switch (BiomesData.Instance.current.BiomeName)
        {
            case "Tutorial":


                if (generatedIndex <= 3)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Straight Segment");

                else if (generatedIndex <= 10)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Change Lane");

                else if (generatedIndex <= 12)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Straight Segment");

                else if (generatedIndex <= 16)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Jump");

                else if (generatedIndex <= 18)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Straight Segment");

                else if (generatedIndex <= 22)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Slide");

                else if (generatedIndex <= 24)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Straight Segment");

                else
                {
                    int randIndex = UnityEngine.Random.Range(0, BiomesData.Instance.current.Segments.Count);
                    return BiomesData.Instance.current.Segments[randIndex];
                }


            case "World 1 - Gray":
            case "World 2 - Red":


                if (generatedIndex <= 3)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Straight Segment");

                else
                    return BiomesData.Instance.current.Segments
                        .FindAll(s => s.name != "Straight Segment")
                        .PickRandom();

            default:

                if (generatedIndex <= 3)
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Straight Segment");

                else
                    return BiomesData.Instance.current.Segments
                        .Find(s => s.name == "Random Obstacles");

        }






    }


    /// <summary>
    /// Check if we need to generate or rm segments and do it
    /// </summary>
    private IEnumerator GenerationRoutine()
    {
        if(playerTransform == null) yield break; 
        
        // Generate new segments if needed
        int cursor = (int)playerTransform.position.z;
        int maxZ = cursor + frontGenerationWindowSize;
        while (cursor < maxZ)
        {
            // Check if segment at this z already exists
            var worldSegment = currentWorldSegments.Find(s => s.IsPositionInsideSegment(cursor));
            bool segmentExists = worldSegment != null;
            if (!segmentExists)
            {

                /*
                 *
                 * Add biome change overlay if biome changed 
                 */

                var segBiomeName = BiomesData.Instance.current.BiomeName;
                var lastSeg = currentWorldSegments.LastOrDefault();
                if (lastSeg != null && (segBiomeName != lastInstantiatedBiomeName))
                {
                    WorldStartOverlay.Instance.Set(
                        lastSeg.position + new Vector3(0, 0, lastSeg.sizeZ)
                        , BiomesData.Instance.current.ColorSkyGround
                    );
                    generatedIndex = 0; // reset index on biome change
                }
                lastInstantiatedBiomeName = segBiomeName;

                /*
                 *
                 * Create new segment 
                 */

                generatedIndex++;
                var lastSegment = currentWorldSegments.LastOrDefault();
                var zTarget = cursor;
                if (lastSegment != null) zTarget = lastSegment.rangeZ.Item2;
                var segmentPrefab = PickWorldSegmentPrefab().prefab;
                var segmentInstance = Instantiate(segmentPrefab, new Vector3(0f, 0, zTarget), Quaternion.identity);
                var sidewalkGenerators = segmentInstance.GetComponentsInChildren<SidewalkGenerator>();
                segmentInstance.name += $"| {generatedIndex} - {BiomesData.Instance.current.BiomeName}";
                foreach (var sidewalkGenerator in sidewalkGenerators) sidewalkGenerator.Generate();
                segmentInstance.transform.parent = transform;
                worldSegment = new WorldSegment() { position = new Vector3(0f, 0f, zTarget), prefab = segmentInstance };
                worldSegment.CalcInstanceData(segmentInstance);

                /*
                 *
                 * Special case: Remove crystals in tutorial biome segments until certain index
                 */

                if ((BiomesData.Instance.current.BiomeName == "Tutorial")
                && (generatedIndex <= 24))
                {
                    RemoveBiomeCrystals("Biome Data - World 0 - Tuto");
                }


                /*
                 *
                 * no need to block frame, each segment can be created in its own frame 
                 */

                if (Application.isPlaying)
                    yield return null;


                /*
                *
                * Finalize segment addition 
                */

                currentWorldSegments.Add(worldSegment);
            }

            if (worldSegment == null)
            {
                Debug.LogError("[WorldGenerationManager] World segment is null after instantiation!");
                break;
            }
            if (worldSegment.sizeZ <= 0)
            {
                Debug.LogError("[WorldGenerationManager] World segment sizeZ is invalid after calculation!");
                break;
            }
            cursor += worldSegment.sizeZ;
        }

        // Remove all segments that are behind the player
        ClearSegmentsBehindPlayer();

        // Update colliders
        SquareCollidersMerger.Instance.GenerateSquareColliders();

        // Restart coroutine
        yield return new WaitForEndOfFrame();
        GenerateSegments(clearExisting: false);
    }



    /// <summary>
    ///   Remove world segments that are behind the player
    /// </summary>
    public void ClearSegmentsBehindPlayer()
    {

        currentWorldSegments.RemoveAll(s =>
        {
            if ((s.position.z + s.sizeZ) < lastRecordZPosition - 10) // 10 offset to avoid removing too early
            {
                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(s.prefab);
                else
                    Destroy(s.prefab);

                return true;
            }
            return false;
        });
    }

    /// <summary>
    ///     Remove world segments that are in front of the player up to given distance
    /// </summary>
    /// <param name="distanceAhead"> Offset distance ahead of player to clear segments from.
    /// This params means: Preserve segments if they are close enough to player </param>
    public void ClearSegmentsInFrontPlayer(int distanceAhead)
    {
        currentWorldSegments.RemoveAll(s =>
        {
            if (s.position.z > (playerTransform.position.z + distanceAhead))
            {
                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(s.prefab);
                else
                    Destroy(s.prefab);

                return true;
            }
            return false;
        });
    }


    /// <summary>
    ///   Remove all crystals that belong to given biome name
    /// </summary>
    /// <param name="biomeName"></param>
    public void RemoveBiomeCrystals(string biomeName)
    {
        var toDestroy = new List<GameObject>();
        var biomeSegments = BiomesData.Instance.items
                                .Find(b => b.name == biomeName)
                                .Segments;

        foreach (var segment in biomeSegments)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("Crystal"))
                {
                    toDestroy.Add(child.gameObject);
                }
            }
        }

        foreach (var obj in toDestroy)
        {
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(obj);
            else
                Destroy(obj);
        }
    }
}
