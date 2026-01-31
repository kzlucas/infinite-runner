
namespace Components.Audio.Scripts
{
    public class AudioMapping
    {

        public SO_AudioConfig  Map(SO_AudioConfig audioConfig)
        {
            foreach (var pair in audioConfig.SceneMusicPairs)
            {
                if (!string.IsNullOrEmpty(pair.SceneName) && pair.MusicClip != null)
                {
                    audioConfig.SceneMusicMap[pair.SceneName] = pair.MusicClip;
                }
            }
            foreach (var pair in audioConfig.SfxSoundsPairs)
            {
                if (!string.IsNullOrEmpty(pair.SceneName) && pair.MusicClip != null)
                {
                    audioConfig.SfxSoundsMap[pair.SceneName] = pair.MusicClip;
                }
            }

            return audioConfig;
        }
    }
}