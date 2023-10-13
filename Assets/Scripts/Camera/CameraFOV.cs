using UnityEngine;

[System.Serializable]
public class CameraFOV 
{
    [SerializeField] private float fov;
    [SerializeField] private float fovSmoothTime;
    private float desiredFov = 20f;
    private float vel = 0f;

    public float FOVUpdate(PlayerRef player)
    {
        float fovOffset = 0f;
        float fovCurve = player.PlayerMovement.MagToMaxRatio * player.PlayerMovement.MagToMaxRatio;

        if (player.PlayerMovement.Magnitude > 5f) fovOffset = 10f * fovCurve;

        desiredFov = Mathf.SmoothDamp(desiredFov, fov + fovOffset, ref vel, fovSmoothTime);
        return desiredFov;
    }
}
