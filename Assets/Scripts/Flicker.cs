using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    [SerializeField] Light flicker;
    [SerializeField] float amplitude;
    [SerializeField] float frequency;

    float startIntensity = 0;
    void Start()
    {
        startIntensity = flicker.intensity;
    }

    void Update()
    {
        flicker.intensity = startIntensity + (Mathf.PerlinNoise(Time.time * frequency, Time.time * frequency) * amplitude);
    }
}
