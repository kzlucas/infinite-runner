using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace Components.Audio.Scripts
{
    public class AudioManager : Singleton<AudioManager>, IInitializable
    {

        [Header("References")]
        private AudioMapping _audioMapping = new();
        private HardwareValidation _hardwareValidation = new();
        public UserSettings UserSettings = new();
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;



        [Header("Initialisation")]
        public int initPriority => 0;
        public System.Type[] initDependencies => null;



        [Header("Config")]
        public SO_AudioConfig audioConfig;



        [Header("Settings")]
        private bool isFading = false;



        public Task InitializeAsync()
        {
            // Map audio config data
            audioConfig = _audioMapping.Map(audioConfig);

            // Ensure there is an AudioListener and sources in the scene
            (musicSource, sfxSource) = _hardwareValidation.AttachAudioSources(Camera.main);

            // Play music for the current scene
            PlayMusicForCurrentScene();

            // Apply user settings
            UserSettings.ApplyToSources(musicSource, sfxSource);

            return Task.CompletedTask;
        }




        /// <summary>
        /// Play music assigned to the current scene.
        /// </summary>
        private void PlayMusicForCurrentScene()
        {
            var sceneName = SceneManager.GetActiveScene().name;

            // Check for exact scene match
            if (audioConfig.SceneMusicMap.TryGetValue(sceneName, out AudioClip musicClip))
            {
                if (musicSource.clip != musicClip)
                {
                    StartCoroutine(FadeTo(musicSource, musicClip, .5f));
                }
            }

            // No music assigned for this scene
            else
            {
                musicSource.Stop();
                musicSource.clip = null;

                Debug.LogWarning($"[AudioManager] No music assigned for scene: {sceneName}");
            }
        }

        /// <summary>
        ///   Play a sound effect by its label.
        /// </summary>
        public void PlaySound(string label)
        {
            if (audioConfig.SfxSoundsMap.TryGetValue(label, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] No sound effect found for label: {label}");
            }
        }


        /// <summary>
        ///  Fade in an audio source.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="fadeTime"></param>
        /// <returns></returns>
        private IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
        {
            StopCoroutine("FadeOut");
            isFading = true;
            audioSource.volume = 0;
            audioSource.Play();
            while (audioSource && audioSource.volume < 1)
            {
                audioSource.volume += Time.deltaTime / fadeTime;
                yield return null;
            }
            isFading = false;
        }


        /// <summary>
        ///  Fade out an audio source.
        /// </summary>
        private IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
        {
            if (audioSource.clip == null)
            {
                audioSource.Stop();
                isFading = false;
                yield break;
            }
            StopCoroutine("FadeIn");
            isFading = true;
            while (audioSource && audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }
            if (audioSource)
            {
                audioSource.Stop();
            }
            isFading = false;
        }


        /// <summary>
        ///  Fade to a new audio clip.
        /// </summary>
        private IEnumerator FadeTo(AudioSource audioSource, AudioClip clip, float fadeTime)
        {
            isFading = true;
            StartCoroutine(FadeOut(audioSource, fadeTime));
            yield return new WaitUntil(() => !isFading);
            audioSource.clip = clip;
            StartCoroutine(FadeIn(audioSource, fadeTime));
        }



    }
}