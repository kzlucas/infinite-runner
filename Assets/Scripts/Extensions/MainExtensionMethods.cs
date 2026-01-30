using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainExtensionMethods
{
    /// <summary>
    /// Find a child GameObject with the specified tag.
    /// </summary>
    /// <param name="parent">The parent GameObject to search within.</param>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>The first child GameObject with the specified tag, or null if not found.</returns>
    public static List<GameObject> FindChildWithTag(this GameObject parent, string tag)
    {
        var matchedChildren = new List<GameObject>();
        foreach (Transform transform in parent.transform)
        {
            if (transform.CompareTag(tag))
            {
                matchedChildren.Add(transform.gameObject);
            }
        }
        return matchedChildren;
    }


    /// <summary>
    /// Pick a random element from a list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to pick from.</param>
    /// <returns>A random element from the list.</returns>
    public static T PickRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogError("Cannot pick a random element from an empty or null list.");
            return default(T);
        }
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }




}
