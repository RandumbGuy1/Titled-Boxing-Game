using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private int soundCapacity;
    [SerializeField, Range(0, 1)] private float volume;
    [SerializeField, Range(0, 1)] private float pitch;
    [SerializeField, Range(0, 1)] private float spatialBlend;
    [SerializeField] private bool looping;
    [SerializeField] private bool playOnAwake;

    public AudioClip Clip => clip;
    public bool PlayOnAwake => playOnAwake;
    public int SoundCapacity => soundCapacity;

    public float Volume => volume;

    public Queue<AudioSource> SourcesQueue { get; private set; } = new Queue<AudioSource>();

    public void SetParamaters(AudioSource source)
    {
        if (source == null || SourcesQueue.Contains(source)) return;

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = looping;
        source.spatialBlend = spatialBlend;
    }

    public void StopInstance(AudioSource source)
    {
        if (source == null || !SourcesQueue.Contains(source)) return;

        source.Stop();
    }

    public void StopAllAudio()
    {
        foreach (AudioSource source in SourcesQueue) source.Stop();
    }

    public void SetSourceQueue(Queue<AudioSource> queue)
    {
        SourcesQueue = queue;
    }
}
