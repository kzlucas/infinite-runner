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
        public GameObject SlotsContainer;

        /// <summary>
        /// Container for instantiated items
        /// </summary>
        public GameObject ItemsContainer;

        /// <summary>>
        /// Prefabs to place on sidewalks
        /// </summary>
        public List<GameObject> Prefabs;


        /// <summary>
        /// Generate items on sidewalk
        /// </summary>
        public void Generate()
        {
            // clear previous items if any (append in edit mode)
            foreach (Transform child in ItemsContainer.transform)
            {
                DestroyImmediate(child.gameObject);
            }

            // get random prefab and instantiate at each slot
            foreach (GameObject slot in SlotsContainer.FindChildWithTag("Slot"))
            {
                var prefab = Prefabs[Random.Range(0, Prefabs.Count)];
                var instance = Instantiate(prefab, slot.transform.position, Quaternion.identity, transform);
                instance.transform.SetParent(ItemsContainer.transform);
                instance.transform.rotation = slot.transform.rotation;
            }

        }
    }
}
