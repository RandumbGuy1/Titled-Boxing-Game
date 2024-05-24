using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject soundInstancePrefab;
    [SerializeField] private Sound[] sounds;

    public Dictionary<AudioClip, Sound> SoundDictionary { get; private set; } = new Dictionary<AudioClip, Sound>();
    public static AudioManager Instance;

    void Awake()
    {
        Instance = this;

        //Store all sounds in a dictionary
        foreach (Sound sound in sounds)
        {
            Queue<AudioSource> soundInstances = new Queue<AudioSource>();
            for (int i = 0; i < sound.SoundCapacity; i++)
            {
                AudioSource instance = Instantiate(soundInstancePrefab).GetComponent<AudioSource>();
                soundInstances.Enqueue(instance);
            }

            SoundDictionary.Add(sound.Clip, sound);
            sound.SetSourceQueue(soundInstances);
            
            //Play all sounds set to playOnAwake
            if (!sound.PlayOnAwake) continue;
            PlayOnce(sound.Clip, Vector3.zero);
        }
    }

    public AudioSource PlayOnce(AudioClip clip, Vector3 sourcePos, float volumeMultiplier = 1f)
    {
        if (clip == null) return null;
        if (!SoundDictionary.ContainsKey(clip)) return null;

        //Spawn audio instance at the sounds source position
        AudioSource source = SoundDictionary[clip].SourcesQueue.Dequeue();
        if (source == null) return null;

        //Set source position
        source.transform.position = sourcePos;

        //Set the data of the audio source using the sound class preset
        SoundDictionary[clip].SetParamaters(source);
        source.volume *= volumeMultiplier;

        source.Play();

        //Send sound back to the sounds pool
        SoundDictionary[clip].SourcesQueue.Enqueue(source);

        return source;
    }

    public void PlayOnce(AudioClip clip)
    {
        if (clip == null) return;
        if (!SoundDictionary.ContainsKey(clip)) return;

        //Spawn audio instance at the sounds source position
        AudioSource source = SoundDictionary[clip].SourcesQueue.Dequeue();
        if (source == null) return;

        //Set source position
        source.transform.position = Vector3.zero;

        //Set the data of the audio source using the sound class preset
        SoundDictionary[clip].SetParamaters(source);

        source.Play();

        //Send sound back to the sounds pool
        SoundDictionary[clip].SourcesQueue.Enqueue(source);
    }

    public AudioSource PlayOnce(AudioClip[] clips, Vector3 sourcePos, float volumeMultiplier = 1f)
    {
        if (clips.Length == 0) return null;

        int newSoundIndex = Mathf.RoundToInt(Random.Range(0, clips.Length));
        AudioClip clip = clips[newSoundIndex];

        if (!SoundDictionary.ContainsKey(clip)) return null;

        AudioSource source = SoundDictionary[clip].SourcesQueue.Dequeue();
        if (source == null) return null;

        source.transform.position = sourcePos;

        SoundDictionary[clip].SetParamaters(source);
        source.volume *= volumeMultiplier;

        source.Play();

        SoundDictionary[clip].SourcesQueue.Enqueue(source);

        return source;
    }

    //Stop a certain audio source
    public void StopSound(AudioClip clip, AudioSource source)
    {
        if (clip == null) return;
        if (!SoundDictionary.ContainsKey(clip)) return;
        if (source == null) return;

        SoundDictionary[clip].StopInstance(source);
    }

    //Stop all audio of a certain sound
    public void StopAllSounds(AudioClip clip)
    {
        if (!SoundDictionary.ContainsKey(clip)) return;

        SoundDictionary[clip].StopAllAudio();
    }
}
