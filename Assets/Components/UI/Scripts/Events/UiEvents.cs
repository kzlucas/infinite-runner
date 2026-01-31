using System;
using Components.Events;
using Unity.VisualScripting;

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
