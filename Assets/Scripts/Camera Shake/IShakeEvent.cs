using UnityEngine;

public interface IShakeEvent
{
    CameraShaker Receiever { get; }
    ShakeData ShakeData { get; }

    Vector3 ShakeOffset { get; }
    bool Finished { get; }

    void UpdateShake(float deltaTime);
    void SetIndexAndReceiever(CameraShaker shaker, int i);
    void RemoveShake();
}
