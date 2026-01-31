using System.Collections.Generic;
using UnityEngine;


namespace WorldGenerator.Scripts
{

    /// <summary>
    ///   Data structure representing a world segment prefab
    ///   along with its runtime instance data.
    /// </summary>
    [System.Serializable]
    public class WorldSegment
    {
        public string Name;
        public GameObject GameObject;
        [HideInInspector] public SO_BiomeData BiomeData { get; set; }
        [HideInInspector] public Vector3 Position;
        [HideInInspector] public int SizeZ;
        [HideInInspector] public (int, int) RangeZ;


        /// <summary>
        /// Generate an instance of this world segment 
        /// with proper initialization
        /// </summary>
        public WorldSegment ToInstance(

              int zTarget
            , SO_BiomeData biomesData
            , Transform parent
        )
        {
            Debug.Assert(GameObject != null, "[WorldSegment] Prefab is null!");

            var segmentInstance = GameObject.Instantiate(GameObject, new Vector3(0f, 0f, zTarget), Quaternion.identity);
            var sidewalkGenerators = segmentInstance.GetComponentsInChildren<SidewalkGenerator>();
            segmentInstance.name += $" | z: {zTarget}";
            foreach (var sidewalkGenerator in sidewalkGenerators) sidewalkGenerator.Generate();
            segmentInstance.transform.parent = parent;
            var worldSegment = new WorldSegment()
            {
                Position = new Vector3(0f, 0f, zTarget)
                ,
                GameObject = segmentInstance
                ,
                BiomeData = biomesData
            };
            worldSegment.CalcInstanceData(segmentInstance);
            return worldSegment;
        }


        /// <summary>
        /// Calculate instance data such as size and range 
        /// of the segment in world units after instantiation
        /// </summary>
        /// <param name="instance"></param>
        public void CalcInstanceData(GameObject instance)
        {
            if (instance != null)
            {
                var markers = instance.transform.Find("Container/Markers");
                var startZ = (int)markers.Find("Start Marker").transform.position.z;
                var endZ = (int)markers.Find("End Marker").transform.position.z;

                SizeZ = (int)Mathf.Abs(endZ - startZ);
                RangeZ = (startZ, endZ);
            }
            else
            {
                Debug.LogError("[WorldSegment] Instance is null, cannot calculate data!");
            }
        }


        /// <summary>
        /// Check if given z position is inside this segment
        /// </summary>
        /// <param name="zPos"></param>
        /// <returns></returns>
        public bool IsPositionInsideSegment(float zPos)
        {
            return (zPos >= RangeZ.Item1 && zPos <= RangeZ.Item2);
        }



        /// <summary>
        ///   Remove all crystals
        /// </summary>
        public void RemoveCrystals()
        {
            var toDestroy = new List<GameObject>();
            Transform[] allChildren = GameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("Crystal"))
                {
                    toDestroy.Add(child.gameObject);
                }
            }

            foreach (var obj in toDestroy)
            {
                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(obj);
                else
                    GameObject.Destroy(obj);
            }
        }

    }
}