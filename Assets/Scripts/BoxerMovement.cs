using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerMovement : MonoBehaviour
{
    public BoxerMoveState State { get; private set; } = BoxerMoveState.Moving;
    private FrameInput boxerInput;

    [Header("Hover Settings")]
    [SerializeField] private LayerMask environment;
    [SerializeField] private float rideRayExtension;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    private bool grounded = false;
    public bool Grounded
    {
        get => grounded;

        private set
        {
            if (grounded != value && value) OnGroundHit?.Invoke(Mathf.Abs(Velocity.y * 0.4f));
            grounded = value;
        }
    }

    public PlayerInput.ReceieveFloatInput OnGroundHit;
    public PlayerInput.ReceieveFloatInput OnRoll;

    public float Magnitude { get; private set; }
    public Vector3 RelativeVel { get; private set; }
    public Vector3 Velocity { get; private set; }

    [Header("Refrences")]
    [SerializeField] private Rigidbody rb;

    void FixedUpdate()
    {
        void HoverOffGround(Vector3 bufferOffset)
        {
            Vector3 bufferPosition = transform.position + bufferOffset;
            bool buffer = Physics.Raycast(bufferPosition, Vector3.down, out var hit, rideHeight + rideRayExtension * 1.5f, environment);

            Grounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out _, rideHeight + rideRayExtension, environment) || buffer;

            if (!buffer) return;

            Vector3 vel = rb.velocity;
            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;

            if (hitBody != null) otherVel = hitBody.velocity;

            float relVel = Vector3.Dot(Vector3.down, vel) - Vector3.Dot(Vector3.down, otherVel);
            float x = hit.distance - rideHeight;
            float springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);

            rb.AddForce(Vector3.down * springForce);

            if (hitBody != null) hitBody.AddForceAtPosition(Vector3.down * -springForce, hit.point);
        }

        switch (State)
        {
            case BoxerMoveState.Moving:

                break;
            case BoxerMoveState.Slipping:

                break;
            case BoxerMoveState.Punching:

                break;
            case BoxerMoveState.Rolling:

                break;
            case BoxerMoveState.Blocking:

                break;
        }

        HoverOffGround(Vector3.zero);
    }

    public void SendFrameInput(FrameInput input) => boxerInput = input;
}

public enum BoxerMoveState
{
    Moving,
    Slipping,
    Punching,
    Rolling,
    Blocking,
}
