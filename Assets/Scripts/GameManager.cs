using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{


    public InputActionReference reloadSceneActionRef;
    private void Start()
    {
        InputHandlersManager.Instance.Register("Reload Scene", reloadSceneActionRef, OnTrigger: () =>
        {
            SceneLoader.Instance?.ReloadCurrentScene();
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