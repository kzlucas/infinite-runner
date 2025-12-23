using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public InputActionReference reloadSceneActionRef;


    private void OnDisable() => StopAllCoroutines();

    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            StopAllCoroutines();
            RegisterHandlers();
        };
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
        Time.timeScale = 0f;
    }

    /// <summary>
    ///     Resumes the game by restoring time scale to one.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }


}