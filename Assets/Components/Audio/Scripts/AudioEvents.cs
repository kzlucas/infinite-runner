using Components.Events;
using Interfaces;

namespace Components.Audio.Scripts
{
    public struct PlaySoundEvent : IGameEvent
    {
        public string soundName;
        public float volume;
        
        public PlaySoundEvent(string name, float vol = 1f)
        {
            soundName = name;
            volume = vol;
        }
    }

    public struct PlayMusicEvent : IGameEvent
    {
        public string sceneName;
        
        public PlayMusicEvent(string scene)
        {
            sceneName = scene;
        }
    }

    public struct AudioSettingsChangedEvent : IGameEvent
    {
        public bool musicEnabled;
        public bool sfxEnabled;
        
        public AudioSettingsChangedEvent(bool music, bool sfx)
        {
            musicEnabled = music;
            sfxEnabled = sfx;
        }
    }
}