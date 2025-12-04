using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>, IInitializable
{


    [Header("Background Music")]
    [SerializeField] private List<SceneMusicPair> sceneMusicPairs = new List<SceneMusicPair>();
    private Dictionary<string, AudioClip> sceneMusicMap = new Dictionary<string, AudioClip>();


    [Header("Sound Effects")]
    [SerializeField] private List<SceneMusicPair> sfxSoundsPairs = new List<SceneMusicPair>();
    private Dictionary<string, AudioClip> sfxSoundsMap = new Dictionary<string, AudioClip>();

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;


    // Fading control
    private bool isFading = false;


    // IInitializable implementation
    public int Priority => 0;
    public System.Type[] Dependencies => null;
    public async System.Threading.Tasks.Task InitializeAsync()
    {
        Debug.Log("[AudioManager] init start...");

        // Initialize audio mapping
        foreach (var pair in sceneMusicPairs)
        {
            if (!string.IsNullOrEmpty(pair.sceneName) && pair.musicClip != null)
            {
                sceneMusicMap[pair.sceneName] = pair.musicClip;
            }
        }
        foreach (var pair in sfxSoundsPairs)
        {
            if (!string.IsNullOrEmpty(pair.sceneName) && pair.musicClip != null)
            {
                sfxSoundsMap[pair.sceneName] = pair.musicClip;
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

        var sceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(sceneName);

        // // Add a small delay to keep the async pattern
        // await System.Threading.Tasks.Task.Delay(1);

        Debug.Log("[AudioManager] init finished!");
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
        if (sceneMusicMap.TryGetValue(sceneName, out AudioClip musicClip))
        {
            if (musicSource.clip != musicClip)
            {
                StartCoroutine(FadeTo(musicSource, musicClip, .5f));
            }
        }

        // If scene name starts with "Level", play level music
        else if (sceneName.StartsWith("Level"))
        {
            if (sceneMusicMap.TryGetValue("Level", out AudioClip levelMusicClip))
            {
                StartCoroutine(FadeTo(musicSource, levelMusicClip, .5f));
            }
        }

        // No music assigned for this scene
        else
        {
            musicSource.Stop();
            musicSource.clip = null;

            Debug.LogError($"[AudioManager] No music assigned for scene: {sceneName}");
        }
    }

    public void PlaySound(string label)
    {
        if (sfxSoundsMap.TryGetValue(label, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
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