using UnityEngine;

[System.Serializable]
public class CameraFOV 
{
    [SerializeField] private float fov;
    [SerializeField] private float fovSmoothTime;
    private float desiredFov = 20f;
    private float vel = 0f;

    public float FOVUpdate(BoxingController player)
    {
        float magnitude = player.Movement.Magnitude;
        float magToMaxRatio = player.Movement.VelToMaxRatio;

        float fovOffset = 0f;
        float fovCurve = magToMaxRatio * magToMaxRatio;

        if (magnitude > 5f) fovOffset = 10f * fovCurve;

        desiredFov = Mathf.SmoothDamp(desiredFov, fov + fovOffset, ref vel, fovSmoothTime);
        return desiredFov;
    }
}
