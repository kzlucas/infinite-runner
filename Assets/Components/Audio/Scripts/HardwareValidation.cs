
using UnityEngine;

namespace Components.Audio.Scripts
{
    public class HardwareValidation
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;


        public (AudioSource, AudioSource) AttachAudioSources(Camera cam)
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

            if (musicSource == null)
            {
                musicSource = audioSources[0];
                musicSource.loop = true;
            }
            // Ensure there is an AudioSource for SFX
            if (sfxSource == null)
            {
                sfxSource = audioSources[1];
                sfxSource.loop = false;
            }

            return (musicSource, sfxSource);
        }
    }
}
