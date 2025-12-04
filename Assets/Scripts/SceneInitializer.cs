using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

public class SceneInitializer : MonoBehaviour
{
    private void Awake()
    {
        _ = InitializeSceneAsync();
    }

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
