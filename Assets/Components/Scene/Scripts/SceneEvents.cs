using Components.Events;

namespace Scene.Scripts
{
    public struct SceneLoadedEvent : IGameEvent
    {
        public string sceneName;
        
        public SceneLoadedEvent(string name)
        {
            sceneName = name;
        }
    }
}