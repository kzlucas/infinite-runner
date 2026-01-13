using System;
using System.Collections.Generic;
using System.Linq;
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
    
    /// <summary>Reference to the player transform for generation tracking</summary>
    public Transform playerTransform;

    /// <summary>Number of segments to generate in front of the player</summary>
    public int frontGenerationWindowSize = 100;

    /// <summary> Index for generated segments</summary>
    private int generatedIndex = 0;



    /// <summary>
    /// Get the needed references and setup for world generation
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync()
    {
        // Initial generation
        GenerateSegments();

        return Task.CompletedTask;
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
    /// Pick a world segment prefab to instantiate
    /// </summary>
    private WorldSegment pickWorldSegmentPrefab()
    {
        if(generatedIndex <= 1)
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
        
        // Generate new segments if needed
        int cursor = (int)playerTransform.position.z;
        int maxZ = cursor + frontGenerationWindowSize;
        while (cursor  < maxZ)
        {
            // Check if segment at this z already exists
            var worldSegment = currentWorldSegments.Find(s => s.IsPositionInsideSegment(cursor));
            bool segmentExists = worldSegment != null;
            if (!segmentExists)
            {
                // Create new segment
                generatedIndex++;
                var lastSegment = currentWorldSegments.LastOrDefault();
                var zTarget = cursor; 
                if (lastSegment != null) zTarget = lastSegment.rangeZ.Item2;
                var segmentPrefab = pickWorldSegmentPrefab().prefab;
                var segmentInstance = Instantiate(segmentPrefab, new Vector3(0f, 0, zTarget), Quaternion.identity);
                segmentInstance.transform.parent = transform;
                worldSegment = new WorldSegment() {position = new Vector3(0f, 0f, zTarget), prefab = segmentInstance };
                worldSegment.CalcInstanceData(segmentInstance);
                currentWorldSegments.Add(worldSegment);
            }

            if(worldSegment == null)
            {
                Debug.LogError("[WorldGenerationManager] World segment is null after instantiation!");
                break;
            }
            if(worldSegment.sizeZ <= 0)
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
    }



    /// <summary>
    ///   Remove world segments that are behind the player
    /// </summary>
    public void ClearSegmentsBehindPlayer()
    {
        currentWorldSegments.RemoveAll(s =>
        {
            if ((s.position.z + s.sizeZ) < playerTransform.position.z )
            {
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
