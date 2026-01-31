using System.Collections;
using InputsHandler;
using UnityEngine;
using UnityEngine.InputSystem;
            


namespace Components.Editor.EditorExtra
{
    public class EditorExtras : Singleton.Model<EditorExtras>
    {
#if UNITY_EDITOR

        [Header("Dependencies")]
        private InputHandlersManager InputHandlersManager => ServiceLocator.Scripts.ServiceLocator.Get<InputHandlersManager>();


        [Header("References")]
        public InputActionReference reloadSceneActionRef;



        private void Start() => SceneLoader.Instance.OnSceneLoaded += OnSceneLoaded;
        private void OnSceneLoaded() => StartCoroutine(HandleSceneLoaded());

        private IEnumerator HandleSceneLoaded()
        {
            yield return new WaitUntil(() => SceneInitializer.Instance.isInitialized);
            RegisterHandlers();
        }


        private void RegisterHandlers()
        {
            InputHandlersManager.Register("Reload Scene", reloadSceneActionRef, OnTrigger: () =>
            {
                SceneLoader.Instance?.ReloadCurrentScene();
            });

        }

#endif
    }
}