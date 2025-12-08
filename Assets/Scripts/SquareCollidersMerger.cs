using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
///  Merges square colliders in the scene that share at least 2 corner points into composite colliders
/// </summary>
public class SquareCollidersMerger : Singleton<SquareCollidersMerger>
{
    private List<Collider> squareColliders = new List<Collider>();
    private List<TypedCollider> typedColliders = new List<TypedCollider>();
    private class TypedCollider
    {
        public Collider collider;
        public ColliderType.Type colliderType;
        public List<Vector3> points;

        public TypedCollider(Collider collider, ColliderType.Type colliderType, List<Vector3> points)
        {
            this.collider = collider;
            this.colliderType = colliderType;
            this.points = points;
        }
    }

    /// <summary>
    ///  Helper class to store point information
    /// </summary>
    public class PointInfo
    {
        public int pointIndex;
        public float angle;
        public float distance;

        public PointInfo(int pointIndex, float angle, float distance)
        {
            this.pointIndex = pointIndex;
            this.angle = angle;
            this.distance = distance;
        }
    }

    /// <summary>
    /// Generate square colliders by merging existing ones in the scene
    /// </summary>
    public void GenerateSquareColliders()
    {
        // Clear existing lists
        squareColliders = new List<Collider>();
        typedColliders = new List<TypedCollider>();

        // Destroy existing merged colliders
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // Get the child at the current index
            Transform child = transform.GetChild(i);

            // Destroy the child GameObject
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
        }

        // Find all game objects with the tag "Square Collider" in the scene
        GameObject[] colliderObjects = GameObject.FindGameObjectsWithTag("Composite Square Collider");

        // Collect their colliders
        foreach (var obj in colliderObjects)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                squareColliders.Add(col);
                col.enabled = true;
            }
        }

        if (squareColliders.Count == 0)
        {
            Debug.LogWarning("[SquareCollidersMerger] No square colliders found with tag 'Composite Square Collider'.");
            return;
        }
        else
        {
            MergeColliders();
        }


        // Deactivate original collider objects
        foreach (var obj in colliderObjects)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }

    }


    /// <summary>
    ///  Merge square colliders that share at least 2 corner points
    /// </summary> 
    void MergeColliders()
    {
        // // Get execution time
        // DateTime startTime = DateTime.Now;

        // Collect points from each collider and group them
        foreach (var scol in squareColliders)
        {
            var scolPoints = GetColliderPoints(scol);

            // find in typedColliders another list that has at least 2 points matching
            int toMergeIndex = typedColliders.FindIndex(typedCollider =>
            {
                int matchCount = 0;
                foreach (var pt in scolPoints)
                {
                    if (typedCollider.points.Contains(pt))
                        matchCount++;
                }
                return matchCount >= 2;
            });

            // merge
            if (toMergeIndex != -1)
            {
                var pointsToMerge = typedColliders[toMergeIndex].points;
                pointsToMerge.AddRange(scolPoints);
                typedColliders[toMergeIndex].points = new List<Vector3>(new HashSet<Vector3>(pointsToMerge));
            }
            // add as new
            else
            {
                var colliderType = scol.GetComponent<ColliderType>().colliderType;
                typedColliders.Add(new TypedCollider(scol, colliderType, scolPoints));
            }
        }

        // Generate colliders from merged points
        foreach (var typedCollider in typedColliders)
        {
            GenerateColliderFromPointsList(typedCollider);
        }

        // // Log execution time
        // DateTime endTime = DateTime.Now;
        // TimeSpan duration = endTime - startTime;
        // Debug.Log($"[SquareCollidersMerger] Merging completed in {duration.TotalMilliseconds} ms");
    }


    /// <summary>
    ///   Get the corner points of a square / cube collider in world space
    /// </summary>
    List<Vector3> GetColliderPoints(Collider collider)
    {
        List<Vector3> colliderPoints = new List<Vector3>();
        Bounds bounds = collider.bounds;

        // World-space corners of the bounding box
        Vector3 minPoint = bounds.min; // Lower-left-back corner
        Vector3 maxPoint = bounds.max; // Upper-right-front corner  

        // Round to avoid floating point precision issues
        minPoint = new Vector3(Mathf.Round(minPoint.x * 1000f) / 1000f,
                               Mathf.Round(minPoint.y * 1000f) / 1000f,
                               Mathf.Round(minPoint.z * 1000f) / 1000f);
        maxPoint = new Vector3(Mathf.Round(maxPoint.x * 1000f) / 1000f,
                               Mathf.Round(maxPoint.y * 1000f) / 1000f,
                               Mathf.Round(maxPoint.z * 1000f) / 1000f);

        colliderPoints.Add(new Vector3(minPoint.x, minPoint.y, minPoint.z)); // Bottom-left-back
        colliderPoints.Add(new Vector3(maxPoint.x, minPoint.y, minPoint.z)); // Bottom-right-back
        colliderPoints.Add(new Vector3(minPoint.x, maxPoint.y, minPoint.z)); // Top-left-back
        colliderPoints.Add(new Vector3(maxPoint.x, maxPoint.y, minPoint.z)); // Top-right-back
        colliderPoints.Add(new Vector3(minPoint.x, minPoint.y, maxPoint.z)); // Bottom-left-front
        colliderPoints.Add(new Vector3(maxPoint.x, minPoint.y, maxPoint.z)); // Bottom-right-front
        colliderPoints.Add(new Vector3(minPoint.x, maxPoint.y, maxPoint.z)); // Top-left-front
        colliderPoints.Add(new Vector3(maxPoint.x, maxPoint.y, maxPoint.z)); // Top-right-front

        // Remove duplicate points (in case of flat colliders)
        HashSet<Vector3> uniquePoints = new HashSet<Vector3>(colliderPoints);
        colliderPoints = new List<Vector3>(uniquePoints);

        return colliderPoints;
    }

    /// <summary>
    ///   Generate a collider from a list of points in the 3D space
    ///   The collider needs to be convex for this to work properly
    /// </summary>
    Collider GenerateColliderFromPointsList(TypedCollider typedCollider)
    {
        // Remove duplicate points
        var points = new List<Vector3>(new HashSet<Vector3>(typedCollider.points));

        /// <summary>
        ///  Get the starting point index (lowest by y, then x, then z)
        /// </summary>
        int GetStartingPointIndex()
        {
            // lowest point by y, then by x, then by z
            int startingPointIndex = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].y < points[startingPointIndex].y ||
                    (Mathf.Approximately(points[i].y, points[startingPointIndex].y) && points[i].x < points[startingPointIndex].x) ||
                    (Mathf.Approximately(points[i].y, points[startingPointIndex].y) && Mathf.Approximately(points[i].x, points[startingPointIndex].x) && points[i].z < points[startingPointIndex].z))
                {
                    startingPointIndex = i;
                }
            }   
            return startingPointIndex;
        }


        /// <summary>
        ///  Get angles and distances from a point to other points given a previous direction
        /// </summary>
        List<PointInfo> GetAnglesAndDistances(Vector3 point, List<Vector3> otherPoints, Vector3 prevDir)
        {
            List<PointInfo> pointInfos = new List<PointInfo>();
            for (int i = 0; i < otherPoints.Count; i++)
            {
                if (otherPoints[i] == point)
                    continue;

                Vector3 dirToPoint = (point - otherPoints[i]).normalized;
                float angle = Vector3.Angle(dirToPoint, prevDir);
                float distance = Vector3.Distance(point, otherPoints[i]);
                pointInfos.Add(new PointInfo(i, angle, distance));
            }
            return pointInfos;
        }

        // swap first point with starting point
        var startingPointIndex = GetStartingPointIndex();
        (points[startingPointIndex], points[0]) = (points[0], points[startingPointIndex]);

        for (int i = 0; i < points.Count - 1; i++)
        {
            var point = points[i];
            Vector3 direction;
            
            if(i == 0) direction =  (point - new Vector3(point.x - 1f, point.y - 1, point.z - 1f)).normalized;
            else direction = (points[i - 1] - point).normalized;

            var pointsInfo = GetAnglesAndDistances(point, points, direction);
            var minAngle = pointsInfo.Min(pi => pi.angle);
            var candidates = pointsInfo.Where(pi => Mathf.Approximately(pi.angle, minAngle)).ToList();
            var selectedPointInfo = candidates.OrderBy(pi => pi.distance).First();

            // Swap next point with closest
            (points[selectedPointInfo.pointIndex], points[i + 1]) = (points[i + 1], points[selectedPointInfo.pointIndex]);

            // Debug.Log($"[SquareCollidersMerger] " 
            //     + $"  |  {i}"
            //     + $"  |  Prev coords: {points[(i - 1) != -1 ? (i - 1) : 0]}"
            //     + $"  |  Curr coords: {points[i]}"
            //     + $"  |  Angle: {selectedPointInfo.angle}"
            //     + $"  |  Dist: {selectedPointInfo.distance}"
            //     + $"  |  Candidates: {candidates.Count}"
            // );
        }


        // Check if point is colinear, if so remove it
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Vector3 prevPoint = i == 0 ? points[points.Count - 1] : points[i - 1];
            Vector3 nextPoint = i == points.Count - 1 ? points[0] : points[i + 1];
            Vector3 dirToPrev = (prevPoint - points[i]).normalized;
            Vector3 dirToNext = (nextPoint - points[i]).normalized;
            float angle = Vector3.Angle(dirToPrev, dirToNext);
            if (Mathf.Approximately(angle, 180f))
            {
                points.RemoveAt(i);
            }
        }


        Mesh mesh = new Mesh();
        mesh.SetVertices(points);

        // Simple triangulation assuming points form a convex shape
        // Draw straight lines from first vertex to every other non-adjacent vertex.
        List<int> triangles = new List<int>();
        for (int i = 1; i < points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i);
        }

        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.Optimize();


        // Create GameObject with MeshCollider
        GameObject go = new GameObject("MergedCollider");
        MeshCollider meshCollider = go.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        go.transform.parent = this.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        // Set collider type
        var colliderTypeComp = go.AddComponent<ColliderType>();
        colliderTypeComp.colliderType = typedCollider.colliderType;

        return meshCollider;
    }
}
