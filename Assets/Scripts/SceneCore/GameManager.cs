using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : Singleton<GameManager>
{
    private bool isPaused = false;
    private float gameCurrentTimeScale = 1f; 
    private float gameMaxTimeScale = 2f; 
    private float timeElapsedSinceStart = 0f;
    public InputActionReference reloadSceneActionRef;


    private void OnDisable() => StopAllCoroutines();

    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            timeElapsedSinceStart = 0f;
            StopAllCoroutines();
            RegisterHandlers();
            if(SceneLoader.Instance.IsGameScene()) PaintManager.Reset();
        };
    }

    private void Update()
    {
        if (isPaused) return;
        
        // gradually increase time scale over time. 400 seconds to reach 2x speed
        timeElapsedSinceStart += Time.unscaledDeltaTime;
        gameCurrentTimeScale = 1f + (timeElapsedSinceStart / 400f); 
        gameCurrentTimeScale = Mathf.Clamp(gameCurrentTimeScale, 0f, gameMaxTimeScale);
        Time.timeScale = gameCurrentTimeScale;
    }


    private void RegisterHandlers()
    {
#if UNITY_EDITOR
        InputHandlersManager.Instance.Register("Reload Scene", reloadSceneActionRef, OnTrigger: () =>
        {
            SceneLoader.Instance?.ReloadCurrentScene();
        });
#endif
    }


    /// <summary>
    ///     Pauses the game by setting time scale to zero.
    /// </summary>  
    public void PauseGame()
    {
        Debug.Log("[GameManager] Pausing game");
        isPaused = true;
        Time.timeScale = 0f;
    }

    /// <summary>
    ///     Resumes the game by restoring time scale to one.
    /// </summary>
    public void ResumeGame()
    {
        Debug.Log("[GameManager] Resuming game");
        isPaused = false;
        Time.timeScale = gameCurrentTimeScale;
    }


}