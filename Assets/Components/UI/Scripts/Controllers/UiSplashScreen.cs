using Components.Events;
using Components.Scenes;
using UnityEngine;
using UnityEngine.UIElements;


namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public class UiSplashScreen : BaseClasses.UiController
    {

        public override void OnDocReady()
        {
            EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
            OnSceneLoadedEvent(new SceneLoadedEvent(SceneLoader.Instance.GetSceneName()));
        }


        private void OnSceneLoadedEvent(SceneLoadedEvent evt)
        {

            root.style.display = SceneLoader.Instance.GetSceneName() == "Splash Screen"
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
        }

    }
}