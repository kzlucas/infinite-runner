using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SidewalkGenerator : MonoBehaviour
{
    /// <summary>
    /// Positions to place items on sidewalks
    /// </summary>
    public GameObject slotsContainer;

    /// <summary>
    /// Container for instantiated items
    /// </summary>
    public GameObject itemsContainer;

    /// <summary>>
    /// Prefabs to place on sidewalks
    /// </summary>
    public List<GameObject> prefabs;


    private void Start()
    {
        Generate();
    }

    /// <summary>
    /// Generate items on sidewalk
    /// </summary>
    public void Generate()
    {

        // clear previous items if any (append in edit mode)
        foreach (Transform child in itemsContainer.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // get random prefab and instantiate at each slot
        foreach (var slot in slotsContainer.transform.GetComponentsInChildren<Transform>())
        {
            if (slot == slotsContainer.transform) continue; // skip container itself
            var prefab = prefabs[Random.Range(0, prefabs.Count)];
            var instance = Instantiate(prefab, slot.position, Quaternion.identity, transform);
            instance.transform.SetParent(itemsContainer.transform);
            instance.transform.rotation = slot.rotation;
        }

    }
}
