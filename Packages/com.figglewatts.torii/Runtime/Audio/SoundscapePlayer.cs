using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Torii.Injection;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace Torii.Audio
{
    [AutoInject]
    public class SoundscapePlayer : MonoBehaviour
    {
        public Soundscape CurrentSoundscape { get; private set; }
        public float TransitionDuration = 1f;
        public string MixerGroup = "SFX";

        public Transform Centre;

        private const string MIXER_PATH = "Mixers/MasterMixer";

        private readonly List<AudioSource> _baseSoundPlayers = new List<AudioSource>();
        private UnityEngine.Coroutine _playRandomSoundsCoroutine;
        private AudioMixerGroup _mixerGroup;

        public void Awake()
        {
            var mixer = Resources.Load<AudioMixer>(MIXER_PATH);
            _mixerGroup = mixer.FindMatchingGroups(MixerGroup).First();
        }

        public void PlaySoundscape(Soundscape soundscape)
        {
            if (CurrentSoundscape == soundscape) return;

            if (CurrentSoundscape != null)
            {
                destroyCurrentSoundscape();
            }

            beginSoundscape(soundscape);
        }

        public void StopCurrentSoundscape()
        {
            if (CurrentSoundscape == null) return;

            destroyCurrentSoundscape();
        }

        private void beginSoundscape(Soundscape soundscape)
        {
            CurrentSoundscape = soundscape;

            foreach (var baseSound in soundscape.BaseSounds)
            {
                var source = createBaseSoundPlayer(baseSound);
                _baseSoundPlayers.Add(source);
                StartCoroutine(fadeInClip(source));
            }

            if (soundscape.Sounds.Count > 0)
            {
                _playRandomSoundsCoroutine = StartCoroutine(playRandomSounds(soundscape));
            }

            if (soundscape.MixerSnapshot == null)
            {
                var defaultSnapshot = _mixerGroup.audioMixer.FindSnapshot("Default");
                _mixerGroup.audioMixer.TransitionToSnapshot(defaultSnapshot, TransitionDuration);
            }
            else
            {
                _mixerGroup.audioMixer.TransitionToSnapshot(soundscape.MixerSnapshot, TransitionDuration);
            }
        }

        private void destroyCurrentSoundscape()
        {
            foreach (var baseSound in _baseSoundPlayers)
            {
                StartCoroutine(fadeOutClipAndDestroy(baseSound));
            }

            _baseSoundPlayers.Clear();

            if (_playRandomSoundsCoroutine != null)
            {
                StopCoroutine(_playRandomSoundsCoroutine);
                _playRandomSoundsCoroutine = null;
            }
        }

        private IEnumerator fadeInClip(AudioSource source)
        {
            float currentTime = 0;
            float duration = TransitionDuration;
            float startVolume = 0;
            source.Play();

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 1, currentTime / duration);
                yield return null;
            }

            source.volume = 1;
        }

        private IEnumerator fadeOutClipAndDestroy(AudioSource source)
        {
            float currentTime = 0;
            float duration = TransitionDuration;
            float startVolume = source.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0, currentTime / duration);
                yield return null;
            }

            source.volume = 0;
            source.Stop();
            Destroy(source.gameObject);
        }

        private IEnumerator playRandomSounds(Soundscape soundscape)
        {
            while (true)
            {
                float nextInterval = RandUtil.Float(soundscape.MinSoundInterval, soundscape.MaxSoundInterval);
                yield return new WaitForSeconds(nextInterval);
                createRandomSound(soundscape);
            }
        }

        private AudioSource createSourceObject()
        {
            GameObject sourceObject = new GameObject("SoundscapeClip");
            sourceObject.transform.SetParent(transform);
            return sourceObject.AddComponent<AudioSource>();
        }

        private AudioSource createBaseSoundPlayer(AudioClip sound)
        {
            var source = createSourceObject();
            source.clip = sound;
            source.spatialBlend = 0; // base sounds are 2D
            source.loop = true;
            source.outputAudioMixerGroup = _mixerGroup;
            return source;
        }

        private void createRandomSound(Soundscape soundscape)
        {
            var randomSound = RandUtil.RandomListElement(soundscape.Sounds);
            var source = createSourceObject();
            source.clip = randomSound.Sound;
            source.spatialBlend = 1;
            source.loop = false;
            source.outputAudioMixerGroup = _mixerGroup;
            source.rolloffMode = AudioRolloffMode.Linear;

            var angle = RandUtil.Float(randomSound.MinAngle, randomSound.MaxAngle);
            var distance = RandUtil.Float(randomSound.MinDistance, randomSound.MaxDistance);

            Transform baseTransform = Centre == null ? transform : Centre.transform;

            Vector3 soundPos = baseTransform.position +
                               (Quaternion.AngleAxis(angle, Vector3.up) * baseTransform.forward * distance);
            source.transform.position = soundPos;
            StartCoroutine(playSoundThenDestroy(source));
        }

        private IEnumerator playSoundThenDestroy(AudioSource source)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
            Destroy(source.gameObject);
        }
    }
}
