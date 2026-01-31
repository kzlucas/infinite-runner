


using Components.Events;

namespace Components.Scenes
{
    /// <summary>
    /// Event published when a scene has finished loading.
    /// </summary>
    public class SceneLoadedEvent: IGameEvent
    {
        public string SceneName { get; private set; }

        public SceneLoadedEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}