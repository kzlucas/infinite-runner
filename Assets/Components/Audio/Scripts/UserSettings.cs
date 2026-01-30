
using UnityEngine;

namespace Components.Audio.Scripts
{
    public class UserSettings
    {
        private AudioSource _musicSource;
        private AudioSource _sfxSource;


        private bool _musicOn = true;
        public bool MusicOn
        {
            get => _musicOn;
            set
            {
                _musicOn = value;
                PlayerPrefService.Save("music", _musicOn.ToString());
                if (!_musicOn)
                {
                    _musicSource.mute = true;
                    _musicSource.Pause();
                }
                else
                {
                    _musicSource.mute = false;
                    _musicSource.UnPause();
                }
            }
        }
        private bool _sfxOn = true;
        public bool SfxOn
        {
            get => _sfxOn;
            set
            {
                _sfxOn = value;
                PlayerPrefService.Save("sfx", _sfxOn.ToString());
                if (!_sfxOn)
                {
                    _sfxSource.mute = true;
                    _sfxSource.Pause();
                }
                else
                {
                    _sfxSource.mute = false;
                    _sfxSource.UnPause();
                }
            }
        }


        /// <summary>
        ///   Apply user audio settings from saved data.
        /// </summary>
        public void ApplyToSources(AudioSource musicSource, AudioSource sfxSource)
        {
            _musicSource = musicSource;
            _sfxSource = sfxSource;

            MusicOn = PlayerPrefService.Load("music") != "False";
            SfxOn = PlayerPrefService.Load("sfx") != "False";
        }


    }
}
