using Components.Events.Interfaces;

namespace Components.Events
{


    public struct OnRunStart: IGameEvent
    {
        public OnRunStart(int runNumber)
        {
        }
    }


    // /// <summary>
    // /// Audio events
    // /// </summary>
    // public struct PlaySoundEvent : IGameEvent
    // {
    //     public string soundName;
    //     public float volume;
        
    //     public PlaySoundEvent(string name, float vol = 1f)
    //     {
    //         soundName = name;
    //         volume = vol;
    //     }
    // }
}