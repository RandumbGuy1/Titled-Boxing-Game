using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraHeadBob : MonoBehaviour
{
    [Header("View Bob Settings")]
    [SerializeField] private Vector2 viewBobMultiplier;
    [SerializeField] private float viewBobSpeed;
    [SerializeField] private float viewBobDampingRatio;
    [SerializeField] private float viewBobAngularFrequency;
    private float viewBobTimer = 0f;
    private float landBobOffset = 0f;

    private Vector3 bobVel = Vector3.zero;
    public bool Bobbing => viewBobTimer != 0f;
    public Vector3 ViewBobOffset { get; private set; }
    public Vector3 ViewBobSnapOffset { get; private set; }

    public void BobUpdate(bool increment)
    {
        viewBobTimer = increment ? viewBobTimer + Time.deltaTime * viewBobSpeed : 0f;

        if (!enabled) return;

        landBobOffset = Mathf.Min(0, landBobOffset + Time.deltaTime * 35f);

        ViewBobSnapOffset = HeadBobOffset(viewBobTimer) + Vector3.down * landBobOffset;
        Vector3 smoothHeadBob = ViewBobOffset;

        HarmonicMotion.Calculate(ref smoothHeadBob, ref bobVel, ViewBobSnapOffset, 
            HarmonicMotion.CalcDampedSpringMotionParams(viewBobDampingRatio, viewBobAngularFrequency));
        ViewBobOffset = smoothHeadBob;
    }

    Vector3 HeadBobOffset(float timer)
    {
        if (timer <= 0) return Vector3.zero;
        return new Vector3(viewBobMultiplier.x * Mathf.Cos(viewBobTimer), viewBobMultiplier.y * Mathf.Sin(viewBobTimer), 0f);
    }

    public void BobOnce(float magnitude)
    {
        if (!enabled) return;

        landBobOffset -= magnitude;
    }

    public void Enable(bool enabled) => this.enabled = enabled;
}
