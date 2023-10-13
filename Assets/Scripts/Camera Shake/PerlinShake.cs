using UnityEngine;

public class PerlinShake : IShakeEvent
{
    private readonly ShakeData shakeData;
    public ShakeData ShakeData { get { return shakeData; } }

    public Vector3 ShakeOffset { get; private set; }
    public bool Finished { get; }

    public CameraShaker Receiever { get; private set; }
    private int index = -1;

    private float timeRemaining;
    private float trama = 0f;

    private Vector3 noise = Vector3.zero;
    private Vector3 noiseOffset = Vector3.zero;

    public PerlinShake(ShakeData shakeData)
    {
        this.shakeData = ScriptableObject.CreateInstance<ShakeData>();
        this.shakeData.Initialize(shakeData);

        timeRemaining = this.shakeData.Duration;

        noiseOffset.x = Random.Range(0f, 32f);
        noiseOffset.y = Random.Range(0f, 32f);
        noiseOffset.z = Random.Range(0f, 32f);
    }

    public void UpdateShake(float deltaTime)
    {
        float offsetDelta = deltaTime * shakeData.Frequency;
        timeRemaining -= deltaTime;

        noiseOffset += Vector3.one * offsetDelta;

        noise.x = OctaveNoise(noiseOffset.x, 1f);
        noise.y = OctaveNoise(noiseOffset.y, 2f);
        noise.z = OctaveNoise(noiseOffset.z, 3f);

        //noise = (noise - Vector3.one * 0.5f) * 2f;
        noise *= shakeData.Magnitude;
        noise *= trama;

        ShakeOffset = Vector3.Lerp(ShakeOffset, noise, shakeData.SmoothSpeed * deltaTime * 15f);

        float agePercent = 1f - (timeRemaining / shakeData.Duration);
        trama = shakeData.BlendOverLifetime.Evaluate(agePercent);
        trama = Mathf.Clamp01(trama);
    }

    public void RemoveShake() => Receiever.RemoveShakeAtIndex(index);

    public void SetIndexAndReceiever(CameraShaker shaker, int i)
    {
        Receiever = shaker;
        index = i;
    }

    private float OctaveNoise(float x, float y)
    {
        float result = 0f;

        float[] octaveFrequencies = { 1, 1.5f, 2f };
        float[] octaveAmplitudes = { 0.6f, 0.25f, 0.15f };

        for (int i = 0; i < octaveFrequencies.Length; i++)
            result += octaveAmplitudes[i] * Mathf.PerlinNoise(octaveFrequencies[i] * x, octaveFrequencies[i] * y);

        return (Mathf.Clamp01(result) - 0.5f) * 2f;
    }
}
