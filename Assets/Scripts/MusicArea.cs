using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicArea : MonoBehaviour
{
    [SerializeField] float volume;
    [SerializeField] AudioClip clip;
    [SerializeField] BoxCollider region;
    [SerializeField] LayerMask hitLayer;

    readonly List<Player> trackedPlayers = new List<Player>();
    readonly List<AudioSource> trackedSources = new List<AudioSource>();

    void Update()
    {
        for (int i = 0; i < trackedSources.Count; i++)
        {
            trackedSources[i].volume = volume * EvaluateSubmergence(trackedPlayers[i].CapsuleCol);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        if (hitLayer != (hitLayer | 1 << layer)) return;

        Player player = other.GetComponent<Player>();
        if (!player) return;

        AudioSource source = other.GetComponent<AudioSource>();
        if (!source) return;

        trackedPlayers.Add(player);
        trackedSources.Add(source);
        source.clip = clip;
        source.Play();
    }

    void OnTriggerExit(Collider other)
    {
        int layer = other.gameObject.layer;
        if (hitLayer != (hitLayer | 1 << layer)) return;

        Player player = other.GetComponent<Player>();
        if (!player) return;

        AudioSource source = other.GetComponent<AudioSource>();
        if (!source) return;

        trackedPlayers.Remove(player);
        trackedSources.Remove(source);
        source.Stop();
    }

    private float EvaluateSubmergence(Collider submergee)
    {
        float total = 1f;
        Bounds obj = submergee.bounds;
        Bounds region = this.region.bounds;

        for (int i = 0, dimensions = 3; i < dimensions; i++)
        {
            float dist = obj.min[i] > region.center[i] ?
                obj.max[i] - region.max[i] :
                region.min[i] - obj.min[i];

            total *= Mathf.Clamp01(1f - dist / obj.size[i]);
        }

        return total;
    }
}
