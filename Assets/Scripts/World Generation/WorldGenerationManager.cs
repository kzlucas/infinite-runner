using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldGenerationManager : MonoBehaviour, IInitializable
{
    
    public int Priority => 0;
    public Type[] Dependencies => null;


    /// <summary>Reference to the player transform for generation tracking</summary>
    public Transform playerTransform;

    /// <summary>Current world segments</summary>
    public List<WorldSegment> segments = new List<WorldSegment>();

    /// <summary>Represent the area in which world segment has to be spawn on game Update</summary>
    private Bounds generationBoundary;

    /// <summary>Size of each generated segment grid (in world units)</summary>
    public int segmentGridSize = 10;
    
    /// <summary>Segment prefab to instantiate</summary>
    public GameObject segmentPrefab; 


    /// <summary> Gernerate segments</summary>
    [ContextMenu("Generate Segments")]
    private void Start()
    {
        // destroy existing segments
        segments.Clear();
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
        return Task.CompletedTask;
    }



    /// <summary>
    /// Check if we need to generate or rm segments and do it
    /// </summary>
    private void Update()
    {
        generationBoundary = new Bounds(
            new Vector3(
                playerTransform.position.x
                , playerTransform.position.y
                , playerTransform.position.z + 25f
            ),
            new Vector3(10f, 0f, 50f)
        );

        // Generate new segments if needed
        int pz = (int)(Math.Ceiling(playerTransform.position.z / segmentGridSize) * segmentGridSize);
        float mz = (int)(Math.Ceiling(generationBoundary.max.z / segmentGridSize) * segmentGridSize);
        for (int z = pz; z < mz; z += segmentGridSize)
        {
            // Check if segment at this z already exists
            bool segmentExists = segments.Exists(s => s.position.z == z);
            if (!segmentExists)
            {
                // Create new segment
                var segmentObj = Instantiate(segmentPrefab, new Vector3(0f, 0f, z), Quaternion.identity);
                segmentObj.transform.parent = transform;
                segments.Add(new WorldSegment() { position = new Vector3(0f, 0f, z), segmentObj = segmentObj });
            }
        }

        // Remove all segments that are behind the player
        segments.RemoveAll(s =>
        {
            if (s.position.z < playerTransform.position.z - segmentGridSize)
            {
                Destroy(s.segmentObj);
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
