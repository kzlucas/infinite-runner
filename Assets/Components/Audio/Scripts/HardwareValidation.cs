
using UnityEngine;

namespace Components.Audio.Scripts
{
    public class HardwareValidation
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;


        public (AudioSource, AudioSource) AttachAudioSources(UnityEngine.Camera cam)
        {
            if (cam == null)
            {
                Debug.LogError("[Audio.HardwareValidation] No main camera found in the scene!");
                return (null, null);
            }

            var audioSources = cam.GetComponents<AudioSource>();
            if (audioSources.Length < 2)
            {
                // Add missing AudioSources if needed
                for (int i = audioSources.Length; i < 2; i++)
                {
                    cam.gameObject.AddComponent<AudioSource>();
                }
            }
            audioSources = cam.GetComponents<AudioSource>();

            if (_musicSource == null)
            {
                _musicSource = audioSources[0];
            }
            // Ensure there is an AudioSource for SFX
            if (_sfxSource == null)
            {
                _sfxSource = audioSources[1];
            }
            
            _musicSource.loop = true;
            _sfxSource.loop = false;
            return (_musicSource, _sfxSource);
        }
    }
}
