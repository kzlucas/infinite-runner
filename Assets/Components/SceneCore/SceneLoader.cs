using System;
using System.Collections;
using Components.Events;
using Components.Scenes;
using Components.ServiceLocator.Scripts;
using Components.UI.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton.Model<SceneLoader>
{
        
    [Header("Dependencies")]
    private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();



    [Header("Scene Loader Settings")]
    private bool isTriggered = false;
    public string currentSceneName => SceneManager.GetActiveScene().name;
    public event Action OnSceneLoaded;
    public event Action OnSceneExit;


    /// <summary>
    ///  Called when entering Play mode.
    /// - Unregister handlers so it doesn't affect the next Play mode run
    /// This is needed when domain reloading is disabled in Unity Editor.
    /// @see https://docs.unity3d.com/6000.2/Documentation/Manual/domain-reloading.html
    /// </summary>
    [RuntimeInitializeOnLoadMethod] 
    static void OnEnteringPlayMode()
    {
        Instance.OnSceneLoaded = null;
        Instance.OnSceneExit = null;
    }


    /// <summary>
    ///   Called when the script instance is being loaded.
    /// </summary>
    private IEnumerator Start()
    {
        // Make sur other components have put their OnSceneLoaded subscriptions before first invocation
        yield return new WaitForEndOfFrame();
        OnSceneLoaded?.Invoke();
    }
    
    /// <summary>
    ///   Checks if a scene with the given name exists in the build settings.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public bool SceneExists(string sceneName)
    {
        return Application.CanStreamedLevelBeLoaded(sceneName);
    }

    /// <summary>
    ///     Loads a scene asynchronously with transition effects.
    /// </summary>
    public void Load(string name)
    {
        if (isTriggered) // prevent multiple clicks
        {
            return;
        }

        isTriggered = true;
        StartCoroutine(LoadSceneAsync(name));
    }


    /// <summary>
    ///     Reloads the current active scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Load(currentSceneName);
    }




    /// <summary>
    ///    Loads a scene asynchronously with transition effects.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneAsync(string name)
    {

        Debug.Log("[SceneLoader] Loading scene: " + name);

        SceneInitializer.Instance.isInitialized = false;
        OnSceneExit?.Invoke();
        EventBus.Publish(new SceneExitEvent());


        // Run scene exit animation and wait for it to finish
        yield return FadeToBlack();

        // load scene
        ServiceLocator.Clear();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        OnSceneLoaded?.Invoke();
        EventBus.Publish(new SceneLoadedEvent(name));
        isTriggered = false;
    }

    public IEnumerator FadeToBlack()
    {
        yield return new WaitUntil(() => UiRegistry.IsReady);
        UiRegistry.ScreenOverlay.Open();
        yield return new WaitForSecondsRealtime(.15f);
    }
    

    public string GetSceneName()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName;    
    }

    public bool IsGameScene()
    {
        return currentSceneName == "Game";
    }
}