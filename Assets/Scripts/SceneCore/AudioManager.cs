using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>, IInitializable
{

    [Header("Config")]
    public SO_AudioConfig audioConfig;


    [Header("Settings")]
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
                musicSource.mute = true;
                musicSource.Pause();
            }
            else
            {
                musicSource.mute = false;
                musicSource.UnPause();
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
                sfxSource.mute = true;
                sfxSource.Pause();
            }
            else
            {
                sfxSource.mute = false;
                sfxSource.UnPause();
            }
        }
    }


    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;


    // Fading control
    private bool isFading = false;


    // IInitializable implementation
    public int initPriority => 0;
    public System.Type[] initDependencies => null;
    public Task InitializeAsync()
    {


        // Initialize audio mapping
        foreach (var pair in audioConfig.SceneMusicPairs)
        {
            if (!string.IsNullOrEmpty(pair.sceneName) && pair.musicClip != null)
            {
                audioConfig.SceneMusicMap[pair.sceneName] = pair.musicClip;
            }
        }
        foreach (var pair in audioConfig.SfxSoundsPairs)
        {
            if (!string.IsNullOrEmpty(pair.sceneName) && pair.musicClip != null)
            {
                audioConfig.SfxSoundsMap[pair.sceneName] = pair.musicClip;
            }
        }

        // Ensure there is an AudioListener and sources in the scene
        var cam = Camera.main;
        var audioSources = EnsureCamHasTwoAudioSources(cam);

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

        // Play music for the current scene
        var sceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(sceneName);

        // Apply user settings
        ApplyUserSettings();

        return Task.CompletedTask;
    }



    /// <summary>
    ///   Apply user audio settings from saved data.
    /// </summary>
    private void ApplyUserSettings()
    {
        MusicOn = PlayerPrefService.Load("music") != "False";
        SfxOn = PlayerPrefService.Load("sfx") != "False";
    }


    private AudioSource[] EnsureCamHasTwoAudioSources(Camera cam)
    {
        if (cam == null)
        {
            Debug.LogError("[AudioManager] No main camera found in the scene!");
            return null;
        }

        var audioSources = cam.GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            // Add missing AudioSources
            for (int i = audioSources.Length; i < 2; i++)
            {
                cam.gameObject.AddComponent<AudioSource>();
            }
        }
        return cam.GetComponents<AudioSource>();
    }


    private void PlayMusicForScene(string sceneName)
    {
        // Check for exact scene match
        if (audioConfig.SceneMusicMap.TryGetValue(sceneName, out AudioClip musicClip))
        {
            if (musicSource.clip != musicClip)
            {
                StartCoroutine(FadeTo(musicSource, musicClip, .5f));
            }
        }

        // If scene name starts with "Level", play level music
        else if (sceneName.StartsWith("Level"))
        {
            if (audioConfig.SceneMusicMap.TryGetValue("Level", out AudioClip levelMusicClip))
            {
                StartCoroutine(FadeTo(musicSource, levelMusicClip, .5f));
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

    private IEnumerator FadeTo(AudioSource audioSource, AudioClip clip, float fadeTime)
    {
        isFading = true;
        StartCoroutine(FadeOut(audioSource, fadeTime));
        yield return new WaitUntil(() => !isFading);
        audioSource.clip = clip;
        StartCoroutine(FadeIn(audioSource, fadeTime));
    }



}

[System.Serializable]
public class SceneMusicPair
{
    public string sceneName;
    public AudioClip musicClip;
}