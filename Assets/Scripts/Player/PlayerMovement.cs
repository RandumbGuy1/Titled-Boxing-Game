using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("General Movement Settings")]
    //[SerializeField] private float inactiveSpeed = 2f;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    private Vector3 moveDir = Vector3.zero;

    public Vector2 Input { get; private set; } = Vector2.zero;

    public bool Jumping { get; private set; } = false;

    private bool moving = false;
    public bool Moving
    {
        get => moving;

        private set
        {
            if (moving != value) OnPlayerMove?.Invoke(value);
            moving = value;
        }
    }

    public float Magnitude { get; private set; }
    public Vector3 RelativeVel { get; private set; }
    public Vector3 Velocity { get; private set; }
    public float MagToMaxRatio => Mathf.Clamp01(Magnitude / (GetMaxSpeed() * 1.5f));

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
    public Vector3 CrouchOffset => (playerHeight - player.CapsuleCol.height) * transform.localScale.y * Vector3.down;

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
            //Grounded for first frame so play land animations
            if (grounded != value && value) OnPlayerLand?.Invoke(Mathf.Abs(Velocity.y * 0.4f));

            grounded = value;
        }   
    }

    public event PlayerInput.ReceieveFloatInput OnPlayerLand;

    public event PlayerInput.ReceieveBoolInput OnPlayerMove;
    public event PlayerInput.ReceieveBoolInput OnPlayerCrouch;

    [Header("Friction Settings")]
    [SerializeField] private float friction;
    [SerializeField] private float frictionMultiplier;
    [SerializeField] private int counterThresold;
    private Vector2Int readyToCounter = Vector2Int.zero;

    [Header("Refrences")]
    [SerializeField] private PhysicMaterial bouncy;
    [SerializeField] private PhysicMaterial slippery;
    [SerializeField] private PlayerRef player;
    [SerializeField] private Rigidbody rb;
    public Rigidbody Rb => rb;

    public bool Limp => limpElapsed < limpTime;
    private float limpElapsed = 0f;
    private float limpTime;
    private SpringJoint joint;

    void Awake()
    {
        playerHeight = player.CapsuleCol.height;

        OnPlayerLand += (float vel) =>
        {
            rb.AddForceAtPosition((player.Orientation.forward + (Random.insideUnitSphere * 0.2f)).normalized * vel, transform.position + transform.up, ForceMode.Impulse);
        };
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    public void SetFrameInput(FrameInput input)
    {
        Input = input.MoveInput;
        Moving = input.MoveInput != Vector2.zero && !Crouching;
        ReceiveJumpInput(input.JumpInput);
        ReceiveCrouchInput(input.RollInput);
    }

    private void MovePlayer()
    {
        limpElapsed += Time.fixedDeltaTime;
        if (limpElapsed < limpTime) return;

        player.CapsuleCol.material = slippery;

        RelativeVel = player.Orientation.InverseTransformDirection(rb.velocity);

        if (rb.velocity.y <= 0f) rb.AddForce((1.7f - 1f) * Physics.gravity.y * Vector3.up, ForceMode.Acceleration);

        Friction();
        HoverOffGround(CalculateVault());

        float movementMultiplier = 3.5f * Time.fixedDeltaTime * (Grounded ? 1f : 0.6f) * (player.Gloves.CanPreformActions ? 1f : 0.3f);
        ClampSpeed(movementMultiplier);

        Magnitude = rb.velocity.magnitude;
        Velocity = rb.velocity;

        moveDir = player.Orientation.TransformDirection(Input.x, 0, Input.y);

        if (Crouching) return;

        rb.AddForce(acceleration * movementMultiplier * moveDir.normalized, ForceMode.Impulse);
    }

    private Vector3 CalculateVault()
    {
        bool wallCheck = Physics.Raycast(transform.position + player.Orientation.forward, 
            Vector3.down, rideHeight + rideRayExtension * 1.5f, environment);

        return wallCheck ? (player.Orientation.forward + moveDir).normalized : Vector3.zero;
    }

    private void HoverOffGround(Vector3 bufferOffset)
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

    private void ReceiveJumpInput(bool jumping)
    {
        Jumping = jumping;

        if (!Grounded || !Jumping) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);

        player.CameraBody.CamHeadBob.BobOnce(5.5f);
    }

    private float crouchElapsed = 0f;
    private void ReceiveCrouchInput(bool crouching)
    {
        UpdateCrouch();

        if (Crouching)
        {
            //if (player.Gloves.Stamina.RanOutofStamina)
            {
                rb.velocity = Vector3.zero;
            }

            if (crouchElapsed > 0.2f)
            {
                Crouching = false;
                crouchElapsed = 0f;
                return;
            }

            crouchElapsed += Time.deltaTime;
            return;
        }

        slideBoostCooldown = Mathf.Max(0f, slideBoostCooldown - Time.deltaTime);

        if (!crouching || slideBoostCooldown > 0f || !player.Gloves.CanPreformActions) return;

        OnPlayerCrouch?.Invoke(Crouching);
        player.CameraBody.CamHeadBob.BobOnce(1f);
        rb.AddForce(5f * slideBoostSpeed * (Grounded ? 0.8f : 0.1f) * moveDir.normalized, ForceMode.Impulse);
        timeSinceLastSlide = slideBoostCooldown;

        player.Gloves.Stamina.TakeStamina(dashStaminaCost, true);
       // player.Gloves.Block.Blocking = false;

        Crouching = true;
    }

    private void UpdateCrouch()
    {
        float targetScale = Crouching ? crouchHeight : playerHeight;
        float targetCenter = (targetScale - playerHeight) * 0.5f;

        if (player.CapsuleCol.height == targetScale && player.CapsuleCol.center.y == targetCenter) return;
        if (Mathf.Abs(targetScale - player.CapsuleCol.height) < 0.01f && Mathf.Abs(targetCenter - player.CapsuleCol.center.y) < 0.01f)
        {
            player.CapsuleCol.height = targetScale;
            player.CapsuleCol.center = Vector3.one * targetCenter;
        }

        player.CapsuleCol.height = Mathf.SmoothDamp(player.CapsuleCol.height, targetScale, ref crouchVel.x, crouchSmoothTime);
        player.CapsuleCol.center = new Vector3(0, Mathf.SmoothDamp(player.CapsuleCol.center.y, targetCenter, ref crouchVel.y, crouchSmoothTime), 0);

        player.Rendering.transform.localPosition = player.CapsuleCol.center;
        player.Rendering.transform.localScale = new Vector3(1f, player.CapsuleCol.height * 0.5f, 1f);
    }

    private void Friction()
    {
        if (Jumping || !Grounded) return;

        if (Crouching)
        {
            rb.AddForce(slideFriction * -rb.velocity.normalized);
            return;
        }

        Vector3 frictionForce = Vector3.zero;

        if (Mathf.Abs(RelativeVel.x) > 0f && Input.x == 0f && readyToCounter.x > counterThresold || !player.Gloves.CanPreformActions) frictionForce -= player.Orientation.right * RelativeVel.x;
        if (Mathf.Abs(RelativeVel.z) > 0f && Input.y == 0f && readyToCounter.y > counterThresold || !player.Gloves.CanPreformActions) frictionForce -= player.Orientation.forward * RelativeVel.z;

        if (CounterMomentum(Input.x, RelativeVel.x)) frictionForce -= player.Orientation.right * RelativeVel.x;
        if (CounterMomentum(Input.y, RelativeVel.z)) frictionForce -= player.Orientation.forward * RelativeVel.z;

        frictionForce = Vector3.ProjectOnPlane(frictionForce, Vector3.up);
        if (frictionForce != Vector3.zero) rb.AddForce(0.2f * friction * acceleration * frictionForce);

        readyToCounter.x = Input.x == 0f ? readyToCounter.x + 1 : 0;
        readyToCounter.y = Input.y == 0f ? readyToCounter.y + 1 : 0;
    }
    private bool CounterMomentum(float input, float mag, float threshold = 0.05f)
        => input > 0 && mag < -threshold || input < 0 && mag > threshold;

    private void ClampSpeed(float movementMultiplier)
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float coefficientOfFriction = acceleration * movementMultiplier / GetMaxSpeed();

        if (vel.sqrMagnitude > maxSpeed * maxSpeed) rb.AddForce(coefficientOfFriction * -vel, ForceMode.Impulse);
    }
    private float GetMaxSpeed()
    {
        if (Crouching) return crouchMaxSpeed;
        //if (!player.Gloves.CanPreformActions || player.Gloves.Block.Blocking || !player.Gloves.CanPunch) return inactiveSpeed;

        return maxSpeed * (Grounded ? 1f : 1.15f);
    }

    public void GoLimp(float seconds)
    {
        limpTime = seconds;
        limpElapsed = 0f;

        Grounded = false;
        player.CapsuleCol.material = bouncy;
    }

    public void AddSpring(Transform connectedAnchor, float maxDistance)
    {
        if (joint != null) return;

        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = connectedAnchor.position;

        joint.maxDistance = maxDistance;
        joint.minDistance = 0f;

        joint.spring = 6f;
        joint.damper = 10f;
        joint.massScale = 5f;
    }

    void OnDisable()
    {
        Magnitude = 0f;
        RelativeVel = Vector3.zero;
    }

    public void RemoveSpring() => Destroy(joint);

    public void ResetVelocity(float x) => rb.velocity = Vector3.zero;
}
