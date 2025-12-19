using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public event Action OnSceneLoaded;
    private bool isTriggered = false;

    /// <summary>
    ///   Called when the script instance is being loaded.
    /// </summary>
    private void Start()
    {
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
    ///    Loads the main game scene.
    /// </summary>
    public void LoadGame()
    {
        Load("Game");
    }


    /// <summary>
    ///    Loads a scene asynchronously with transition effects.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneAsync(string name)
    {

        Debug.Log("[SceneLoader] Loading scene: " + name);

        // Run scene exit animation and wait for it to finish
        yield return FadeToBlack();

        // load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        OnSceneLoaded?.Invoke();
        isTriggered = false;
    }

    public IEnumerator FadeToBlack()
    {
        (UiManager.Instance.screenOverlay as IOpenable).Open();
        yield return new WaitForSecondsRealtime(.15f);
    }
    

    public string GetSceneName()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName;    
    }
}