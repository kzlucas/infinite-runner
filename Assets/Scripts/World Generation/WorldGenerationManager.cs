using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldGenerationManager : MonoBehaviour, IInitializable
{
    
    public int initPriority => 1;
    public Type[] initDependencies => new Type[] { typeof(BiomesData) };



    /// <summary>Current world segments</summary>
    private List<WorldSegment> currentWorldSegments = new List<WorldSegment>();

    /// <summary>Represent the area in which world segment has to be spawn on game Update</summary>
    private Bounds generationBoundary;

    /// <summary>Size of each generated segment grid (in world units)</summary>
    private int segmentGridSize = 10;
    
    /// <summary>Reference to the player transform for generation tracking</summary>
    private Transform playerTransform;

    /// <summary>Number of segments to generate in front of the player</summary>
    public int generateCountInFront = 10;

    /// <summary> Index for generated segments</summary>
    private int generatedIndex = 0;


    private void Start()
    {
        // Initial generation
        GenerateSegments();
    }


    /// <summary> Generate segments</summary>
    public void GenerateSegments()
    {
        generatedIndex = 0;
        
        // destroy existing segments
        currentWorldSegments.Clear();
        foreach(var seg in GameObject.FindGameObjectsWithTag("World Segment"))
            DestroyImmediate(seg);
            
        // generate
        Update();
    }


    /// <summary>
    /// Get the needed references and setup for world generation
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync()
    {
        // Get player position reference
        if(playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (playerTransform == null)
                Debug.LogError("[WorldGenerationManager] Player object with tag 'Player' not found in the scene.");
        }

        // Initial generation
        GenerateSegments();

        return Task.CompletedTask;
    }



    /// <summary>
    /// Pick a world segment prefab to instantiate
    /// </summary>
    private WorldSegment pickWorldSegmentPrefab()
    {
        if(generatedIndex <= 10000)
            return BiomesData.Instance.current
                    .segments.Find(s => s.name == "Straight Segment");

        else 
            return BiomesData.Instance.current
                    .segments.Find(s => s.name == "Random Obstacles");
    }


    /// <summary>
    /// Check if we need to generate or rm segments and do it
    /// </summary>
    private void Update()
    {
        if(playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        generationBoundary = new Bounds(
            new Vector3(
                playerTransform.position.x
                , playerTransform.position.y
                , playerTransform.position.z + (generateCountInFront / 2f * 10f)
            ),
            new Vector3(10f, 0f, generateCountInFront * 10f)
        );

        // Generate new segments if needed
        int pz = (int)(Math.Ceiling(playerTransform.position.z / segmentGridSize) * segmentGridSize);
        float mz = (int)(Math.Ceiling(generationBoundary.max.z / segmentGridSize) * segmentGridSize);
        for (int z = pz; z < mz; z += segmentGridSize)
        {
            // Check if segment at this z already exists
            bool segmentExists = currentWorldSegments.Exists(s => s.position.z == z);
            if (!segmentExists)
            {
                var segmentPrefab = pickWorldSegmentPrefab().prefab;

                // Create new segment
                generatedIndex++;
                var segmentObj = Instantiate(segmentPrefab, new Vector3(0f, 0, z), Quaternion.identity);
                segmentObj.transform.parent = transform;
                currentWorldSegments.Add(new WorldSegment() { position = new Vector3(0f, 0f, z), prefab = segmentObj });


                // Update colliders
                SquareCollidersMerger.Instance.GenerateSquareColliders();
            }
        }

        // Remove all segments that are behind the player
        currentWorldSegments.RemoveAll(s =>
        {
            if (s.position.z < playerTransform.position.z - segmentGridSize)
            {
                Destroy(s.prefab);
                return true;
            }
            return false;
        });
    }


    /// <summary>
    /// Draw generation boundaries to visualize generation area
    /// </summary>
    private void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(generationBoundary.center, generationBoundary.size);            
        }
    }
}
