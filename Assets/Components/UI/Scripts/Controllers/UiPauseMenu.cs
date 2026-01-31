using Components.Events;
using Components.Scenes;
using Components.TimeScale;
using InputsHandler;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public class UiPauseMenu : BaseClasses.UiPopin
    {

        [Header("Dependencies")]
        private InputHandlersManager InputHandlersManager => ServiceLocator.Scripts.ServiceLocator.Get<InputHandlersManager>();



        [Header("Pause Menu Settings")]
        public bool IsLocked = false;
        public InputActionReference PauseGameActionRef;


        private void Start() => EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
        private void OnDestroy() => EventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);            
        private void OnSceneLoadedEvent(SceneLoadedEvent e)
        {
            InputHandlersManager.Register("Open Pause Menu", PauseGameActionRef, OnTrigger: () =>
            {
                if (IsLocked) return;
                Debug.Log("[UiPauseMenu] Toggling Pause Menu");
                UiRegistry.Instance.PauseMenu.Toggle();
            });
        }


        public override void OnOpen()
        {
            Debug.Log("[UiPauseMenu] Pausing Game " + GetInstanceID().ToString());
            TimeScaleManager.Instance.PauseGame();
        }

        public override void OnClose()
        {
            TimeScaleManager.Instance.ResumeGame();
        }

    }
}