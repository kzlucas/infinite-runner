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
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;


        [Header("Initialisation")]
        public int InitPriority => 0;
        public System.Type[] InitDependencies => null;


        [Header("Configuration")]
        public SO_AudioConfig AudioConfig;


        [Header("State")]
        private bool _isFading = false;
        public float MaxVolume = .5f;


        
        // IAudioService implementation
        public bool IsMusicEnabled => UserSettings.MusicEnabled;
        public bool IsSfxEnabled => UserSettings.SfxEnabled;


        public async Task InitializeAsync()
        {
            await SetupAudioSystem();
            PlayMusicForScene(SceneLoader.Instance.GetSceneName());
        }

        private async Task SetupAudioSystem()
        {
            // Configuration setup
            AudioConfig = _audioMapping.Map(AudioConfig);

            // Hardware setup
            (_musicSource, _sfxSource) = _hardwareValidation.AttachAudioSources(UnityEngine.Camera.main);

            // Load and apply user settings
            UserSettings.LoadSettings();
            UserSettings.ApplyToSources(_musicSource, _sfxSource, MaxVolume);

            await Task.CompletedTask;
        }

        private void Start()
        {
            EventBus.Subscribe<PlaySoundEvent>(OnPlaySoundEvent);
            EventBus.Subscribe<PlayMusicEvent>(OnPlayMusicEvent);
            EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
            EventBus.Subscribe<AudioSettingsChangedEvent>(OnAudioSettingsChanged);
        }

        private void OnDestroy()
        {
            // Cleanup event subscriptions
            EventBus.Unsubscribe<PlaySoundEvent>(OnPlaySoundEvent);
            EventBus.Unsubscribe<PlayMusicEvent>(OnPlayMusicEvent);
            EventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
            EventBus.Unsubscribe<AudioSettingsChangedEvent>(OnAudioSettingsChanged);
        }



        // Event handlers
        private async void OnPlaySoundEvent(PlaySoundEvent soundEvent)
        {
            await SetupAudioSystem();
            PlaySound(soundEvent.SoundName, soundEvent.Volume);
        }

        private async void OnPlayMusicEvent(PlayMusicEvent musicEvent)
        {
            await SetupAudioSystem();
            PlayMusicForScene(SceneLoader.Instance.GetSceneName());
        }

        private async void OnSceneLoadedEvent(SceneLoadedEvent sceneEvent)
        {
            await SetupAudioSystem();
            PlayMusicForScene(sceneEvent.SceneName);
        }

        private void OnAudioSettingsChanged(AudioSettingsChangedEvent settingsEvent)
        {
            UserSettings.ApplyToSources(_musicSource, _sfxSource, MaxVolume);

            if(settingsEvent.MusicEnabled)
                PlayMusicForScene(SceneLoader.Instance.GetSceneName());
            else
                StopMusic();
        }


        /// <summary>
        /// Play music for a specific scene.
        /// </summary>
        public async void PlayMusicForScene(string sceneName)
        {
            if (!IsMusicEnabled) return;

            // Check for exact scene match
            if (AudioConfig.SceneMusicMap.TryGetValue(sceneName, out AudioClip musicClip))
            {
                StartCoroutine(FadeToMusic(_musicSource, musicClip, .5f));
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
        public void PlaySound(string soundName, float volume = -1f)
        {
            if(volume == -1f) volume = MaxVolume;
            if (!IsSfxEnabled) return;

            if (AudioConfig.SfxSoundsMap.TryGetValue(soundName, out AudioClip clip))
            {
                _sfxSource.PlayOneShot(clip, volume);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] No sound effect found for: {soundName}");
            }
        }

        public void StopMusic()
        {
            if (_musicSource != null)
            {
                _musicSource.Stop();
                _musicSource.clip = null;
            }
        }


        /// <summary>
        ///  Fade in an audio source.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="fadeTime"></param>
        /// <returns></returns>
        private IEnumerator FadeInMusic(AudioSource audioSource, float fadeTime)
        {
            StopCoroutine("FadeOut");
            _isFading = true;
            audioSource.volume = 0;
            audioSource.Play();
            while (audioSource && audioSource.volume < MaxVolume)
            {
                audioSource.volume += Time.unscaledDeltaTime / fadeTime;
                yield return null;
            }
            audioSource.volume = MaxVolume;
            _isFading = false;
        }


        /// <summary>
        ///  Fade out an audio source.
        /// </summary>
        private IEnumerator FadeOutMusic(AudioSource audioSource, float fadeTime)
        {
            if (audioSource.clip == null)
            {
                audioSource.Stop();
                _isFading = false;
                yield break;
            }
            StopCoroutine("FadeIn");
            _isFading = true;
            while (audioSource && audioSource.volume > 0)
            {
                audioSource.volume -= Time.unscaledDeltaTime / fadeTime;
                yield return null;
            }
            if (audioSource)
            {
                audioSource.Stop();
            }
            audioSource.volume = 0; 
            _isFading = false;
        }


        /// <summary>
        ///  Fade to a new audio clip.
        /// </summary>
        private IEnumerator FadeToMusic(AudioSource audioSource, AudioClip clip, float fadeTime)
        {
            _isFading = true;
            StartCoroutine(FadeOutMusic(audioSource, fadeTime));
            yield return new WaitUntil(() => !_isFading);
            if(!this) yield break;
            audioSource.clip = clip;
            StartCoroutine(FadeInMusic(audioSource, fadeTime));
            audioSource.volume = MaxVolume;
        }

    }
}