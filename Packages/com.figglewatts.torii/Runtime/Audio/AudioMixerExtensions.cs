using UnityEngine.Audio;

namespace Torii.Audio
{
    public static class AudioMixerExtensions
    {
        public static void TransitionToSnapshot(this AudioMixer audioMixer,
            AudioMixerSnapshot snapshot,
            float timeToReach = 1f)
        {
            audioMixer.TransitionToSnapshots(new []{ snapshot }, new[] { 1f }, timeToReach);
        }
    }
}
