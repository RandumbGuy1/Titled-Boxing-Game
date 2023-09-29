using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraHeadBob
{
    [Header("View Bob Settings")]
    [SerializeField] private bool enabled = true;
    [SerializeField] private Vector2 viewBobMultiplier;
    [SerializeField] private float viewBobSpeed;
    [SerializeField] private float viewBobDampingRatio;
    [SerializeField] private float viewBobAngularFrequency;
    [Space(10)]
    [SerializeField] private float maxTilt;
    [SerializeField] private float tiltSmoothTime;
    private float viewBobTimer = 0f;
    private float landBobOffset = 0f;

    private Vector3 bobVel = Vector3.zero;
    private float tiltVel = 0f;

    public bool Bobbing => viewBobTimer != 0f;

    public float TiltSway { get; private set; }
    public Vector3 ViewBobOffset { get; private set; }
    public Vector3 ViewBobSnapOffset { get; private set; }

    public void BobUpdate(PlayerRef player)
    {
        viewBobTimer = player.PlayerMovement.Grounded && (player.PlayerMovement.Magnitude > 0.5f && player.PlayerMovement.Moving)
            ? viewBobTimer + Time.deltaTime * viewBobSpeed : 0f;

        if (!enabled) return;

        landBobOffset = Mathf.Min(0, landBobOffset + Time.deltaTime * 35f);

        //Calculate Headbob Spring Motion
        float swimMultiplier = player.PlayerMovement.InWater ? 0.2f : 1f;

        ViewBobSnapOffset = swimMultiplier * HeadBobOffset(viewBobTimer) + Vector3.down * landBobOffset;
        Vector3 smoothHeadBob = ViewBobOffset;

        HarmonicMotion.Calculate(ref smoothHeadBob, ref bobVel, ViewBobSnapOffset, 
            HarmonicMotion.CalcDampedSpringMotionParams(viewBobDampingRatio, viewBobAngularFrequency));
        ViewBobOffset = smoothHeadBob;

        //Calculate Tilt
        float tilt = Mathf.Clamp((player.PlayerMovement.Input.x * maxTilt * 0.75f + player.CameraBody.CamLookSettings.RotationDelta.y * maxTilt), -maxTilt, maxTilt);    
        TiltSway = Mathf.SmoothDamp(TiltSway, -tilt, ref tiltVel, tiltSmoothTime);   
    }

    Vector3 HeadBobOffset(float timer)
    {
        if (timer <= 0) return Vector3.zero;
        return new Vector3(viewBobMultiplier.x * Mathf.Cos(viewBobTimer), viewBobMultiplier.y * Mathf.Abs(Mathf.Sin(viewBobTimer)), 0f);
    }

    public void BobOnce(float magnitude)
    {
        if (!enabled) return;

        landBobOffset -= magnitude;
    }

    public void Enable(bool enabled) => this.enabled = enabled;
}
