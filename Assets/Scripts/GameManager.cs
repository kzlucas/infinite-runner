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
            PaintManager.ClearBucket();
        };
    }

    private void Update()
    {
        if (isPaused) return;
        
        // gradually increase time scale over time. 100 seconds to reach 2x speed
        timeElapsedSinceStart += Time.unscaledDeltaTime;
        gameCurrentTimeScale = 1f + (timeElapsedSinceStart / 100f); 
        gameCurrentTimeScale = Mathf.Clamp(gameCurrentTimeScale, 0f, gameMaxTimeScale);
        Time.timeScale = gameCurrentTimeScale;
    }


    private void RegisterHandlers()
    {
        Debug.Log("[GameManager] Registering GameManager Input Handlers");

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
        isPaused = true;
        Time.timeScale = 0f;
    }

    /// <summary>
    ///     Resumes the game by restoring time scale to one.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = gameCurrentTimeScale;
    }


}