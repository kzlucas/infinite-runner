using System.Collections.Generic;
using UnityEngine;


namespace Components.Audio.Scripts
{

    [CreateAssetMenu(fileName = "Create Audio Config", menuName = "Audio Config/New Audio Config", order = 1)]
    public class SO_AudioConfig : ScriptableObject
    {

        [Header("Background Music")]

        [SerializeField] private List<SceneMusicPair> _sceneMusicPairs = new List<SceneMusicPair>();
        public List<SceneMusicPair> SceneMusicPairs => _sceneMusicPairs;

        private Dictionary<string, AudioClip> _sceneMusicMap = new Dictionary<string, AudioClip>();
        public Dictionary<string, AudioClip> SceneMusicMap => _sceneMusicMap;



        [Header("Sound Effects")]

        [SerializeField] private List<SceneMusicPair> _sfxSoundsPairs = new List<SceneMusicPair>();
        public List<SceneMusicPair> SfxSoundsPairs => _sfxSoundsPairs;

        private Dictionary<string, AudioClip> _sfxSoundsMap = new Dictionary<string, AudioClip>();
        public Dictionary<string, AudioClip> SfxSoundsMap => _sfxSoundsMap;

    }
}