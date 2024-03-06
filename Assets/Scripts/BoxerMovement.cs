using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerMovement : MonoBehaviour
{
    [Header("General Movement Settings")]
    [SerializeField] private float inactiveSpeed = 2f;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    private Vector3 moveDir = Vector3.zero;

    [Header("Crouch Settings")]
    [SerializeField] private float dashStaminaCost = 10f;
    [SerializeField] private float slideFriction;
    [SerializeField] private float crouchMaxSpeed;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchSmoothTime;
    [SerializeField] private float slideBoostSpeed;
    [SerializeField] private float slideBoostCooldown;
    private float playerHeight = 0f;
    private float timeSinceLastSlide = 0f;
    private Vector2 crouchVel = Vector2.zero;

    public bool Crouching { get; private set; } = false;
    public Vector3 CrouchOffset => (playerHeight - capsuleCol.height) * transform.localScale.y * Vector3.down;

    [Header("Friction Settings")]
    [SerializeField] private float friction;
    [SerializeField] private float frictionMultiplier;
    [SerializeField] private int counterThresold;
    private Vector2Int readyToCounter = Vector2Int.zero;

    public BoxerMoveState State { get; private set; } = BoxerMoveState.Moving;
    private FrameInput input;

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
    [SerializeField] private RigidbodyHover hover;
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsuleCol;

    void Awake()
    {
        playerHeight = capsuleCol.height;
    }

    void FixedUpdate()
    {
        Grounded = hover.Intersecting;
        RelativeVel = orientation.InverseTransformDirection(rb.velocity);
        Magnitude = rb.velocity.magnitude;
        Velocity = rb.velocity;

        if (rb.velocity.y <= 0f) //Extra Gravity
            rb.AddForce((1.7f - 1f) * Physics.gravity.y * Vector3.up, ForceMode.Acceleration);

        moveDir = orientation.TransformDirection(input.MoveInput.x, 0f, input.MoveInput.y);

        Friction();

        switch (State)
        {
            case BoxerMoveState.Moving:
                float movementMultiplier = 3.5f * Time.fixedDeltaTime * (Grounded ? 1f : 0.6f);
                ClampSpeed(maxSpeed, movementMultiplier);
                rb.AddForce(acceleration * movementMultiplier * moveDir.normalized, ForceMode.Impulse);
                break;
            case BoxerMoveState.Slipping:

                break;
            case BoxerMoveState.Punching:
                movementMultiplier = 3.5f * Time.fixedDeltaTime * (Grounded ? 1f : 0.6f) * 0.1f;
                ClampSpeed(inactiveSpeed, movementMultiplier);
                rb.AddForce(acceleration * movementMultiplier * moveDir.normalized, ForceMode.Impulse);

                break;
            case BoxerMoveState.Rolling:

                break;
            case BoxerMoveState.Blocking:

                break;
        }
    }

    public void SendFrameInput(FrameInput input) => this.input = input;

    private void Friction()
    {
        if (input.JumpInput || !Grounded) return;

        if (Crouching)
        {
            rb.AddForce(slideFriction * -rb.velocity.normalized);
            return;
        }

        Vector3 frictionForce = Vector3.zero;

        if (Mathf.Abs(RelativeVel.x) > 0f && input.MoveInput.x == 0f && readyToCounter.x > counterThresold) frictionForce -= orientation.right * RelativeVel.x;
        if (Mathf.Abs(RelativeVel.z) > 0f && input.MoveInput.y == 0f && readyToCounter.y > counterThresold) frictionForce -= orientation.forward * RelativeVel.z;

        if (CounterMomentum(input.MoveInput.x, RelativeVel.x)) frictionForce -= orientation.right * RelativeVel.x;
        if (CounterMomentum(input.MoveInput.y, RelativeVel.z)) frictionForce -= orientation.forward * RelativeVel.z;

        frictionForce = Vector3.ProjectOnPlane(frictionForce, Vector3.up);
        if (frictionForce != Vector3.zero) rb.AddForce(0.2f * friction * acceleration * frictionForce);

        readyToCounter.x = input.MoveInput.x == 0f ? readyToCounter.x + 1 : 0;
        readyToCounter.y = input.MoveInput.y == 0f ? readyToCounter.y + 1 : 0;
    }
    private bool CounterMomentum(float input, float mag, float threshold = 0.05f)
        => input > 0 && mag < -threshold || input < 0 && mag > threshold;

    private void ClampSpeed(float maxSpeed, float movementMultiplier)
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float coefficientOfFriction = acceleration * movementMultiplier / maxSpeed;

        if (vel.sqrMagnitude > maxSpeed * maxSpeed) rb.AddForce(coefficientOfFriction * -vel, ForceMode.Impulse);
    }
}

public enum BoxerMoveState
{
    Moving,
    Slipping,
    Punching,
    Rolling,
    Blocking,
}
