using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer : MonoSingleton<AudioPlayer>
{
    public int MaxChannels = 10;
    public int _channelsAvailable => _channels.Count(c => !c.isPlaying);
    public AudioSource FreeChannel => _channels.First(c => !c.isPlaying);

    private List<AudioSource> _channels;

    private const string MIXER_PATH = "Mixers/MasterMixer";

    public override void Init()
    {
        _channels = new List<AudioSource>();
        addChannel();
    }

    private void setMixerGroup(AudioSource channel, string mixerGroup)
    {
        AudioMixer mixer = Resources.Load<AudioMixer>(MIXER_PATH);
        AudioMixerGroup group = mixer.FindMatchingGroups(mixerGroup).First();
        channel.outputAudioMixerGroup = group;
    }

    public AudioSource PlayClip(AudioClip clip, bool loop = false, string mixerGroup = null)
    {
        var channel = _channelsAvailable == 0 ? addChannel() : FreeChannel;
        if (channel == null)
        {
            Debug.LogWarning($"Unable to play audio clip '{clip.name}', no channels available");
            return null;
        }

        if (!string.IsNullOrEmpty(mixerGroup))
        {
            setMixerGroup(channel, mixerGroup);
        }

        channel.loop = loop;
        channel.clip = clip;
        channel.Play();

        return channel;
    }

    public void PlayClip3D(AudioClip clip, Vector3 position, string mixerGroup = null)
    {
        var source = createSourceObject(position);
        source.clip = clip;
        source.spatialBlend = 1;
        source.loop = false;
        source.rolloffMode = AudioRolloffMode.Linear;

        if (!string.IsNullOrEmpty(mixerGroup))
        {
            setMixerGroup(source, mixerGroup);
        }

        StartCoroutine(playSoundThenDestroy(source));
    }

    private AudioSource addChannel()
    {
        if (_channels.Count + 1 > MaxChannels)
        {
            return null;
        }

        AudioSource channel = gameObject.AddComponent<AudioSource>();
        _channels.Add(channel);
        return channel;
    }

    private AudioSource createSourceObject(Vector3 position)
    {
        GameObject sourceObject = new GameObject("AudioSource");
        sourceObject.transform.position = position;
        sourceObject.transform.SetParent(transform);
        return sourceObject.AddComponent<AudioSource>();
    }

    private IEnumerator playSoundThenDestroy(AudioSource source)
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
        Destroy(source.gameObject);
    }
}
