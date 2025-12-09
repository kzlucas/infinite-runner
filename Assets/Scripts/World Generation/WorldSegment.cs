using UnityEngine;


[System.Serializable]
public class WorldSegment
{
    public string name;
    public GameObject prefab;
    [HideInInspector] public Vector3 position;
}