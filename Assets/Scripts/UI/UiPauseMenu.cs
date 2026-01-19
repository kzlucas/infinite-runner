using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiPauseMenu : UiPopin
{
    public bool isLocked = false;
    public InputActionReference pauseGameActionRef;


    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            InputHandlersManager.Instance.Register("Open Pause Menu", pauseGameActionRef, OnTrigger: () =>
            {
                if(isLocked) return;
                Debug.Log("[UiPauseMenu] Toggling Pause Menu");
                UiManager.Instance.pauseMenu.Toggle();
            });
        };
    }

    public override void OnOpen()
    {
        GameManager.Instance.PauseGame();
    }

    public override void OnClose()
    {
        GameManager.Instance.ResumeGame();
    }

}