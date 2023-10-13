using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class PostProcessController : MonoBehaviour
{
    [SerializeField] private Volume volume;

    private DepthOfField depthProfile;
    public ProfileValue FocusDistance { get; private set; }

    private Vignette vignetteProfile;
    public ProfileValue VignetteIntensity { get; private set; }

    private MotionBlur motionProfile;
    public ProfileValue BlurIntensity { get; private set; }

    void Awake()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out depthProfile);
        volume.profile.TryGet(out vignetteProfile);
        volume.profile.TryGet(out motionProfile);

        BlurIntensity = new ProfileValue(motionProfile.intensity.value);
        FocusDistance = new ProfileValue(depthProfile.focusDistance.value);
        VignetteIntensity = new ProfileValue(vignetteProfile.intensity.value);
    }

    void Update()
    {
        BlurIntensity.UpdateValues(value => motionProfile.intensity.value = value);
        FocusDistance.UpdateValues(value => depthProfile.focusDistance.value = value);
        VignetteIntensity.UpdateValues(value => vignetteProfile.intensity.value = value);
    }
}

public class ProfileValue
{
    private float value;
    private float time;
    private float desired;
    private float vel;

    public ProfileValue(float desired)
    {
        value = desired;
        vel = 0f;
        time = 0.1f;

        this.desired = desired;
    }

    public void UpdateValues(Action<float> SetValue)
    {
        value = Mathf.SmoothDamp(value, desired, ref vel, time);
        SetValue(value);
    }

    public void SetValue(float desired, float time, bool snap = false)
    {
        if (snap) value = desired;
        else this.desired = desired;
        this.time = time;
    }
}
