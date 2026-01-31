using System.Collections;
using Components.Events;
using Components.Scenes;
using UnityEngine;
using UnityEngine.UIElements;


namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public class UiScreenOverlay : BaseClasses.UiController, IOpenable
    {
        public string ColorString = "black";
        public bool IsOpen { get; set; } = true;


        private void Start() => EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
        private void OnDestroy() => EventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);            
        private void OnSceneLoadedEvent(SceneLoadedEvent e)
        {
            if (this == null || gameObject == null) return;
            StartCoroutine(OnSceneLoaded());
        }



        private IEnumerator OnSceneLoaded()
        {
            yield return new WaitUntil(() => DocReady);
            var screen = root.Q<VisualElement>("screen");
            screen.AddToClassList(ColorString);
            screen.RemoveFromClassList("white");
            screen.RemoveFromClassList("fade-out");
            screen.RemoveFromClassList("fade-in");
            screen.MarkDirtyRepaint();
            yield return new WaitForEndOfFrame();
            Close();
        }

        public void Open()
        {
            var screen = root.Q<VisualElement>("screen");
            screen.AddToClassList(ColorString);
            screen.AddToClassList("fade-in");
            screen.RemoveFromClassList("fade-out");
            IsOpen = true;
        }

        public void Close()
        {
            var screen = root.Q<VisualElement>("screen");
            screen.AddToClassList(ColorString);
            screen.AddToClassList("fade-out");
            screen.RemoveFromClassList("fade-in");
            IsOpen = false;
        }


        public void Flash(string color, float duration = 0.1f)
        {
            StartCoroutine(_Flash(color, duration));
        }
        private IEnumerator _Flash(string color, float fadesDuration = .1f, float pauseDuration = 0f)
        {
            yield return new WaitUntil(() => DocReady);
            var screen = root.Q<VisualElement>("screen");
            screen.RemoveFromClassList(ColorString);
            screen.AddToClassList(color);
            screen.AddToClassList("fade-in-fast");
            screen.RemoveFromClassList("fade-out-fast");
            yield return new WaitForSecondsRealtime(fadesDuration + pauseDuration);
            screen.AddToClassList("fade-out-fast");
            screen.RemoveFromClassList("fade-in-fast");
            yield return new WaitForSecondsRealtime(fadesDuration);
            screen.RemoveFromClassList(color);
            screen.AddToClassList(ColorString);
        }
    }
}