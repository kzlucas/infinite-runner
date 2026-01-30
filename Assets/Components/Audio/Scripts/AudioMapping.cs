
namespace Components.Audio.Scripts
{
    public class AudioMapping
    {

        public SO_AudioConfig  Map(SO_AudioConfig audioConfig)
        {
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

            return audioConfig;
        }
    }
}