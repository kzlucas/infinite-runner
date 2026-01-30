using Components.ServiceLocator.Scripts;
using Components.UI.Scripts;
using InputsHandler;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiPauseMenu : UiPopin
{
        
    [Header("Dependencies")]
    private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();
    private InputHandlersManager InputHandlersManager => ServiceLocator.Get<InputHandlersManager>();
    


    [Header("Pause Menu Settings")]
    public bool isLocked = false;
    public InputActionReference pauseGameActionRef;


    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            InputHandlersManager.Register("Open Pause Menu", pauseGameActionRef, OnTrigger: () =>
            {
                if(isLocked) return;
                Debug.Log("[UiPauseMenu] Toggling Pause Menu");
                UiRegistry.pauseMenu.Toggle();
            });
        };
    }

    public override void OnOpen()
    {
        TimeScaleManager.Instance.PauseGame();
    }

    public override void OnClose()
    {
        TimeScaleManager.Instance.ResumeGame();
    }

}