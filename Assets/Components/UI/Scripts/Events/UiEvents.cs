using Components.Events;

namespace Components.UI.Scripts.Events
{
    public class CountdownStarted : IGameEvent
    {
        public CountdownStarted(float timestamp)
        {
            
        }
    }
    public class CountdownFinished : IGameEvent
    {
        public CountdownFinished(float timestamp)
        {
        }
    }

}
