using System.Collections;
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

}
