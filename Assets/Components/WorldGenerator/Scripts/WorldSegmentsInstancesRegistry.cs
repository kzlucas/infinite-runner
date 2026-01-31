
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerator.Scripts
{
    /// <summary>
    ///  Registry for all instantiated world segments
    /// </summary>
    public static class InstancesRegistry
    {
        private static List<WorldSegment> instances = new List<WorldSegment>();


        public static void RegisterInstance(WorldSegment segment)
        {
            instances.Add(segment);
        }

        public static void Clear()
        {
            instances.Clear();
            foreach (var seg in GameObject.FindGameObjectsWithTag("World Segment"))
                GameObject.DestroyImmediate(seg);
        }

        public static WorldSegment FindByZ(int zPosition)
        {
            return instances.Find(s => s.IsPositionInsideSegment(zPosition));
        }

        public static WorldSegment LastOrDefault()
        {
            if (instances.Count == 0) return null;
            return instances[instances.Count - 1];
        }

        public static WorldSegment PreviousOrDefault()
        {
            if (instances.Count < 2) return null;
            return instances[instances.Count - 2];
        }


        /// <summary>
        ///   Remove crystals from all segments of given biome
        /// </summary>
        public static void RemoveBiomeCrystals(string biomeName)
        {   
            foreach (var segment in instances)
            {
                var bn = segment.BiomeData;
                if(bn.BiomeName == biomeName)
                {
                    segment.RemoveCrystals();
                }
            }
        }

        /// <summary>
        ///   Remove world segments that are behind the player
        /// </summary>
        public static void ClearSegmentsBehind(float z)
        {
            instances.RemoveAll(s =>
            {
                if (s.position.z < z) // 10 offset to avoid removing too early
                {
                    if (Application.isEditor && !Application.isPlaying)
                        GameObject.DestroyImmediate(s.GameObject);
                    else
                        GameObject.Destroy(s.GameObject);

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
        public static void ClearSegmentsInFront(int z)
        {
            instances.RemoveAll(s =>
            {
                if (s.position.z > z)
                {
                    if (Application.isEditor && !Application.isPlaying)
                         GameObject.DestroyImmediate(s.GameObject);
                    else
                         GameObject.Destroy(s.GameObject);

                    return true;
                }
                return false;
            });
        }
    }
}