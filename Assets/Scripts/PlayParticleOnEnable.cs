using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnEnable : MonoBehaviour
{
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool addative = false;

    [Header("Light Emission Settings")]
    [SerializeField] private float intensity;
    [SerializeField] private float intensitySmoothing;
    private float desiredIntensity;

    [Header("Assignables")]
    [SerializeField] private new Light light;
    private ParticleSystem particle;
    private Coroutine lightUpdating;

    void Awake() => particle = GetComponent<ParticleSystem>();

    void OnEnable()
    {
        if (playOnEnable) PlayParticle();
    }

    void OnDisable()
    {
        if (light == null) return;

        light.intensity = 0;
        desiredIntensity = 0;

        lightUpdating = null;
    }

    public void PlayParticle()
    {
        if (particle != null)
        {
            particle.Play();
            if (GameManager.Instance.CurrentGameState == GameState.Paused) particle.Pause();
        }

        if (light != null)
        {
            desiredIntensity = addative ? desiredIntensity + intensity : intensity;
            if (lightUpdating == null) lightUpdating = StartCoroutine(UpdateLight());
        }
    }

    private IEnumerator UpdateLight()
    {
        while (isActiveAndEnabled)
        {
            if (desiredIntensity + light.intensity != 0)
            {
                desiredIntensity = Mathf.Lerp(desiredIntensity, 0, intensitySmoothing * 0.5f * Time.deltaTime);
                light.intensity = Mathf.Lerp(light.intensity, desiredIntensity, intensitySmoothing * Time.deltaTime);

                if (desiredIntensity < 0.01f)
                {
                    desiredIntensity = 0f;
                    light.intensity = 0f;
                }
            }

            yield return null;
        }
    }
}
