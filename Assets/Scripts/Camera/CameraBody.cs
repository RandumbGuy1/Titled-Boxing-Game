using UnityEngine;
using UnityEngine.Rendering;
using System;

public class CameraBody : MonoBehaviour
{
    [SerializeField] private CameraFOV camFov;
    [SerializeField] private CameraIdleSway camIdleSway;
    [SerializeField] private CameraHeadBob camHeadBob;
    [SerializeField] private TPSCameraCollider camCollider;
    [SerializeField] private CameraLook camLookSettings;
    [SerializeField] private CameraSprintEffect camSprintEffect;

    [Header("Basic Settings")]
    [SerializeField] private Vector3 posOffset;
    private Vector3 smoothPosOffset = Vector3.zero;

    public CameraIdleSway CamIdleSway => camIdleSway;
    public CameraHeadBob CamHeadBob => camHeadBob;
    public TPSCameraCollider CamCollider => camCollider;
    public CameraLook CamLookSettings => camLookSettings;
    public CameraShaker CamShaker => camShaker;

    public Vector3 TPSOffset => Vector3.back * camCollider.SmoothPull + smoothPosOffset;
    public bool InThirdPerson => CamCollider.Enabled;

    [Header("Refrences")]
    [SerializeField] private Transform targetHead;
    [SerializeField] private CameraShaker camShaker;
    [SerializeField] private PlayerRef player;
    [SerializeField] private LockOn lockOn;

    public bool CanMoveCamera { get; private set; } = true;

    void Awake()
    {
        SetCursorState(true);

        player.PlayerInput.OnMouseInput += camLookSettings.LookUpdate;
        player.PlayerMovement.OnPlayerLand += camHeadBob.BobOnce;
    }

    void Update()
    {
        if (player == null) return;

        player.PlayerCam.fieldOfView = camFov.FOVUpdate(player.PlayerMovement.Magnitude, player.PlayerMovement.MagToMaxRatio);
        camSprintEffect.SpeedLines(player);
        camIdleSway.IdleCameraSway(player);
        camHeadBob.BobUpdate(player);
        camCollider.ColliderUpdate(player.PlayerCam.transform.position, player.transform.position);
    }

    float angle = 0f;
    float GetLockOnAngleDelta()
    {
        if (lockOn.LockOnTarget == null) return angle;

        Vector3 camToLockOn = (lockOn.LockOnTarget.position - player.Orientation.position).normalized;
        angle = Mathf.Rad2Deg * Mathf.Atan2(camToLockOn.x, camToLockOn.z) - camLookSettings.SmoothRotation.y;

        return angle;
    }

    void LateUpdate()
    {
        //Apply Rotations And Positions
        {
            transform.rotation = Quaternion.Euler(camLookSettings.SmoothRotation.x, GetLockOnAngleDelta() + camLookSettings.SmoothRotation.y, 0);
            player.Orientation.rotation = Quaternion.Euler(0, GetLockOnAngleDelta() + camLookSettings.SmoothRotation.y, 0);

            //Camera effects rotation
            player.PlayerCam.transform.localRotation = Quaternion.Euler(ToEuler(camHeadBob.ViewBobOffset) + Vector3.forward * camHeadBob.TiltSway + camShaker.Offset + camIdleSway.HeadSwayOffset);

            //Camera positions
            Vector3 cameraTPSOffset = camCollider.Enabled ? posOffset + CamHeadBob.ViewBobOffset * 0.2f : Vector3.zero;
            smoothPosOffset = Vector3.Lerp(smoothPosOffset, cameraTPSOffset, 6f * Time.deltaTime);

            player.PlayerCam.transform.localPosition = Vector3.back * camCollider.SmoothPull + smoothPosOffset;
            transform.position = targetHead.position + player.PlayerMovement.CrouchOffset;
        }
    }

    public static Vector3 ToEuler(Vector3 a)
    {
        return new Vector3(a.y, a.x, a.z);
    }

    public void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private Vector3 Clamp180(Vector3 euler)
    {
        if (euler.x > 180) euler.x -= 360;
        if (euler.y > 180) euler.x -= 360;
        if (euler.z > 180) euler.x -= 360;

        return euler;
    }
}
