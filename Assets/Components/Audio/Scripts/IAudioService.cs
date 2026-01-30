namespace Components.Audio.Scripts
{
    public interface IAudioService
    {
        void PlaySound(string soundName, float volume = 1f);
        void PlayMusicForScene(string sceneName);
        void StopMusic();
        void SetMusicVolume(float volume);
        void SetSfxVolume(float volume);
        bool IsMusicEnabled { get; }
        bool IsSfxEnabled { get; }
    }
}