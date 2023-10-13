using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shake Data", menuName = "Custom Shake Event Data", order = 1)]
public class ShakeData : ScriptableObject
{
    [Header("Shake Settings")]
    [SerializeField] private float magnitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float smoothSpeed = 1f;
    [Space(10)]
    [SerializeField] private AnimationCurve blendOverLifetime = new AnimationCurve(
      new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * 720.0f),
      new Keyframe(0.2f, 1.0f),
      new Keyframe(1.0f, 0.0f));

    public float Magnitude { get { return magnitude; } set { magnitude = value; } }
    public float Frequency { get { return frequency; } set { frequency = value; } }
    public float Duration { get { return duration; } set { duration = value; } }
    public float SmoothSpeed { get { return smoothSpeed; } set { smoothSpeed = value; } }
    public AnimationCurve BlendOverLifetime { get { return blendOverLifetime; } }

    public static ShakeData Create(float magnitude, float frequency, float duration, float smoothness)
    {
        ShakeData newData = CreateInstance<ShakeData>();
        newData.Initialize(magnitude, frequency, duration, smoothness);

        return newData;
    }

    public void Initialize(float magnitude, float frequency, float duration, float smoothness)
    {
        this.magnitude = magnitude;
        this.frequency = frequency;
        this.duration = duration;
        this.smoothSpeed = smoothness;
    }

    public void Initialize(ShakeData sd)
    {
        magnitude = sd.Magnitude;
        frequency = sd.Frequency;
        duration = sd.Duration;
        smoothSpeed = sd.SmoothSpeed;
    }
}
