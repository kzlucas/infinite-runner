using System.Collections.Generic;
using UnityEngine;
using Utils;


namespace WorldGenerator.Scripts
{


    /// <summary>
    ///  Generator for sidewalk items
    /// </summary>
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
            foreach (GameObject slot in slotsContainer.FindChildWithTag("Slot"))
            {
                var prefab = prefabs[Random.Range(0, prefabs.Count)];
                var instance = Instantiate(prefab, slot.transform.position, Quaternion.identity, transform);
                instance.transform.SetParent(itemsContainer.transform);
                instance.transform.rotation = slot.transform.rotation;
            }

        }
    }
}
