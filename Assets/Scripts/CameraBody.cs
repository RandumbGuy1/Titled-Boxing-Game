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

    public bool CanMoveCamera { get; private set; } = true;

    void Awake()
    {
        SetCursorState(true);

        player.PlayerInput.OnMouseInput += camLookSettings.LookUpdate;
        player.PlayerInput.OnPerspectiveToggle += (bool toggle) => {
            if (!toggle) return;

            camCollider.Enabled = !camCollider.Enabled;
            player.Rendering.shadowCastingMode = camCollider.Enabled ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
        };

        player.PlayerMovement.OnPlayerLand += camHeadBob.BobOnce;
    }

    void Update()
    {
        player.PlayerCam.fieldOfView = camFov.FOVUpdate(player);

        camSprintEffect.SpeedLines(player);
        camIdleSway.IdleCameraSway(player);
        camHeadBob.BobUpdate(player);
        camCollider.ColliderUpdate(player.PlayerCam.transform.position, player.transform.position);
    }

    void LateUpdate()
    {
        smoothDeltaRotation = Vector3.Lerp(smoothDeltaRotation, deltaRotation, Time.deltaTime * 25f);

        //Apply Rotations And Positions
        {
            Quaternion newCamRot = Quaternion.Euler((Vector3) camLookSettings.SmoothRotation + smoothDeltaRotation + camIdleSway.HeadSwayOffset);
            Quaternion newPlayerRot = Quaternion.Euler(0f, camLookSettings.SmoothRotation.y + deltaRotation.y, 0f);

            player.Orientation.localRotation = newPlayerRot;
            transform.localRotation = newCamRot;
            player.PlayerCam.transform.localRotation = Quaternion.Euler(ToEuler(camHeadBob.ViewBobOffset) + Vector3.forward * camHeadBob.TiltSway + camShaker.Offset);

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

    private Vector3 deltaRotation = Vector3.zero;
    private Vector3 smoothDeltaRotation = Vector3.zero;

    public void SetMoveCamera(bool move)
    {
        if (CanMoveCamera == move) return;
             
        CanMoveCamera = move;

        if (CanMoveCamera) player.PlayerInput.OnMouseInput += camLookSettings.LookUpdate;
        else player.PlayerInput.OnMouseInput -= camLookSettings.LookUpdate;
    }
        

    public void LookAt(Vector3 fromTo)
    {
        deltaRotation.y += Vector3.SignedAngle(player.Orientation.forward, fromTo, Vector3.up);
    }
}
