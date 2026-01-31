using System.Collections;
using Components.Events;
using Components.Scenes;
using Components.ServiceLocator.Scripts;
using Components.UI.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton.Model<SceneLoader>
{

    [Header("Scene Loader Settings")]
    private bool isTriggered = false;
    public string currentSceneName => SceneManager.GetActiveScene().name;


    /// <summary>
    ///   Called when the script instance is being loaded.
    /// </summary>
    private IEnumerator Start()
    {
        // Make sur other components have put their OnSceneLoaded subscriptions before first invocation
        yield return new WaitForEndOfFrame();
        EventBus.Publish(new SceneLoadedEvent(currentSceneName));
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
        if (isTriggered)  return;// prevent multiple clicks
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
        SceneInitializer.Instance.isInitialized = false;
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

        EventBus.Publish(new SceneLoadedEvent(name));
        isTriggered = false;
    }

    public IEnumerator FadeToBlack()
    {
        yield return new WaitUntil(() => UiRegistry.Instance.IsReady);
        UiRegistry.Instance.ScreenOverlay.Open();
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