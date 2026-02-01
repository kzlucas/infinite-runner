
using UnityEngine;
using Components.Events;
using Components.DataServices;

namespace Components.Audio.Scripts
{
    [System.Serializable]
    public class UserSettings
    {
        public bool MusicEnabled { get; private set; } = true;
        public bool SfxEnabled { get; private set; } = true;

        public void LoadSettings()
        {
            MusicEnabled = PlayerPrefService.Load("music") != "False";
            SfxEnabled = PlayerPrefService.Load("sfx") != "False";
        }

        public void SetMusicEnabled(bool enabled)
        {
            MusicEnabled = enabled;
            SaveSetting("music", enabled.ToString());
            
            // Publish event
            EventBus.Publish(new AudioSettingsChangedEvent(enabled, SfxEnabled));
        }

        public void SetSfxEnabled(bool enabled)
        {
            SfxEnabled = enabled;
            SaveSetting("sfx", enabled.ToString());
            
            // Publish event
            EventBus.Publish(new AudioSettingsChangedEvent(MusicEnabled, enabled));
        }

        public void ApplyToSources(AudioSource musicSource, AudioSource sfxSource, float volume)
        {
            if (musicSource != null)
            {
                musicSource.mute = !MusicEnabled;
                musicSource.volume = MusicEnabled ? volume : 0f;
            }

            if (sfxSource != null)
            {
                sfxSource.mute = !SfxEnabled;
                sfxSource.volume = SfxEnabled ? volume : 0f;
            }
        }

        private void SaveSetting(string key, string value)
        {
            PlayerPrefService.Save(key, value);
        }
    }
}
