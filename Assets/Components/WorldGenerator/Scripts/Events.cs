using WorldGenerator.Scripts;

namespace Components.Events
{
    public class BiomeChangedEvent: IGameEvent
    {
        public SO_BiomeData NewBiomeData { get; private set; }

        public BiomeChangedEvent(SO_BiomeData newBiomeData)
        {
            NewBiomeData = newBiomeData;
        }
    }
}