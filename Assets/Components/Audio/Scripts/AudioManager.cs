using System.Collections;
using System.Threading.Tasks;
using Components.Events;
using Components.Scenes;
using UnityEngine;

namespace Components.Audio.Scripts
{
    public class AudioManager : Singleton.Model<AudioManager>, IAudioService, IInitializable
    {

        [Header("Components")]
        private AudioMapping _audioMapping = new();
        private HardwareValidation _hardwareValidation = new();
        public UserSettings UserSettings = new();
        

        [Header("Runtime Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;


        [Header("Initialisation")]
        public int InitPriority => 0;
        public System.Type[] InitDependencies => null;


        [Header("Configuration")]
        public SO_AudioConfig audioConfig;


        [Header("State")]
        private bool isFading = false;
        public float MaxVolume = .3f;


        
        // IAudioService implementation
        public bool IsMusicEnabled => UserSettings.MusicEnabled;
        public bool IsSfxEnabled => UserSettings.SfxEnabled;



        public async Task InitializeAsync()
        {
            // Setup configuration and hardware
            await SetupAudioSystem();

            // Subscribe to events
            SubscribeToEvents();

            // Load and apply user settings
            UserSettings.LoadSettings();
            UserSettings.ApplyToSources(musicSource, sfxSource);
        }

        private async Task SetupAudioSystem()
        {
            // Configuration setup
            audioConfig = _audioMapping.Map(audioConfig);

            // Hardware setup
            (musicSource, sfxSource) = _hardwareValidation.AttachAudioSources(UnityEngine.Camera.main);

            await Task.CompletedTask;
        }

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PlaySoundEvent>(OnPlaySoundEvent);
            EventBus.Subscribe<PlayMusicEvent>(OnPlayMusicEvent);
            EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
            EventBus.Subscribe<AudioSettingsChangedEvent>(OnAudioSettingsChanged);
        }




        // Event handlers
        private void OnPlaySoundEvent(PlaySoundEvent soundEvent)
        {
            PlaySound(soundEvent.soundName, soundEvent.volume);
        }

        private void OnPlayMusicEvent(PlayMusicEvent musicEvent)
        {
            PlayMusicForScene(musicEvent.sceneName);
        }

        private void OnSceneLoadedEvent(SceneLoadedEvent sceneEvent)
        {
            PlayMusicForScene(sceneEvent.SceneName);
        }

        private void OnAudioSettingsChanged(AudioSettingsChangedEvent settingsEvent)
        {
            UserSettings.ApplyToSources(musicSource, sfxSource);
        }


        /// <summary>
        /// Play music for a specific scene.
        /// </summary>
        public void PlayMusicForScene(string sceneName)
        {
            if (!IsMusicEnabled) return;

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
                StopMusic();
                Debug.LogWarning($"[AudioManager] No music assigned for scene: {sceneName}");
            }
        }

        /// <summary>
        ///   Play a sound effect by its label.
        /// </summary>
        public void PlaySound(string soundName, float volume = 1f)
        {
            if (!IsSfxEnabled) return;

            if (audioConfig.SfxSoundsMap.TryGetValue(soundName, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip, volume);
                sfxSource.volume = MaxVolume;
            }
            else
            {
                Debug.LogWarning($"[AudioManager] No sound effect found for: {soundName}");
            }
        }

        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
                musicSource.clip = null;
            }
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
            {
                musicSource.volume = IsMusicEnabled ? volume : 0f;
            }
        }

        public void SetSfxVolume(float volume)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = IsSfxEnabled ? volume : 0f;
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
            while (audioSource && audioSource.volume < MaxVolume)
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

        private void OnDestroy()
        {
            // Cleanup event subscriptions
            EventBus.Unsubscribe<PlaySoundEvent>(OnPlaySoundEvent);
            EventBus.Unsubscribe<PlayMusicEvent>(OnPlayMusicEvent);
            EventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
            EventBus.Unsubscribe<AudioSettingsChangedEvent>(OnAudioSettingsChanged);
        }
    }
}