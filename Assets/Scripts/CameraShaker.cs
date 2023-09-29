using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [Header("Active Shakes")]
    [SerializeField] private List<IShakeEvent> shakeEvents = new List<IShakeEvent>();

    public Vector3 Offset { get; private set; }
    private bool canAddShakes = true;
    private bool processShakes = true;

    public void ShakeOnce(IShakeEvent shakeEvent)
    {
        if (!canAddShakes) return;

        shakeEvent.SetIndexAndReceiever(this, shakeEvents.Count);
        shakeEvents.Add(shakeEvent);
    }

    public void RemoveShakeAtIndex(int i)
    {
        if (i < 0 || i > shakeEvents.Count - 1) return;
        shakeEvents.RemoveAt(i);
    }

    void LateUpdate()
    {
        if (shakeEvents.Count <= 0)
        {
            Offset = Vector3.zero;
            return;
        }

        Vector3 rotationOffset = Vector3.zero;

        for (int i = shakeEvents.Count - 1; i != -1; i--)
        {
            IShakeEvent shake = shakeEvents[i];

            if (shake.Finished)
            {
                shake.RemoveShake();
                continue;
            }

            shake.SetIndexAndReceiever(this, i);
            if (processShakes) shake.UpdateShake(Time.deltaTime);
            rotationOffset += shake.ShakeOffset;
        }

        Offset = rotationOffset;
    }

    public void DisableAddShakes(bool b = false) => canAddShakes = b;
    public void StopRunningShakes(bool b = false) => processShakes = b;
}
