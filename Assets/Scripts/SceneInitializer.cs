using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;


/// <summary>
/// SceneInitializer is responsible for initializing all IInitializable objects in the scene
/// in the correct order based on their dependencies.
/// </summary>
public class SceneInitializer : Singleton<SceneInitializer>
{
    public override void Awake()
    {
        base.Awake();
        SceneLoader.Instance.OnSceneLoaded += () => _ = InitializeSceneAsync();
    }



    /// <summary>
    /// Initialize all IInitializable objects in the scene
    /// </summary>
    private async Task InitializeSceneAsync()
    {
        // Collect all IInitializable
        List<IInitializable> initializables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IInitializable>()
            .ToList();

        // Résoudre l'ordre en fonction des dépendances
        List<IInitializable> ordered = ResolveInitializationOrder(initializables);

        // Exécuter dans l'ordre
        foreach (var obj in ordered)
        {
            await obj.InitializeAsync();
        }

        Debug.Log("[SceneInitializer] 🎉 All initializables are ready!");
    }

    /// <summary>
    /// Resolve initialization order based on dependencies using Topological Sort
    /// </summary>
    private List<IInitializable> ResolveInitializationOrder(List<IInitializable> items)
    {
        var result = new List<IInitializable>();
        var visited = new HashSet<IInitializable>();

        foreach (var item in items)
        {
            Visit(item, visited, result, items);
        }

        // At last, sort by Priority
        return result.OrderBy(i => i.Priority).ToList();
    }


    /// <summary>
    /// Depth-First Search to visit dependencies
    /// </summary>
    private void Visit(
        IInitializable item,
        HashSet<IInitializable> visited,
        List<IInitializable> result,
        List<IInitializable> allItems)
    {
        if (visited.Contains(item))
            return;

        visited.Add(item);

        if (item.Dependencies != null)
        {
            foreach (var depType in item.Dependencies)
            {
                var dep = allItems.FirstOrDefault(i => i.GetType() == depType);

                if (dep == null)
                    Debug.LogWarning($"[SceneInitializer] Dependency {depType.Name} not found for {item.GetType().Name}");

                else
                    Visit(dep, visited, result, allItems);
            }
        }

        result.Add(item);
    }
}
