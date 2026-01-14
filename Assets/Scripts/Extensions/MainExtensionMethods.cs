using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainExtensionMethods
{




    /// <summary>
    /// Replace a coroutine with a new instance, stopping the old one if it's running.
    /// </summary>
    /// <param name="routineInstance">The current coroutine instance.</param>
    /// <param name="routineModel">The new coroutine to start.</param>
    /// <returns>The new coroutine instance.</returns>
    public static IEnumerator Replace(this IEnumerator routineInstance, IEnumerator routineModel)
    {
        if (routineInstance != null)
        {
            // Stop the coroutine if it's already running
            GameManager.Instance.StopCoroutine(routineInstance);
        }

        // Restart the coroutine
        routineInstance = routineModel; 
        GameManager.Instance.StartCoroutine(routineInstance);
        return routineInstance;
    }

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
}
