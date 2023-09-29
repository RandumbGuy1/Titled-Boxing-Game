using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnEnable : MonoBehaviour
{
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private ParticleSystem particle;

    void OnEnable()
    {
        if (playOnEnable && particle != null) particle.Play();
    }
}
