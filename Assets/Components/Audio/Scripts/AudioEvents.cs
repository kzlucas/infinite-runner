using Components.Events;

namespace Components.Audio.Scripts
{
    public struct PlaySoundEvent : IGameEvent
    {
        public string SoundName;
        public float Volume;
        
        public PlaySoundEvent(string name, float vol = 1f)
        {
            SoundName = name;
            Volume = vol;
        }
    }

    public struct PlayMusicEvent : IGameEvent
    {
        public string SceneName;
        
        public PlayMusicEvent(string scene)
        {
            SceneName = scene;
        }
    }

    public struct AudioSettingsChangedEvent : IGameEvent
    {
        public bool MusicEnabled;
        public bool SfxEnabled;
        
        public AudioSettingsChangedEvent(bool music, bool sfx)
        {
            MusicEnabled = music;
            SfxEnabled = sfx;
        }
    }
}