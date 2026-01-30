using UnityEngine;


[System.Serializable]
public class WorldSegment
{
    public string name;
    public GameObject prefab;
    [HideInInspector] public Vector3 position;
    [HideInInspector] public int sizeZ;
    [HideInInspector] public (int, int) rangeZ;


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

            sizeZ = (int)Mathf.Abs(endZ - startZ);
            rangeZ = (startZ, endZ);
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
        return (zPos >= rangeZ.Item1 && zPos <= rangeZ.Item2);
    }


    
}