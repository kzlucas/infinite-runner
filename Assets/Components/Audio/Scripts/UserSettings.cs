
using UnityEngine;
using Components.Events;

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

        public void ApplyToSources(AudioSource musicSource, AudioSource sfxSource)
        {
            if (musicSource != null)
            {
                musicSource.mute = !MusicEnabled;
                musicSource.volume = MusicEnabled ? 0.7f : 0f;
            }

            if (sfxSource != null)
            {
                sfxSource.mute = !SfxEnabled;
                sfxSource.volume = SfxEnabled ? 1f : 0f;
            }
        }

        private void SaveSetting(string key, string value)
        {
            PlayerPrefService.Save(key, value);
        }
    }
}
