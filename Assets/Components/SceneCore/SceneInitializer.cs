using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Components.Events;
using Components.Scenes;


/// <summary>
/// SceneInitializer is responsible for initializing all IInitializable objects in the scene
/// in the correct order based on their dependencies.
/// </summary>
public class SceneInitializer : Singleton.Model<SceneInitializer>
{
    /// <summary>
    /// This flag indicates whether the scene has been initialized
    /// </summary>
    public bool isInitialized = false;


    /// <summary>
    ///  Called when entering Play mode.
    /// - Unregister handlers so it doesn't affect the next Play mode run
    /// This is needed when domain reloading is disabled in Unity Editor.
    /// @see https://docs.unity3d.com/6000.2/Documentation/Manual/domain-reloading.html
    /// </summary>
    [RuntimeInitializeOnLoadMethod] 
    static void OnEnteringPlayMode()
    {
        EventBus.Unsubscribe<SceneLoadedEvent>(Instance.OnSceneLoadedEvent);
        Instance.isInitialized = false;
    }

    private void Start() => EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
    private void OnDestroy() => EventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);



    /// <summary>
    /// Initialize all IInitializable objects in the scene
    /// </summary>
    private void OnSceneLoadedEvent(SceneLoadedEvent e)
    {
        _ = InitializeSceneAsync(e);
    }
    private async Task InitializeSceneAsync(SceneLoadedEvent e)
    {
        isInitialized = false;
        
        // Collect all IInitializable
        List<IInitializable> initializables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IInitializable>()
            .ToList();

        // resolve order
        List<IInitializable> ordered = ResolveInitializationOrder(initializables);

        // execute initialization in order
        foreach (var obj in ordered)
        {
            Debug.Log($"[SceneInitializer] ⏳ Initializing {obj.GetType().Name} timestamp: {System.DateTime.Now:T}");
            await InitOrTimeout(obj, 1000);
        }

        Debug.Log("[SceneInitializer] 🎉 All initializables are ready!");
        Instance.isInitialized = true;
    }


    /// <summary>
    /// Initialize an IInitializable with a timeout
    /// </summary>
    private async Task InitOrTimeout(IInitializable item, int timeoutMs)
    {
        var initTask = item.InitializeAsync();
        var delayTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(initTask, delayTask);

        if (completedTask == delayTask)
        {
            Debug.LogWarning($"[SceneInitializer] ⚠️ Initialization of {item.GetType().Name} timed out after {timeoutMs} ms");
        }
        else
        {
            await initTask; // Ensure any exceptions are observed
        }
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
        return result.OrderBy(i => i.InitPriority).ToList();
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

        if (item.InitDependencies != null)
        {
            foreach (var depType in item.InitDependencies)
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
