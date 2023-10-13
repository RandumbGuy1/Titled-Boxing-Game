using UnityEngine;

public class KickbackShake : IShakeEvent
{
    private readonly ShakeData shakeData;
    public ShakeData ShakeData { get { return shakeData; } }

    public Vector3 ShakeOffset { get; private set; }
    public bool Finished { get; }

    public CameraShaker Receiever { get; private set; }
    private int index = -1;

    private float timeRemaining;
    private float trama = 0f;

    private Vector3 desiredDir = Vector3.zero;
    private Vector3 targetDir = Vector3.zero;
    private Vector3 smoothDir = Vector3.zero;

    public KickbackShake(ShakeData shakeData, Vector3 initialKickback)
    {
        this.shakeData = ScriptableObject.CreateInstance<ShakeData>();
        this.shakeData.Initialize(shakeData);

        timeRemaining = this.shakeData.Duration;
        desiredDir = initialKickback * this.shakeData.Magnitude;

        KickBack();
    }

    public KickbackShake(ShakeData shakeData)
    {
        this.shakeData = ScriptableObject.CreateInstance<ShakeData>();
        this.shakeData.Initialize(shakeData);

        timeRemaining = this.shakeData.Duration;
        desiredDir = Random.insideUnitCircle * this.shakeData.Magnitude;

        KickBack();
    }

    public void UpdateShake(float deltaTime)
    {
        timeRemaining -= deltaTime;

        float agePercent = 1f - (timeRemaining / shakeData.Duration);
        trama = shakeData.BlendOverLifetime.Evaluate(agePercent);
        trama = Mathf.Clamp(trama, 0f, 1f);

        desiredDir = Vector3.Lerp(desiredDir, targetDir, shakeData.SmoothSpeed * 0.4f * Time.smoothDeltaTime);
        smoothDir = Vector3.Slerp(smoothDir, desiredDir, shakeData.SmoothSpeed * Time.smoothDeltaTime);

        ShakeOffset = smoothDir * trama;

        if (shakeData.Frequency <= 0) return;

        if ((targetDir - desiredDir).sqrMagnitude < (shakeData.Magnitude * shakeData.Frequency * 0.05f) * (shakeData.Magnitude * shakeData.Frequency * 0.05f))
        {
            Vector3 randomDir = Random.insideUnitSphere;
            while (!InBounds(Vector3.Dot(-targetDir, randomDir), 0.5f, -0.5f)) randomDir = Random.insideUnitSphere;

            targetDir = (randomDir * 2.8f - targetDir).normalized * shakeData.Magnitude;
        }
    }

    private void KickBack()
    {
        if (shakeData.Frequency > 0)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            while (!InBounds(Vector3.Dot(-targetDir, randomDir), 0.5f, -0.5f)) randomDir = Random.insideUnitSphere;
            targetDir = (randomDir * 2f - desiredDir * 0.7f).normalized * shakeData.Magnitude;
        }
        else targetDir = Vector3.zero;
    }

    bool InBounds(float amount, float max, float min) => amount >= min && amount <= max;

    public void RemoveShake()
    {
        Receiever.RemoveShakeAtIndex(index);
        SetIndexAndReceiever(null, -1);
    }

    public void SetIndexAndReceiever(CameraShaker shaker, int i)
    {
        Receiever = shaker;
        index = i;
    }
}
