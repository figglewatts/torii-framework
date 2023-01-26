using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Torii.Audio
{
    [CreateAssetMenu(menuName = "Game/Soundscape")]
    public class Soundscape : ScriptableObject
    {
        public List<AudioClip> BaseSounds = new List<AudioClip>();

        public Vector2 SoundIntervalRange = new Vector2(30, 60);
        public float MinSoundInterval => SoundIntervalRange.x;
        public float MaxSoundInterval => SoundIntervalRange.y;

        public List<SoundscapeSound> Sounds = new List<SoundscapeSound>();
        public AudioMixerSnapshot MixerSnapshot;
    }
}
