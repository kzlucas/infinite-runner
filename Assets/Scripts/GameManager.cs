using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{


    public InputActionReference reloadSceneActionRef;
    public InputActionReference pauseGameActionRef;


    private void Start()
    {


        #if UNITY_EDITOR
        InputHandlersManager.Instance.Register("Reload Scene", reloadSceneActionRef, OnTrigger: () =>
        {
            SceneLoader.Instance?.ReloadCurrentScene();
        });
        #endif

        InputHandlersManager.Instance.Register("Open Pause Menu", pauseGameActionRef, OnTrigger: () =>
        {
            UiManager.Instance.pauseMenu.Toggle();
        });

    }


    /// <summary>
    ///     Pauses the game by setting time scale to zero.
    /// </summary>
    public void PauseGame()
    {
        Debug.Log("[GameManager] Game Paused");
        Time.timeScale = 0f;
    }

    /// <summary>
    ///     Resumes the game by restoring time scale to one.
    /// </summary>
    public void ResumeGame()
    {
        Debug.Log("[GameManager] Game Resumed");
        Time.timeScale = 1f;
    }


}