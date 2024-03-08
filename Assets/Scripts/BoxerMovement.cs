using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class BoxerMovement : MonoBehaviour
{
    [Header("General Movement Settings")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float uprightStrength = 20f;
    [SerializeField] private float uprightDampening = 2f;

    [Header("Crouch Settings")]
    [SerializeField] private float dashStaminaCost = 10f;
    [SerializeField] private float slideFriction;
    [SerializeField] private float crouchMaxSpeed;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchSmoothTime;
    [SerializeField] private float slideBoostSpeed;
    [SerializeField] private float slideBoostCooldown;
    private float crouchElapsed = 0f;
    private float playerHeight = 0f;
    private Vector2 crouchVel = Vector2.zero;

    public bool Rolling => State == BoxerMoveState.Rolling;
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

    public UnityEvent<float> OnJump;
    public UnityEvent<float> OnGroundHit;
    public UnityEvent<bool> OnRoll;
    public UnityEvent<bool> OnMove;

    public float Magnitude { get; private set; }
    public Vector3 RelativeVel { get; private set; }
    public Vector3 Velocity { get; private set; }
    public float VelToMaxRatio => Mathf.Clamp01(Magnitude / (maxSpeed * 1.5f));

    [Header("Refrences")]
    [SerializeField] private BoxingController gloves;
    [SerializeField] private MeshRenderer gfx;
    [SerializeField] private RigidbodyHover hover;
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsuleCol;

    public Rigidbody Rb => rb;

    void Awake()
    {
        playerHeight = capsuleCol.height;
    }

    void FixedUpdate()
    {
        if (input == null) return;

        void ClampSpeed(float maxSpeed, float movementMultiplier)
        {
            Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float coefficientOfFriction = acceleration * movementMultiplier / maxSpeed;

            if (vel.sqrMagnitude > maxSpeed * maxSpeed) rb.AddForce(coefficientOfFriction * -vel, ForceMode.Impulse);
        }

        void Friction()
        {
            if (input.JumpInput || !Grounded) return;

            if (Rolling)
            {
                rb.AddForce(slideFriction * -rb.velocity.normalized);
                return;
            }

            static bool CounterMomentum(float input, float mag, float threshold = 0.05f) => input > 0 && mag < -threshold || input < 0 && mag > threshold;

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

        Grounded = hover.Intersecting;
        RelativeVel = orientation.InverseTransformDirection(rb.velocity);
        Magnitude = rb.velocity.magnitude;
        Velocity = rb.velocity;

        if (rb.velocity.y <= 0f) rb.AddForce((1.7f - 1f) * Physics.gravity.y * Vector3.up, ForceMode.Acceleration);

        UpdateSpring(uprightStrength, uprightDampening);
        Friction();

        switch (State)
        {
            case BoxerMoveState.Moving:
                float movementMultiplier = 3.5f * Time.fixedDeltaTime * (Grounded ? 1f : 0.6f);
                ClampSpeed(maxSpeed, movementMultiplier);
                rb.AddForce(acceleration * movementMultiplier * input.MoveDir.normalized, ForceMode.Impulse);
                break;
            case BoxerMoveState.Slipping:

                break;
            case BoxerMoveState.Rolling:

                break;
        }
    }

    public void UpdateSpring(float strength, float dampening)
    {
        rb.AddTorque(Vector3.right * ((0 - rb.rotation.x) * strength * 5f - (rb.angularVelocity.x * dampening)));
        rb.AddTorque(Vector3.up * ((0 - rb.rotation.y) * strength * 5f - (rb.angularVelocity.y * dampening)));
        rb.AddTorque(Vector3.forward * ((0 - rb.rotation.z) * strength * 5f - (rb.angularVelocity.z * dampening)));
    }

    public void SendFrameInput(FrameInput input)
    {
        if (!enabled) return;

        this.input = input;

        ReceiveJumpInput(input.JumpInput);
        ReceiveRollInput(input.RollInput);
    }

    private void ReceiveJumpInput(bool jumping)
    {
        if (!Grounded || !jumping) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
    }
    private void ReceiveRollInput(bool rolling)
    {
        UpdateRoll();

        if (Rolling)
        {
            if (gloves.Stamina.RanOutofStamina) rb.velocity = Vector3.zero;

            if (crouchElapsed > 0.2f)
            {
                State = BoxerMoveState.Moving;
                crouchElapsed = 0f;
                return;
            }

            crouchElapsed += Time.deltaTime;
            return;
        }

        slideBoostCooldown = Mathf.Max(0f, slideBoostCooldown - Time.deltaTime);

        if (!rolling || slideBoostCooldown > 0f || !gloves.CanPreformActions) return;

        OnRoll?.Invoke(rolling);

        rb.AddForce(5f * slideBoostSpeed * (Grounded ? 0.8f : 0.1f) * input.MoveDir.normalized, ForceMode.Impulse);

        gloves.Stamina.TakeStamina(dashStaminaCost, true);
        gloves.Block.Blocking = false;

        State = BoxerMoveState.Rolling;
    }

    private void UpdateRoll()
    {
        float targetScale = Rolling ? crouchHeight : playerHeight;
        float targetCenter = (targetScale - playerHeight) * 0.5f;

        if (capsuleCol.height == targetScale && capsuleCol.center.y == targetCenter) return;
        if (Mathf.Abs(targetScale - capsuleCol.height) < 0.01f && Mathf.Abs(targetCenter - capsuleCol.center.y) < 0.01f)
        {
            capsuleCol.height = targetScale;
            capsuleCol.center = Vector3.one * targetCenter;
        }

        capsuleCol.height = Mathf.SmoothDamp(capsuleCol.height, targetScale, ref crouchVel.x, crouchSmoothTime);
        capsuleCol.center = new Vector3(0, Mathf.SmoothDamp(capsuleCol.center.y, targetCenter, ref crouchVel.y, crouchSmoothTime), 0);

        gfx.transform.localPosition = capsuleCol.center;
        gfx.transform.localScale = new Vector3(1f, capsuleCol.height * 0.5f, 1f);
    }
}